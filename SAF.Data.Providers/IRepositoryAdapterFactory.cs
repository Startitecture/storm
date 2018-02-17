// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for creating IRepositoryAdapter instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    /// <summary>
    /// Provides an interface for creating <see cref="IRepositoryAdapter"/> instances.
    /// </summary>
    public interface IRepositoryAdapterFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a new repository adapter.
        /// </summary>
        /// <param name="dataContext">
        /// The data context.
        /// </param>
        /// <returns>
        /// An <see cref="IRepositoryAdapter"/> for the specified <paramref name="dataContext"/>.
        /// </returns>
        IRepositoryAdapter Create(Database dataContext);

        #endregion
    }
}