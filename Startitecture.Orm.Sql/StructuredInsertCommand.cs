// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredInsertCommand.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The structured insert command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of structure that is the source of the command data.
    /// </typeparam>
    public class StructuredInsertCommand<TStructure> : StructuredSqlCommand<TStructure>
    {
        /// <summary>
        /// The command text.
        /// </summary>
        private readonly Lazy<string> commandText;

        /// <summary>
        /// The match expressions.
        /// </summary>
        private readonly List<EntityAttributeDefinition> selectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The items to insert.
        /// </summary>
        private readonly List<TStructure> items = new List<TStructure>();

        /// <summary>
        /// The item definition.
        /// </summary>
        private IEntityDefinition itemDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredInsertCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public StructuredInsertCommand([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(structuredCommandProvider)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredInsertCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public StructuredInsertCommand([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
            : base(structuredCommandProvider, databaseTransaction)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public override string CommandText => this.commandText.Value;

        /// <summary>
        /// Declares the table to insert into.
        /// </summary>
        /// <param name="insertItems">
        /// The items to insert.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of item to insert the table into.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="StructuredInsertCommand{TStructure}"/>.
        /// </returns>
        public StructuredInsertCommand<TStructure> InsertInto<TDataItem>([NotNull] IEnumerable<TStructure> insertItems)
        {
            if (insertItems == null)
            {
                throw new ArgumentNullException(nameof(insertItems));
            }

            this.itemDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TDataItem>();
            this.items.Clear();
            this.items.AddRange(insertItems);
            return this;
        }

        /// <summary>
        /// Select the results of the insert, using the specified match keys to link the inserted values with the
        /// </summary>
        /// <param name="matchProperties">
        /// The match properties.
        /// </param>
        /// <returns>
        /// The current <see cref="StructuredInsertCommand{TStructure}"/>.
        /// </returns>
        public StructuredInsertCommand<TStructure> SelectResults(params Expression<Func<TStructure, object>>[] matchProperties)
        {
            var structureDefinition = Singleton<DataAnnotationsDefinitionProvider>.Instance.Resolve<TStructure>();
            this.selectionAttributes.Clear();
            this.selectionAttributes.AddRange(matchProperties.Select(x => new AttributeLocation(x)).Select(structureDefinition.Find));
            return this;
        }

        /// <summary>
        /// Executes the the insertion without retrieving inserted values.
        /// </summary>
        public void Execute()
        {
            var dataTableLoader = new DataTableLoader<TStructure>(this.StructuredCommandProvider.EntityDefinitionProvider);

            using (var dataTable = dataTableLoader.Load(this.items))
            {
                this.Execute(dataTable);
            }
        }

        /// <summary>
        /// Executes the structured command and updates identity columns.
        /// </summary>
        /// <param name="rowIdentity">
        /// The row identity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rowIdentity"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{TStructure}"/> of items returned by the command.
        /// </returns>
        public IEnumerable<TStructure> ExecuteWithIdentityUpdate([NotNull] Expression<Func<TStructure, object>> rowIdentity)
        {
            if (rowIdentity == null)
            {
                throw new ArgumentNullException(nameof(rowIdentity));
            }

            var dataTableLoader = new DataTableLoader<TStructure>(this.StructuredCommandProvider.EntityDefinitionProvider);

            using (var dataTable = dataTableLoader.Load(this.items))
            using (var reader = this.ExecuteReader(dataTable))
            {
                while (reader.Read())
                {
                    var identityList = this.items.Where(x => rowIdentity.Compile().Invoke(x) == default).ToList();
                    TStructure itemToRemove = default;

                    // TODO: Build an index based on the selection values instead of this partial looping.
                    foreach (var item in identityList)
                    {
                        bool isMatch = this.selectionAttributes.All(
                            selectionAttribute =>
                                selectionAttribute.GetValueDelegate.DynamicInvoke(item).Equals(reader.GetValue(selectionAttribute.Ordinal)));

                        if (isMatch == false)
                        {
                            continue;
                        }

                        var attributeDefinition = this.itemDefinition.Find(rowIdentity);
                        attributeDefinition.SetValueDelegate.DynamicInvoke(item, reader.GetValue(attributeDefinition.Ordinal));
                        itemToRemove = item;
                        break;
                    }

                    if (Evaluate.IsDefaultValue(itemToRemove))
                    {
                        dynamic rowObject = new ExpandoObject();
                        var rowDictionary = (IDictionary<string, object>)rowObject;

                        foreach (var attribute in this.itemDefinition.DirectAttributes)
                        {
                            rowDictionary.Add(attribute.PropertyName, reader.GetValue(attribute.Ordinal));
                        }

                        // Throw an exception if something has gone wrong.
                        var keyValues = this.selectionAttributes.ToDictionary(definition => definition.PropertyName, definition => reader.GetValue(definition.Ordinal));
                        var keyValuePairs = keyValues.Select(pair => $"{pair.Key}='{pair.Value}'");
                        throw new OperationException(rowObject, $"Could not locate an item matching {string.Join(";", keyValuePairs)}");
                    }

                    // Don't re-enumerate items we've already fixed.
                    identityList.Remove(itemToRemove);
                }
            }

            return this.items;
        }

        /// <summary>
        /// Creates the output for an output table.
        /// </summary>
        /// <param name="structureDefinition">
        /// The structure definition of the table valued parameter.
        /// </param>
        /// <param name="directAttributes">
        /// The direct attributes.
        /// </param>
        /// <param name="commandBuilder">
        /// The command builder.
        /// </param>
        /// <param name="terminateStatement">
        /// A value indicating whether to terminate the statement.
        /// </param>
        private static void CreateOutput(
            IEntityDefinition structureDefinition,
            IReadOnlyCollection<EntityAttributeDefinition> directAttributes,
            StringBuilder commandBuilder,
            bool terminateStatement)
        {
            var outputAttributes = from tvpAttribute in structureDefinition.AllAttributes
                                   join directAttribute in directAttributes on tvpAttribute.AbsoluteLocation equals directAttribute.AbsoluteLocation
                                   orderby tvpAttribute.Ordinal
                                   select tvpAttribute;

            var statementTerminator = terminateStatement ? ";" : string.Empty;

            var insertedColumns = outputAttributes.OrderBy(x => x.Ordinal).Select(x => x.PropertyName).ToList();
            var outputColumns = directAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{x.PhysicalName}");
            commandBuilder.AppendLine($"OUTPUT {string.Join(",", outputColumns)}")
                .AppendLine($"INTO @inserted ({string.Join(", ", insertedColumns)}){statementTerminator}");
        }

        /// <summary>
        /// The compile command text.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        private string CompileCommandText()
        {
            var structureDefinition = Singleton<DataAnnotationsDefinitionProvider>.Instance.Resolve<TStructure>();
            var directAttributes = this.itemDefinition.DirectAttributes.ToList();
            var insertAttributes = this.itemDefinition.DirectAttributes.Any(x => x.IsIdentityColumn)
                                       ? this.itemDefinition.UpdateableAttributes.ToList()
                                       : directAttributes;

            var targetColumns = insertAttributes.OrderBy(x => x.PhysicalName).Select(x => x.PhysicalName);

            var sourceAttributes = (from tvpAttribute in structureDefinition.AllAttributes
                                    join insertAttribute in insertAttributes on tvpAttribute.PhysicalName equals insertAttribute.PhysicalName
                                    orderby tvpAttribute.PhysicalName
                                    select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => x.PhysicalName);

            var commandBuilder = new StringBuilder();

            var selectResults = this.selectionAttributes.Any();

            if (selectResults)
            {
                commandBuilder.AppendLine($"DECLARE @inserted {this.StructureTypeName};");
            }

            commandBuilder.AppendLine($"INSERT INTO [{this.itemDefinition.EntityContainer}].[{this.itemDefinition.EntityName}]")
                .AppendLine($"({string.Join(", ", targetColumns)})");

            if (selectResults)
            {
                // In an INSERT we do not terminate this statement with a semi-colon. However in a MERGE we would do that.
                CreateOutput(structureDefinition, directAttributes, commandBuilder, false);
            }

            commandBuilder.AppendLine($"SELECT {string.Join(", ", sourceColumns)} FROM {this.Parameter} AS tvp;");

            if (selectResults)
            {
                this.SelectOutput(structureDefinition, commandBuilder);
            }

            var compileCommandText = commandBuilder.ToString();
            return compileCommandText;
        }

        /// <summary>
        /// Selects the output from an inserted table variable.
        /// </summary>
        /// <param name="structureDefinition">
        /// The structure definition of the table valued parameter.
        /// </param>
        /// <param name="commandBuilder">
        /// The command builder to update.
        /// </param>
        private void SelectOutput(IEntityDefinition structureDefinition, StringBuilder commandBuilder)
        {
            var keyAttributes = (from key in this.itemDefinition.AllAttributes.Where(x => x.IsPrimaryKey)
                                 join fk in structureDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                 select new { TargetKey = key, SourceKey = fk }).ToList();

            var matchAttributes = (from key in this.selectionAttributes
                                   join fk in structureDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                   select new { TargetKey = key, SourceKey = fk }).ToList();

            var selectionJoinMatchColumns =
                (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                    x => $"i.[{x.SourceKey.PropertyName}] = tvp.[{x.SourceKey.PropertyName}]");

            // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
            // table valued parameter.
            var selectedKeyAttributes =
                keyAttributes.Select(x => new { Column = $"i.[{x.SourceKey.PropertyName}]", Attribute = x.SourceKey }).ToList();

            // Everything for selecting from the TVP uses property name in order to match UDTT columns.
            var nonKeyAttributes = structureDefinition.AllAttributes.Except(selectedKeyAttributes.Select(x => x.Attribute))
                .Select(
                    x => new
                             {
                                 Column = $"tvp.[{x.PropertyName}]",
                                 Attribute = x
                             });

            var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.PropertyName).Select(x => x.Column);

            commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                .AppendLine("FROM @inserted AS i")
                .AppendLine($"INNER JOIN {this.Parameter} AS tvp")
                .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
        }
    }
}