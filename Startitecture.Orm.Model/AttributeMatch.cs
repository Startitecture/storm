namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    public class AttributeMatch<TSource, TRelation>
    {
        public AttributeMatch([NotNull] Expression<Func<TSource, object>> sourceExpression, [NotNull] Expression<Func<TRelation, object>> relationExpression)
        {
            this.SourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.RelationExpression = relationExpression ?? throw new ArgumentNullException(nameof(relationExpression));
        }

        public Expression<Func<TSource, object>> SourceExpression { get; }

        public Expression<Func<TRelation, object>> RelationExpression { get; }

        ////public void Match([NotNull] Expression<Func<TSource, object>> sourceExpression, [NotNull] Expression<Func<TRelation, object>> relationExpression)
        ////{
        ////    this.SourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
        ////    this.RelationExpression = relationExpression ?? throw new ArgumentNullException(nameof(relationExpression));
        ////}
    }
}