namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    public class AttributeMatchSet<TSource, TRelation>
    {
        private readonly List<AttributeMatch<TSource, TRelation>> matches = new List<AttributeMatch<TSource, TRelation>>();

        public IEnumerable<AttributeMatch<TSource, TRelation>> Matches => this.matches;

        public AttributeMatchSet<TSource, TRelation> On(
            [NotNull] Expression<Func<TSource, object>> sourceExpression,
            [NotNull] Expression<Func<TRelation, object>> relationExpression)
        {
            if (sourceExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceExpression));
            }

            if (relationExpression == null)
            {
                throw new ArgumentNullException(nameof(relationExpression));
            }

            this.matches.Add(new AttributeMatch<TSource, TRelation>(sourceExpression, relationExpression));
            return this;
        }
    }
}