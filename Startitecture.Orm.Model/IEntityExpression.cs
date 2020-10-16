// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityExpression.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for expressions related to other selections.
    /// </summary>
    public interface IEntityExpression
    {
        /// <summary>
        /// Gets the expression name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the entity selection in the expression.
        /// </summary>
        ISelection Expression { get; }

        /// <summary>
        /// Gets the table relations between the expression and the other set or selection.
        /// </summary>
        IEnumerable<IEntityRelation> Relations { get; }
    }
}