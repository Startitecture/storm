// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryRepositoryAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The query repository adapter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using Startitecture.Orm.Common;

    /// <summary>
    /// The query repository adapter factory.
    /// </summary>
    public class QueryRepositoryAdapterFactory : IRepositoryAdapterFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dataContext">
        /// The data context.
        /// </param>
        /// <returns>
        /// The <see cref="IRepositoryAdapter"/>.
        /// </returns>
        public IRepositoryAdapter Create(IDatabaseContext dataContext)
        {
            return new QueryRepositoryAdapter(dataContext);
        }

        #endregion
    }
}