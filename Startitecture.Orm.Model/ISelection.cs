namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface ISelection
    {
        /// <summary>
        /// Gets the entity relations represented in the selection.
        /// </summary>
        IEnumerable<IEntityRelation> Relations { get; }

        /// <summary>
        /// Gets the property values for the selection filter.
        /// </summary>
        IEnumerable<object> PropertyValues { get; }

        /// <summary>
        /// Gets the selection expressions.
        /// </summary>
        IEnumerable<SelectExpression> SelectExpressions { get; }

        /// <summary>
        /// Gets the order by expressions for the selection.
        /// </summary>
        IEnumerable<OrderExpression> OrderByExpressions { get; }

        /// <summary>
        /// Gets the filters for the selection.
        /// </summary>
        IEnumerable<ValueFilter> Filters { get; }

        /// <summary>
        /// Gets the page options for the selection.
        /// </summary>
        ResultPage Page { get; }

        /// <summary>
        /// Gets the child selection, if any, for the selection.
        /// </summary>
        LinkedSelection LinkedSelection { get; }
    }
}