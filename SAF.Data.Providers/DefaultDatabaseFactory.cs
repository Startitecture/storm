// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Creates database instances using the default constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// Creates database instances using the default constructor.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of database to create.
    /// </typeparam>
    public class DefaultDatabaseFactory<TContext> : IDatabaseFactory<TContext>
        where TContext : Database
    {
        /// <summary>
        /// The default factory.
        /// </summary>
        private static readonly DefaultDatabaseFactory<TContext> DefaultFactory = new DefaultDatabaseFactory<TContext>();

        /// <summary>
        /// Prevents a default instance of the <see cref="T:SAF.Data.Providers.DefaultDatabaseFactory`1"/> class from being created.
        /// </summary>
        private DefaultDatabaseFactory()
        {
        }

        /// <summary>
        /// Gets the default database factory instance.
        /// </summary>
        public static DefaultDatabaseFactory<TContext> Default
        {
            get
            {
                return DefaultFactory;
            }
        }

        /// <summary>
        /// Creates a new database instance.
        /// </summary>
        /// <returns>
        /// A new database instance of the type <typeparamref name="TContext"/>.
        /// </returns>
        public TContext Create()
        {
            return Activator.CreateInstance<TContext>();
        }
    }
}