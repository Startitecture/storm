// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The selection interface.
    /// </summary>
    public interface ISelection : IEntitySet
    {
        /// <summary>
        /// Gets the selection expressions.
        /// </summary>
        IEnumerable<SelectExpression> SelectExpressions { get; }

        /// <summary>
        /// Gets the order by expressions for the selection.
        /// </summary>
        IEnumerable<OrderExpression> OrderByExpressions { get; }

        /// <summary>
        /// Gets the page options for the selection.
        /// </summary>
        ResultPage Page { get; }
    }
}