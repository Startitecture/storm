// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using Startitecture.Orm.Common;

    /// <summary>
    /// The SQL Server adapter factory.
    /// </summary>
    public class SqlServerAdapterFactory : IRepositoryAdapterFactory
    {
        /// <inheritdoc />
        public IRepositoryAdapter Create(IDatabaseContext dataContext)
        {
            return new SqlServerAdapter(dataContext);
        }
    }
}