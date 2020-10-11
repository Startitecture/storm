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
        /// Maps an <see cref="ISelection"/> to selection of the specified <typeparamref name="TDestEntity"/> type.
        /// </summary>
        /// <typeparam name="TDestEntity">
        /// The type of entity to represent with the selection.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ISelection"/> for the specified type.
        /// </returns>
        EntitySelection<TDestEntity> MapSelection<TDestEntity>()
            where TDestEntity : class, new();
    }
}