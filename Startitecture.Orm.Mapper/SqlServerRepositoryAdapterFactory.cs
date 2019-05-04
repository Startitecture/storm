// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerRepositoryAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The query repository adapter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using Common;

    /// <summary>
    /// The query repository adapter factory.
    /// </summary>
    public class SqlServerRepositoryAdapterFactory : IRepositoryAdapterFactory
    {
        #region Public Methods and Operators

        /// <inheritdoc />
        public IRepositoryAdapter Create(IDatabaseContext dataContext)
        {
            return new SqlServerRepositoryAdapter(dataContext);
        }

        #endregion
    }
}