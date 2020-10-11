// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationExpression.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Expresses an entity set that is related to another entity set or selection.
    /// </summary>
    public class RelationExpression : IValueFilter
    {
        /// <summary>
        /// The table relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationExpression"/> class.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to relate.
        /// </param>
        /// <param name="relations">
        /// The relations between the entity set and the other selection or set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entitySet"/> or <paramref name="relations"/> is null.
        /// </exception>
        public RelationExpression([NotNull] IEntitySet entitySet, [NotNull] IEnumerable<IEntityRelation> relations)
        {
            this.EntitySet = entitySet ?? throw new ArgumentNullException(nameof(entitySet));

            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            var entityRelations = relations.ToList();

            if (entityRelations.Any() == false)
            {
                throw new ArgumentException($"A {nameof(RelationExpression)} must have at least one entity relation.", nameof(relations));
            }

            this.relations.AddRange(entityRelations);
            this.AttributeLocation = new AttributeLocation(this.relations.First().SourceExpression);
            this.FilterType = FilterType.MatchesSubQuery;
            this.FilterValues = entitySet.PropertyValues;
        }

        /// <summary>
        /// Gets the entity set in the expression.
        /// </summary>
        public IEntitySet EntitySet { get; }

        /// <summary>
        /// Gets the table relations between the expression and the other set or selection.
        /// </summary>
        public IEnumerable<IEntityRelation> Relations => this.relations;

        /// <inheritdoc />
        public AttributeLocation AttributeLocation { get; }

        /// <inheritdoc />
        public FilterType FilterType { get; }

        /// <inheritdoc />
        public IEnumerable<object> FilterValues { get; }
    }
}