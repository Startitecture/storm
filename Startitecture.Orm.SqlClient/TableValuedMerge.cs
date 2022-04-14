// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedMerge.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// A MERGE command that uses a table-valued parameter (TVP) to merge multiple rows of data.
    /// </summary>
    /// <typeparam name="T">
    /// The type of table to use as the source of the merge.
    /// </typeparam>
    public class TableValuedMerge<T> : TransactSqlMergeBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedMerge{T}"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public TableValuedMerge(IDatabaseContext databaseContext)
            : base(Singleton<TableValuedParameterCommandFactory>.Instance, databaseContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedMerge{T}"/> class.
        /// </summary>
        /// <param name="commandFactory">
        /// The command factory to use to create the command.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public TableValuedMerge(IDbTableCommandFactory commandFactory, IDatabaseContext databaseContext)
            : base(commandFactory, databaseContext)
        {
        }

        /// <inheritdoc />
        protected override string DeclareSourceTable(string parameterName, IEnumerable<EntityAttributeDefinition> sourceAttributes)
        {
            return string.Empty; // Already declared in the parameter
        }

        /// <inheritdoc />
        protected override string DeclareInsertedTable(IEnumerable<EntityAttributeDefinition> insertedAttributes)
        {
            if (insertedAttributes == null)
            {
                throw new ArgumentNullException(nameof(insertedAttributes));
            }

            var insertColumns = insertedAttributes.Select(
                definition => $"{this.NameQualifier.Escape(definition.PhysicalName)} {definition.PropertyInfo.GetSqlType()}");

            return $"DECLARE {this.NameQualifier.AddParameterPrefix("inserted")} table({string.Join(", ", insertColumns)});";
        }

        /// <inheritdoc />
        protected override string SourceSelection(
            string parameterName,
            Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes)
        {
            return this.DatabaseContext.RepositoryAdapter.NameQualifier.AddParameterPrefix(parameterName);
        }
    }
}