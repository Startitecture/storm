Startitecture.Orm (ST/ORM)
======================
ST/ORM is an object-relational mapper that combines the speed of ORMs like [Dapper](https://github.com/StackExchange/Dapper) and [PetaPoco](https://github.com/CollaboratingPlatypus/PetaPoco/wiki) with the rich POCO composition and query features of traditional ORMs.

Initially based on the work done in PetaPoco, ST/ORM represents an 8 year journey in schema modeling, query language, data design, and object hyrdration, built (and re-built) for use in private corporations. [Startitecture](https://startitecture.com) has gained the rights to redistribute those substantial modifications from the original open source PetaPoco library as a fully open source package.

The full API documentation for ST/ORM is available [here](https://startitecture.github.io/storm/).   

Vision
---
ST/ORM is designed with a single purpose - to overcome the inherent problems in using ORM POCOs as dual purpose domain and data models, while maintaining or increasing development productivity.

In any complex application, domain (runtine) models and data (persistence) models will diverge, and developers are faced with a variety of unpleasant choices:

1. Extend ORM data entities to support domain model methods and properties and deal with the performace issues of traditonal ORMs
2. Build anemic domain models based on entities, increasing coupling and decreasing cohesion in the application layer
3. Build fully separate domain and data models, and rely either on significant customization or application-layer aggregation for hyrdating complex entity aggregates

ST/ORM eliminates these issues by:

* Maintaining the original IL hydration model found in PetaPoco and Dapper
* Extending it to support an unlimited object graph
* Avoiding object cloning with primary key-based item caches
* Reverse POCO generation based on [System.ComponentModel.DataAnnotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) and [System.ComponentModel.DataAnnotations.Schema](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema?view=netcore-3.1).
* Out-of-the-box support for CRUD operations using the repository pattern
* Providing a fluent query interface for unlimited LEFT and INNER JOINS
* Supporting an ever-increasing subset of set-based SQL predicates
* Supporting MERGE and INSERT operations with table valued parameters (SQL Server) or JSON (PostgreSQL)
* Async methods for all operations with downstream async support in `DbConnection`, `DbCommand`, and `DbTransaction`

In future releases, ST/ORM will add support for MySQL, Oracle, and SQLite.

In addition to functionality, ST/ORM is fully customizable, with interfaces for every facet of operation, including Object-Object mapping. ST/ORM has been tested with version 10.0.0 of [AutoMapper](https://automapper.org/) with object graph self-referencing intact. That means if it can be mapped, it can be directly hydrated by a ST/ORM Entity Repository. (To integrate AutoMapper, install the [Startitecture.Orm.AutoMapper](https://www.nuget.org/packages/Startitecture.Orm.AutoMapper/) package.)

Quick Start Guide
---

To get started, install the [Startitecture.Orm](https://www.nuget.org/packages/Startitecture.Orm/) NuGet package. For setting up POCO entities, refer to the readme.txt in the project. 

_(Note: at this time, POCO generation is not supported for platforms outside SQL Server. However, it is possible to manually create them, or use another T4 template that generates equivalent POCOs and DataAnnotations decorations and they will work with any database adapter.)_

Next, install the NuGet package appropriate for the target database ([Startitecture.Orm.SqlClient](https://www.nuget.org/packages/Startitecture.Orm.SqlClient/) for SQL Server or [Startitecture.Orm.PostgreSql](https://www.nuget.org/packages/Startitecture.Orm.PostgreSql/) for PostgreSQL). 

### Complex POCOs

ST/ORM will work with individual POCOs representing a single entity, but its real power comes from the ability to represent and query aggregate entities in a database as complex objects. ST/ORM relies on explicitly composed POCO classes for greater control. 

Generated POCOs can be extended and composed with other POCOs to create entity aggregates:
```C#
public partial class DomainAggregateRow : IEquatable<DomainAggregateRow>
{
    [Relation]
    public SubContainerRow SubContainer { get; set; }

    [Relation]
    public AggregateOptionRow AggregateOption { get; set; }

    [Relation]
    public TemplateRow Template { get; set; }

    [Relation]
    public CategoryAttributeRow CategoryAttribute { get; set; }

    [Relation]
    public DomainIdentityRow CreatedBy { get; set; }

    [Relation]
    public DomainIdentityRow LastModifiedBy { get; set; }

    [Relation]
    public OtherAggregateRow OtherAggregate { get; set; }
}

public partial class SubContainerRow : IEquatable<SubContainerRow>
{
    [Relation]
    public TopContainerRow TopContainer { get; set; }
}
```
There are no limitations on complexity, and ST/ORM will traverse the entire object graph while automatically short-cutting any circular references.

### IDatabaseContext

ST/ORM provides multiple options for database interaction. The lowest level is the `IDatabaseContext` interface, which provides access to the `IDbConnection`, contains a method for starting an `IDbTransaction`, and exposes execution and query methods for executing parameterized SQL statements.

Example using the `ExecuteScalar` and `Query` methods:
```C#
var connectionString = ConfigurationRoot.GetConnectionString("MasterDatabase");
var definitionProvider = new DataAnnotationsDefinitionProvider();
var repositoryAdapter = new TransactSqlAdapter(definitionProvider);

using (var target = new DatabaseContext(connectionString, providerName, repositoryAdapter))
{
    var tables = target.Query<dynamic>("SELECT * FROM sys.tables WHERE [type] = @0", 'U').ToList();
    Assert.IsTrue(tables.Any());
    Assert.IsNotNull(tables.FirstOrDefault()?.name);
    Assert.IsTrue(tables.FirstOrDefault()?.object_id > 0);

    var tableCount = target.ExecuteScalar<int>("SELECT COUNT(1) FROM sys.tables WHERE [type] = @0", 'U');
    Assert.AreNotEqual(0, tableCount);
}
``` 
Above, the `dynamic` type is used - ST/ORM (like PetaPoco and Dapper) automatically creates properties on dynamic objects based on the columns in the `IDataReader`.
 
### IRepositoryProvider

The next level for database interaction is the `IRepositoryProvider` interface, which expands upon `IDatabaseContext` with common CRUD operation methods, encapsulating SQL queries with `EntitySelection<T>` and the `ISelection` interface. The fluent SQL creation language includes support for `COUNT()`, paging operations, UNIONS, INTERSECTS, and EXCEPT clauses, and limited support for CTEs (the CTE will be joined to the main query by explicit column matching as a filter).

An extensive example below:
```C#
List<DomainAggregateRow> expectedPage1;
List<DomainAggregateRow> expectedPage2;

var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
{
    var topContainer2 = new TopContainerRow
    {
        Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
    };

    target.Insert(topContainer2);

    var subContainerA = new SubContainerRow
    {
        Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
        TopContainer = topContainer2,
        TopContainerId = topContainer2.TopContainerId
    };

    target.Insert(subContainerA);

    var categoryAttribute20 = new CategoryAttributeRow
    {
        Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
        IsActive = true,
        IsSystem = false
    };

    target.Insert(categoryAttribute20);

    var timBobIdentity = new DomainIdentityRow
    {
        FirstName = "Tim",
        LastName = "Bob",
        UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
    };

    target.Insert(timBobIdentity);

    var fooBarIdentity = new DomainIdentityRow
    {
        FirstName = "Foo",
        LastName = "Bar",
        UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
    };

    target.Insert(fooBarIdentity);

    var otherAggregate10 = new OtherAggregateRow
    {
        Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
        AggregateOptionTypeId = 3
    };

    target.Insert(otherAggregate10);

    var template23 = new TemplateRow
    {
        Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
    };

    target.Insert(template23);

    var aggregateOption1 = new AggregateOptionRow
    {
        Name = $"UNIT_TEST:AgOption1-{Generator.Next(int.MaxValue)}",
        AggregateOptionTypeId = 2,
        Value = 439034.0332m
    };

    var aggregateOption2 = new AggregateOptionRow
    {
        Name = $"UNIT_TEST:AgOption2-{Generator.Next(int.MaxValue)}",
        AggregateOptionTypeId = 4,
        Value = 32453253
    };

    var domainAggregate1 = new DomainAggregateRow
    {
        AggregateOption = aggregateOption1,
        CategoryAttribute = categoryAttribute20,
        CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
        Name = $"UNIT_TEST:Aggregate1-{Generator.Next(int.MaxValue)}",
        Description = "My First Domain Aggregate",
        CreatedBy = timBobIdentity,
        CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        CreatedTime = DateTimeOffset.Now.AddMonths(-1),
        LastModifiedBy = fooBarIdentity,
        LastModifiedByDomainIdentityId = fooBarIdentity.DomainIdentityId,
        LastModifiedTime = DateTimeOffset.Now,
        SubContainer = subContainerA,
        SubContainerId = subContainerA.SubContainerId,
        Template = template23,
        TemplateId = template23.TemplateId
    };

    target.Insert(domainAggregate1);

    var domainAggregate2 = new DomainAggregateRow
    {
        AggregateOption = aggregateOption2,
        CategoryAttribute = categoryAttribute20,
        CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
        Name = $"UNIT_TEST:Aggregate2-{Generator.Next(int.MaxValue)}",
        Description = "My Second Domain Aggregate",
        CreatedBy = timBobIdentity,
        CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        CreatedTime = DateTimeOffset.Now.AddMonths(-1),
        LastModifiedBy = fooBarIdentity,
        LastModifiedByDomainIdentityId = fooBarIdentity.DomainIdentityId,
        LastModifiedTime = DateTimeOffset.Now,
        OtherAggregate = otherAggregate10,
        SubContainer = subContainerA,
        SubContainerId = subContainerA.SubContainerId,
        Template = template23,
        TemplateId = template23.TemplateId
    };

    target.Insert(domainAggregate2);

    var domainAggregate3 = new DomainAggregateRow
    {
        CategoryAttribute = categoryAttribute20,
        CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
        Name = $"UNIT_TEST:Aggregate3-{Generator.Next(int.MaxValue)}",
        Description = "My Third Domain Aggregate",
        CreatedBy = timBobIdentity,
        CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        CreatedTime = DateTimeOffset.Now.AddMonths(-1),
        LastModifiedBy = timBobIdentity,
        LastModifiedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        LastModifiedTime = DateTimeOffset.Now,
        SubContainer = subContainerA,
        SubContainerId = subContainerA.SubContainerId,
        Template = template23,
        TemplateId = template23.TemplateId
    };

    target.Insert(domainAggregate3);

    var domainAggregate4 = new DomainAggregateRow
    {
        CategoryAttribute = categoryAttribute20,
        CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
        Name = $"UNIT_TEST:Aggregate4-{Generator.Next(int.MaxValue)}",
        Description = "My Fourth Domain Aggregate",
        CreatedBy = timBobIdentity,
        CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        CreatedTime = DateTimeOffset.Now.AddMonths(-1),
        LastModifiedBy = timBobIdentity,
        LastModifiedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        LastModifiedTime = DateTimeOffset.Now,
        SubContainer = subContainerA,
        SubContainerId = subContainerA.SubContainerId,
        Template = template23,
        TemplateId = template23.TemplateId
    };

    target.Insert(domainAggregate4);

    var domainAggregate5 = new DomainAggregateRow
    {
        CategoryAttribute = categoryAttribute20,
        CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
        Name = $"UNIT_TEST:Aggregate5-{Generator.Next(int.MaxValue)}",
        Description = "My Fifth Domain Aggregate",
        CreatedBy = timBobIdentity,
        CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        CreatedTime = DateTimeOffset.Now.AddMonths(-1),
        LastModifiedBy = timBobIdentity,
        LastModifiedByDomainIdentityId = timBobIdentity.DomainIdentityId,
        LastModifiedTime = DateTimeOffset.Now,
        SubContainer = subContainerA,
        SubContainerId = subContainerA.SubContainerId,
        Template = template23,
        TemplateId = template23.TemplateId
    };

    target.Insert(domainAggregate5);

    aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
    target.Insert(aggregateOption1);

    aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
    target.Insert(aggregateOption2);

    var associationRow = new AssociationRow
    {
        DomainAggregateId = domainAggregate2.DomainAggregateId,
        OtherAggregateId = otherAggregate10.OtherAggregateId
    };

    target.Insert(associationRow);

    expectedPage1 = new List<DomainAggregateRow>
                        {
                            domainAggregate1,
                            domainAggregate2,
                            domainAggregate3
                        };

    expectedPage2 = new List<DomainAggregateRow>
                        {
                            domainAggregate4,
                            domainAggregate5
                        };
}

using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
{
    var countQuery = Query.SelectEntities<DomainAggregateRow>(
        select => select.Count(row => row.DomainAggregateId)
            .From(set => set.InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId))
            .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId)));

    // TODO: How will we label our aggregate columns?
    var count = target.GetScalar<int>(countQuery);

    Assert.AreEqual(5, count);

    var tableExpression = Query.SelectEntities<DomainAggregateRow>(
        select => select.Select(row => row.DomainAggregateId)
            .From(
                set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                    .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                    .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                    .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                    .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                    .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                    .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                    .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                    .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
            .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId))
            .Sort(set => set.OrderBy(row => row.Name))
            .Seek(subset => subset.Skip(0).Take(3)));

    var selection = Query.With(tableExpression, "pgCte")
        .ForSelection<DomainAggregateRow>(matches => matches.On(row => row.DomainAggregateId, row => row.DomainAggregateId))
        .From(
            set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
        .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId))
        .Sort(set => set.OrderBy(row => row.Name));

    var actualPage1 = target.SelectEntities(selection).ToList();
    Assert.AreEqual(
        expectedPage1.First(),
        actualPage1.First(),
        string.Join(Environment.NewLine, expectedPage1.First().GetDifferences(actualPage1.First())));

    CollectionAssert.AreEqual(expectedPage1, actualPage1);

    // Advance the number of rows
    selection.ParentExpression.Expression.Page.SetPage(2);

    var actualPage2 = target.SelectEntities(selection).ToList();
    Assert.AreEqual(
        expectedPage2.First(),
        actualPage2.First(),
        string.Join(Environment.NewLine, expectedPage2.First().GetDifferences(actualPage2.First())));

    CollectionAssert.AreEqual(expectedPage2, actualPage2);
}
```
### IEntityRepository

The final level of database interaction is the `IEntityRepository<TModel>` interface, which adds entity to domain model mapping via an `IEntityMapper`. It is possible to use the latest version of [AutoMapper](https://automapper.org/) with the [Startitecture.Orm.AutoMapper](https://www.nuget.org/packages/Startitecture.Orm.AutoMapper/) NuGet package.

Both the SQL Server (`SqlClientRepository<TModel, TEntity>`) and PostgreSQL (`PostgreSqlRepository<TModel, TEntity>`) implementations contain additional methods for insert and merge (or upsert) of multiple rows. 

#### SqlClientRepository

For SQL Server, INSERT and MERGE commands using a table-valued parameter (TVP) require a user-defined table type (UDTT) in the target database, represented in code using a POCO:
```C#
/// <summary>
/// Represents a user-defined table type for the <see cref="FieldRow"/> entity.
/// </summary>
[TableType("FieldTableType")]
[Table("Field", Schema = "dbo")]
public class FieldTableTypeRow
{
    /// <summary>
    /// Gets or sets the field ID.
    /// </summary>
    [Column(Order = 1)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? FieldId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Column(Order = 2)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [Column(Order = 3)]
    public string Description { get; set; }
}
```
The same UDTT can be used for both inserts and merges. 

Multiple row insert for SQL Server:
```C#
var internalId = new Field
{
    Name = "INS_Internal ID",
    Description = "Unique ID used internally"
};

var firstName = new Field
{
    Name = "INS_First Name",
    Description = "The person's first name"
};

var lastName = new Field
{
    Name = "INS_Last Name",
    Description = "The person's last name"
};

var yearlyWage = new Field
{
    Name = "INS_Yearly Wage",
    Description = "The base wage paid year over year."
};

var hireDate = new Field
{
    Name = "INS_Hire Date",
    Description = "The date and time of hire for the person"
};

var bonusTarget = new Field
{
    Name = "INS_Bonus Target",
    Description = "The target bonus for the person"
};

var contactNumbers = new Field
{
    Name = "INS_Contact Numbers",
    Description = "A list of contact numbers for the person in order of preference"
};

var expected = new List<Field>
                {
                    internalId,
                    firstName,
                    lastName,
                    yearlyWage,
                    hireDate,
                    bonusTarget,
                    contactNumbers
                };

var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
using (var transaction = provider.BeginTransaction())
{
    var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, this.mapper);
    var fieldValueRepository = new SqlClientRepository<FieldValue, FieldValueRow>(provider, this.mapper);

    var fieldSelection = Query.SelectEntities<FieldRow>(
        select => select.Select(row => row.FieldId).Where(set => set.AreEqual(row => row.Name, "INS_%")));

    var fieldValuesToDelete = fieldRepository.DynamicSelect(fieldSelection);
    fieldValueRepository.DeleteSelection(
        Query.Select<FieldValueRow>()
            .Where(set => set.Include(row => row.FieldId, fieldValuesToDelete.Select(o => (int)o.FieldId).ToArray())));

    fieldRepository.DeleteSelection(fieldSelection);

    var fieldRows = from f in expected
                    select new FieldTableTypeRow
                    {
                        Name = f.Name,
                        Description = f.Description
                    };

    // Select results = by name because the TVP won't have the IDs
    // TODO: See if we can simplify this process to just pull inserted items without joins
    var results = fieldRepository.InsertForResults(fieldRows, insert => insert.SelectResults(row => row.Name)).ToList();

    transaction.Commit();

    var actual = this.mapper.Map<List<Field>>(results);
    CollectionAssert.AreEquivalent(expected, actual);
}
```
MERGE with SQL Server:
```C#
var internalId = new Field
{
    Name = "MERGE_Existing_Internal ID",
    Description = "Unique ID used internally"
};

var firstName = new Field
{
    Name = "MERGE_Existing_First Name",
    Description = "The person's first name"
};

var lastName = new Field
{
    Name = "MERGE_Existing_Last Name",
    Description = "The person's last name"
};

var yearlyWage = new Field
{
    Name = "MERGE_Existing_Yearly Wage",
    Description = "The base wage paid year over year."
};

var hireDate = new Field
{
    Name = "MERGE_NonExisting_Hire Date",
    Description = "The date and time of hire for the person"
};

var bonusTarget = new Field
{
    Name = "MERGE_NonExisting_Bonus Target",
    Description = "The target bonus for the person"
};

var contactNumbers = new Field
{
    Name = "MERGE_NonExisting_Contact Numbers",
    Description = "A list of contact numbers for the person in order of preference"
};

var fields = new List<Field>
                {
                    internalId,
                    firstName,
                    lastName,
                    yearlyWage,
                    hireDate,
                    bonusTarget,
                    contactNumbers
                };

var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
using (var transaction = provider.BeginTransaction())
{
    var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, this.mapper);
    fieldRepository.Merge(
        from f in fields
        select new FieldTableTypeRow
        {
            Name = f.Name,
            Description = f.Description
        },
        merge => merge.On<FieldTableTypeRow>(row => row.Name, row => row.Name));

    transaction.Commit();
}
```
#### PostreSqlRepository
PostgreSQL, via the NPGSQL ADO.NET provider, supports the direct translation of BCL types to JSONB parameters, so no additional types are required. In the example below, the same UDTT POCOs are reused for convenience but they could just as easily be REST contract DTOs without decoration.
```C#
var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
{
    var identityRepository = new PostgreSqlRepository<DomainIdentity, DomainIdentityRow>(provider, this.mapper);

    var domainIdentity = identityRepository.FirstOrDefault(
                                Query.From<DomainIdentity>()
                                    .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, Environment.UserName)))
                            ?? identityRepository.Save(
                                new DomainIdentity(Environment.UserName)
                                {
                                    FirstName = "King",
                                    MiddleName = "T.",
                                    LastName = "Animal"
                                });

    var expected = new GenericSubmission("My Submission", domainIdentity);
    var internalId = new Field
    {
        Name = "Internal ID",
        Description = "Unique ID used internally"
    };

    var firstName = new Field
    {
        Name = "First Name",
        Description = "The person's first name"
    };

    var lastName = new Field
    {
        Name = "Last Name",
        Description = "The person's last name"
    };

    var yearlyWage = new Field
    {
        Name = "Yearly Wage",
        Description = "The base wage paid year over year."
    };

    var hireDate = new Field
    {
        Name = "Hire Date",
        Description = "The date and time of hire for the person"
    };

    var bonusTarget = new Field
    {
        Name = "Bonus Target",
        Description = "The target bonus for the person"
    };

    var contactNumbers = new Field
    {
        Name = "Contact Numbers",
        Description = "A list of contact numbers for the person in order of preference"
    };

    expected.SetValue(internalId, 9234);
    expected.SetValue(firstName, "Dan");
    expected.SetValue(lastName, "The Man");
    expected.SetValue(yearlyWage, 75100.35m);
    expected.SetValue(hireDate, DateTimeOffset.Now);
    expected.SetValue(bonusTarget, 1.59834578934);
    expected.SetValue(
        contactNumbers,
        new List<string>
            {
                "423-222-2252",
                "615-982-0012",
                "+1-555-252-5521"
            });

    expected.Submit();

    var fieldRepository = new PostgreSqlRepository<Field, FieldRow>(provider, this.mapper);

    var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
    var inclusionValues = fields.Keys.ToArray();
    var existingFields = fieldRepository.SelectEntities(
        new EntitySelection<Field>().Where(set => set.Include(field => field.Name, inclusionValues)));

    foreach (var field in existingFields)
    {
        var output = fields[field.Name];
        this.mapper.MapTo(field, output);
    }

    foreach (var field in fields.Values.Where(field => field.FieldId.HasValue == false))
    {
        fieldRepository.Save(field);
    }

    var submissionRepository = new PostgreSqlRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);

    using (var transaction = provider.BeginTransaction())
    {
        submissionRepository.Save(expected);

        var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
        Assert.AreNotEqual(0, submissionId);

        var fieldValueRepository = new PostgreSqlRepository<FieldValue, FieldValueRow>(provider, this.mapper);

        // Do the field values
        var valuesList = from v in expected.SubmissionValues
                            select new FieldValueRow
                                {
                                    FieldId = v.Field.FieldId.GetValueOrDefault(),
                                    LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                    LastModifiedTime = expected.SubmittedTime
                                };

        var insertedValues = fieldValueRepository.InsertForResults(
                valuesList,
                insert => insert.Returning(
                    row => row.FieldValueId,
                    row => row.FieldId,
                    row => row.LastModifiedByDomainIdentifierId,
                    row => row.LastModifiedTime))
            .ToDictionary(row => row.FieldId, row => row);

        // Map back to the domain object if needed.
        foreach (var value in expected.SubmissionValues.Where(value => value.FieldValueId.HasValue == false))
        {
            var input = insertedValues[value.Field.FieldId.GetValueOrDefault()];
            this.mapper.MapTo(input, value);
        }

        var fieldValueElementRepository = new PostgreSqlRepository<FieldValueElement, FieldValueElementRow>(provider, this.mapper);

        // Do the field value elements
        var elementsList = (from e in expected.SubmissionValues.SelectMany(value => value.Elements)
                            select new FieldValueElementTableTypeRow
                                    {
                                        FieldValueElementId = e.FieldValueElementId,
                                        FieldValueId = e.FieldValue.FieldValueId.GetValueOrDefault(),
                                        Order = e.Order,
                                        DateElement = e.Element as DateTimeOffset? ?? e.Element as DateTime?,
                                        FloatElement = e.Element as double? ?? e.Element as float?,
                                        IntegerElement =
                                            e.Element as long? ?? e.Element as int? ?? e.Element as short? ?? e.Element as byte?,
                                        MoneyElement = e.Element as decimal?,
                                        TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                    }).ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order));

        var insertedElements = fieldValueElementRepository.InsertForResults(
                elementsList.Values,
                insert => insert.Returning(row => row.FieldValueElementId, row => row.FieldValueId, row => row.Order))
            .ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order));

        foreach (var element in expected.SubmissionValues.SelectMany(value => value.Elements))
        {
            var key = new Tuple<long, int>(element.FieldValue.FieldValueId.GetValueOrDefault(), element.Order);
            var input = insertedElements[key];
            this.mapper.MapTo(input, element);
            elementsList[key].FieldValueElementId = input.FieldValueElementId;
        }

        var dateElementRepository = new PostgreSqlRepository<FieldValueElement, DateElementRow>(provider, this.mapper);
        dateElementRepository.Insert(
            elementsList.Values.Where(row => row.DateElement.HasValue),
            insert => insert.InsertInto(row => row.DateElementId, row => row.Value)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement));

        var floatElementRepository = new PostgreSqlRepository<FieldValueElement, FloatElementRow>(provider, this.mapper);
        floatElementRepository.Insert(
            elementsList.Values.Where(row => row.FloatElement.HasValue),
            insert => insert.InsertInto(row => row.FloatElementId, row => row.Value)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement));

        var integerElementRepository = new PostgreSqlRepository<FieldValueElement, IntegerElementRow>(provider, this.mapper);
        integerElementRepository.Insert(
            elementsList.Values.Where(row => row.IntegerElement.HasValue),
            insert => insert.InsertInto(row => row.IntegerElementId, row => row.Value)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement));

        var moneyElementRepository = new PostgreSqlRepository<FieldValueElement, MoneyElementRow>(provider, this.mapper);
        moneyElementRepository.Insert(
            elementsList.Values.Where(row => row.MoneyElement.HasValue),
            insert => insert.InsertInto(row => row.MoneyElementId, row => row.Value)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement));

        var textElementRepository = new PostgreSqlRepository<FieldValueElement, TextElementRow>(provider, this.mapper);
        textElementRepository.Insert(
            elementsList.Values.Where(row => row.TextElement != null),
            insert => insert.InsertInto(row => row.TextElementId, row => row.Value)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement));

        // Attach the values to the submission
        var genericValueSubmissions = from v in insertedValues.Values
                                        select new GenericSubmissionValueRow
                                                {
                                                    GenericSubmissionId = submissionId,
                                                    GenericSubmissionValueId = v.FieldValueId
                                                };

        var genericSubmissionValueRepository = new PostgreSqlRepository<FieldValue, GenericSubmissionValueRow>(provider, this.mapper);
        genericSubmissionValueRepository.Insert(genericValueSubmissions, null);
        transaction.Commit();
    }
}
```

Contributions
---
To raise an issue or feature request, please visit the [Issues section](https://github.com/Startitecture/Core/issues) of our [GitHub](https://github.com/Startitecture/Core) repository. For faster resolution, please consider a [pull request](https://github.com/Startitecture/Core/pulls) that includes unit tests.

License
---
The code and API documentation for ST/ORM are licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0).