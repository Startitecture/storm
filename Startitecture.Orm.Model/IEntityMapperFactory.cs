// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMapperFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// Provides an interface for creating <see cref="IEntityMapper"/> instances.
    /// </summary>
    public interface IEntityMapperFactory
    {
        /// <summary>
        /// Creates an <see cref="IEntityMapper"/> instance.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="IEntityMapper" />.
        /// </returns>
        IEntityMapper Create();
    }
}