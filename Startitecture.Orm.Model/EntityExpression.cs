// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityExpression.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Resources;

    /// <summary>
    /// Represents a table expression.
    /// </summary>
    public class EntityExpression
    {
        /// <summary>
        /// The table relations.
        /// </summary>
        private readonly List<IEntityRelation> tableRelations = new List<IEntityRelation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityExpression"/> class.
        /// </summary>
        /// <param name="tableSelection">
        /// The table selection.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="relations">
        /// The relations between the table expression and the dependent entity selection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tableSelection"/> or <paramref name="relations"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is null or whitespace.
        /// </exception>
        public EntityExpression(
            [NotNull] ISelection tableSelection,
            [NotNull] string tableName,
            [NotNull] IEnumerable<IEntityRelation> relations)
        {
            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(tableName));
            }

            this.TableSelection = tableSelection ?? throw new ArgumentNullException(nameof(tableSelection));
            this.TableName = tableName;
            this.tableRelations.AddRange(relations);
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the table selection.
        /// </summary>
        public ISelection TableSelection { get; }

        /// <summary>
        /// The table relations.
        /// </summary>
        public IEnumerable<IEntityRelation> TableRelations => this.tableRelations;
    }
}