// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using Startitecture.Orm.Common;

    /// <summary>
    /// The SQL Server adapter factory.
    /// </summary>
    public class SqlClientAdapterFactory : IRepositoryAdapterFactory
    {
        /// <inheritdoc />
        public IRepositoryAdapter Create(IDatabaseContext dataContext)
        {
            return new SqlClientAdapter(dataContext);
        }
    }
}