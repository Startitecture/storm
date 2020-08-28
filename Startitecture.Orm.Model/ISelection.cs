// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelection.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for selecting entities in a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for selecting entities in a repository.
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