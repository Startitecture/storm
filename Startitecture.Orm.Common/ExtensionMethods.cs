// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains extension methods for the SAF data namespace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using SAF.StringResources;

    using Startitecture.Core;

    /// <summary>
    /// Contains extension methods for the SAF data namespace.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets a connection for the specified connection name.
        /// </summary>
        /// <param name="connectionStrings">
        /// The connection string collection containing the connection.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// A <see cref="DbConnection"/> for the specified connection string element.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connectionStrings"/> or <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// None of the connection strings contain an element with the specified <paramref name="name"/>, or the connection string did
        /// not specify a provider.
        /// </exception>
        /// <exception cref="Startitecture.Core.OperationException">
        /// The provider specified in the connection string was associated with a factory that created a null connection.
        /// </exception>
        public static DbConnection GetConnection(this ConnectionStringSettingsCollection connectionStrings, string name)
        {
            if (connectionStrings == null)
            {
                throw new ArgumentNullException(nameof(connectionStrings));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var connectionString = connectionStrings[name];

            if (connectionString == null)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringNameNotFound, nameof(name));
            }

            string providerName = connectionString.ProviderName;

            if (providerName == null)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringDidNotSpecifyProvider, nameof(name));
            }

            var providerExists = DbProviderFactories.GetFactoryClasses().Rows.Cast<DataRow>().Any(r => r[2].Equals(providerName));

            if (providerExists == false)
            {
                throw new ArgumentException(ValidationMessages.ConnectionStringProviderDoesNotExist, nameof(name));
            }

            var factory = DbProviderFactories.GetFactory(providerName);
            DbConnection connection = null;

            try
            {
                connection = factory.CreateConnection();

                if (connection == null)
                {
                    throw new OperationException(factory, ValidationMessages.DataConnectionFactoryReturnedNull);
                }

                connection.ConnectionString = connectionString.ConnectionString;
                return connection;
            }
            catch
            {
                connection?.Dispose();
                throw;
            }
        }
    }
}
