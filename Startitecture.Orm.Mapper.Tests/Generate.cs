// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    using Moq;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The generate.
    /// </summary>
    internal class Generate
    {
        /// <summary>
        /// A default integer.
        /// </summary>
        private const int MyInt = 4930;

        /// <summary>
        /// A default nullable integer.
        /// </summary>
        private const int MyNullableInt = 9492;

        /// <summary>
        /// The my short.
        /// </summary>
        private const short MyShort = 838;

        /// <summary>
        /// The my string.
        /// </summary>
        private const string MyString = "MyString";

        /// <summary>
        /// The standard offset.
        /// </summary>
        private static readonly DateTimeOffset MyDateTimeOffset = new DateTimeOffset(2016, 4, 3, 21, 2, 9, TimeSpan.FromHours(4));

        /// <summary>
        /// The my nullable date time offset.
        /// </summary>
        private static readonly DateTimeOffset MyNullableDateTimeOffset = new DateTimeOffset(2016, 3, 13, 7, 23, 29, TimeSpan.FromHours(5));

        /// <summary>
        /// Creates a fake complex row.
        /// </summary>
        /// <returns>
        /// A new <see cref="ComplexFlatRow" /> instance.
        /// </returns>
        public static ComplexFlatRow CreateFakeComplexRow()
        {
            return new ComplexFlatRow
                       {
                           CreatedByDescription = MyString, 
                           CreatedByFakeMultiReferenceEntityId = MyInt, 
                           CreatedByUniqueName = MyString, 
                           UniqueName = MyString, 
                           FakeSubEntityId = MyInt, 
                           FakeSubSubEntityId = MyInt, 
                           ModifiedByUniqueName = MyString, 
                           ModifiedTime = MyDateTimeOffset, 
                           Description = MyString, 
                           FakeSubSubEntityUniqueName = MyString, 
                           FakeEnumerationId = MyInt, 
                           CreationTime = MyDateTimeOffset, 
                           FakeSubEntityUniqueName = MyString, 
                           FakeDependentEntityDependentTimeValue = MyNullableDateTimeOffset, 
                           FakeSubEntityUniqueOtherId = MyShort, 
                           FakeOtherEnumerationId = MyInt, 
                           FakeSubSubEntityDescription = MyString, 
                           FakeDependentEntityId = MyNullableInt, 
                           FakeSubEntityDescription = MyString, 
                           FakeDependentEntityDependentIntegerValue = MyNullableInt, 
                           ModifiedByDescription = MyString, 
                           ModifiedByFakeMultiReferenceEntityId = MyInt, 
                           FakeComplexEntityId = MyInt
                       };
        }

        /// <summary>
        /// Creates a fake complex row.
        /// </summary>
        /// <param name="withDependentEntity">
        /// A value indicating whether to create the row with a dependent entity.
        /// </param>
        /// <returns>
        /// A new <see cref="ComplexFlatRow"/> instance.
        /// </returns>
        public static ComplexRaisedRow CreateFakeRaisedComplexRow(bool withDependentEntity)
        {
            var createdBy = new MultiReferenceRow { Description = MyString, FakeMultiReferenceEntityId = MyInt, UniqueName = MyString };
            var fakeSubSubEntity = new SubSubRow { FakeSubSubEntityId = MyInt, UniqueName = MyString, Description = MyString };
            var fakeSubEntity = new SubRow
                                    {
                                        FakeSubEntityId = MyInt,
                                        FakeSubSubEntityId = MyInt,
                                        SubSubEntity = fakeSubSubEntity,
                                        UniqueName = MyString,
                                        UniqueOtherId = MyShort,
                                        Description = MyString
                                    };

            var modifiedBy = new MultiReferenceRow { FakeMultiReferenceEntityId = MyInt, Description = MyString, UniqueName = MyString };

            var fakeDependentEntity = new DependentRow
                                          {
                                              FakeDependentEntityId = MyInt,
                                              DependentIntegerValue = MyInt,
                                              DependentTimeValue = MyDateTimeOffset
                                          };

            return new ComplexRaisedRow
                       {
                           CreatedBy = createdBy,
                           UniqueName = MyString,
                           FakeSubEntityId = MyInt,
                           SubEntity = fakeSubEntity,
                           ModifiedBy = modifiedBy,
                           DependentEntity = withDependentEntity ? fakeDependentEntity : null,
                           ModifiedTime = MyDateTimeOffset,
                           Description = MyString,
                           FakeEnumerationId = MyInt,
                           CreationTime = MyDateTimeOffset,
                           FakeOtherEnumerationId = MyInt,
                           ComplexEntityId = MyInt
                       };
        }

        /// <summary>
        /// Creates a POCO data request.
        /// </summary>
        /// <param name="item">
        /// The item to use as the source of the property values.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <typeparam name="T">
        /// The type of POCO to create the request for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Startitecture.Orm.Mapper.PocoDataRequest"/> for the specified type.
        /// </returns>
        public static PocoDataRequest CreatePocoDataRequest<T>(T item, IEntityDefinitionProvider definitionProvider)
        {
            var entityDefinition = definitionProvider.Resolve<T>();
            var attributeDefinitions = new ConcurrentDictionary<string, EntityAttributeDefinition>();

            foreach (var attributeDefinition in entityDefinition.ReturnableAttributes)
            {
                // Don't add duplicates in case of multiple paths.
                if (attributeDefinitions.ContainsKey(attributeDefinition.ReferenceName))
                {
                    continue;
                }

                attributeDefinitions.AddOrUpdate(attributeDefinition.ReferenceName, attributeDefinition, (s, column) => attributeDefinition);
            }

            var dataReader = item.MockDataReader(attributeDefinitions, entityDefinition);

            var pocoDataRequest = new PocoDataRequest(dataReader.Object, entityDefinition)
                                      {
                                          ////FieldCount = attributeDefinitions.Count,
                                          FirstColumn = 0
                                      };

            return pocoDataRequest;
        }
    }
}