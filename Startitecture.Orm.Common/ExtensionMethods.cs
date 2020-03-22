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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// Contains extension methods for the SAF data namespace.
    /// </summary>
    public static class ExtensionMethods
    {
/*
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
*/

        /// <summary>
        /// The not indexed property selector.
        /// </summary>
        private static readonly Func<PropertyInfo, bool> NotIndexedProperty = x => x.GetIndexParameters().Length == 0;

        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="propertyNames">
        /// The names of the properties to collect. If no names are specified, all the entity's properties are returned.
        /// </param>
        /// <returns>
        /// A collection of non-indexed properties for the specified type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// None of the specified property names (case-sensitive) matched properties of the entity type.
        /// </exception>
        public static IEnumerable<PropertyInfo> GetNonIndexedProperties(this Type entityType, params string[] propertyNames)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var nonIndexedProperties = entityType.GetProperties().Where(prop => !prop.GetIndexParameters().Any());

            return propertyNames.Any() ? nonIndexedProperties.Where(x => propertyNames.Contains(x.Name)) : nonIndexedProperties;
        }
    }
}
