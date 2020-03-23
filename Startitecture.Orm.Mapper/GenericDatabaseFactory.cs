// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericDatabaseFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates database instances using the default constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using Startitecture.Orm.Common;

    /// <summary>
    /// Creates database instances using the default constructor.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of database to create.
    /// </typeparam>
    public class GenericDatabaseFactory<TContext> : IDatabaseFactory
        where TContext : IDatabaseContext
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="GenericDatabaseFactory{TContext}"/> class from being created.
        /// </summary>
        private GenericDatabaseFactory()
        {
        }

        /// <summary>
        /// Gets the default database factory instance. TODO: Fix CA1000 on this by putting TContext into method
        /// </summary>
        public static GenericDatabaseFactory<TContext> Default { get; } = new GenericDatabaseFactory<TContext>();

        /// <summary>
        /// Creates a new database instance.
        /// </summary>
        /// <returns>
        /// A new database instance of the type <typeparamref name="TContext"/>.
        /// </returns>
        public IDatabaseContext Create()
        {
            return Activator.CreateInstance<TContext>();
        }
    }
}