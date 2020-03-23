// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mappers.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A static class for managing registration of IMapper instances with PetaPoco.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Startitecture.Core;

    /// <summary>
    /// A static class for managing registration of IMapper instances.
    /// </summary>
    public static class Mappers
    {
        #region Static Fields

        /// <summary>
        /// The lock.
        /// </summary>
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        /// <summary>
        /// The mappers dictionary.
        /// </summary>
        private static readonly Dictionary<object, IMapper> MappersDictionary = new Dictionary<object, IMapper>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieve the <see cref="IMapper"/> implementation to be used for a specified type.
        /// </summary>
        /// <param name="type">
        /// The type to get a mapper for.
        /// </param>
        /// <returns>
        /// An <see cref="IMapper"/> for the specified type.
        /// </returns>
        public static IMapper GetMapper(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Lock.EnterReadLock();

            try
            {
                if (MappersDictionary.TryGetValue(type, out var val))
                {
                    return val;
                }

                if (MappersDictionary.TryGetValue(type.Assembly, out val))
                {
                    return val;
                }

                return Singleton<StandardMapper>.Instance;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Registers a mapper for all types in a specific assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly whose types are to be managed by this mapper.
        /// </param>
        /// <param name="mapper">
        /// The <see cref="IMapper"/> implementation to register.
        /// </param>
        public static void Register(Assembly assembly, IMapper mapper)
        {
            RegisterInternal(assembly, mapper);
        }

        /// <summary>
        /// Registers a mapper for a single POCO type.
        /// </summary>
        /// <param name="type">
        /// The type to be managed by this mapper.
        /// </param>
        /// <param name="mapper">
        /// The <see cref="IMapper"/> implementation to register.
        /// </param>
        public static void Register(Type type, IMapper mapper)
        {
            RegisterInternal(type, mapper);
        }

        /// <summary>
        /// Remove all mappers for all types in a specific assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly whose mappers are to be revoked.
        /// </param>
        public static void Revoke(Assembly assembly)
        {
            RevokeInternal(assembly);
        }

        /// <summary>
        /// Remove the mapper for a specific type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapper is to be removed.
        /// </param>
        public static void Revoke(Type type)
        {
            RevokeInternal(type);
        }

        /// <summary>
        /// Revoke an instance of a mapper.
        /// </summary>
        /// <param name="mapper">
        /// The <see cref="IMapper"/> instance to revoke.
        /// </param>
        public static void Revoke(IMapper mapper)
        {
            Lock.EnterWriteLock();
            try
            {
                foreach (var i in MappersDictionary.Where(kvp => kvp.Value == mapper).ToList())
                {
                    MappersDictionary.Remove(i.Key);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
                ////FlushCaches();
            }
        }

        #endregion

        #region Methods

        /////// <summary>
        /////// The flush caches.
        /////// </summary>
        ////private static void FlushCaches()
        ////{
        ////    // Whenever a mapper is registered or revoked, we have to assume any generated code is no longer valid.
        ////    // Since this should be a rare occurance, the simplest approach is to simply dump everything and start over.
        ////    MultiPocoFactory.FlushCaches();
        ////    PocoDataFactory.FlushCaches();
        ////}

        /// <summary>
        /// Registers a mapper with a type or assembly.
        /// </summary>
        /// <param name="typeOrAssembly">
        /// The type or assembly.
        /// </param>
        /// <param name="mapper">
        /// The <see cref="IMapper"/> implementation to register.
        /// </param>
        private static void RegisterInternal(object typeOrAssembly, IMapper mapper)
        {
            Lock.EnterWriteLock();

            try
            {
                MappersDictionary.Add(typeOrAssembly, mapper);
            }
            finally
            {
                Lock.ExitWriteLock();
                ////FlushCaches();
            }
        }

        /// <summary>
        /// Revoke an instance of a mapper.
        /// </summary>
        /// <param name="typeOrAssembly">
        /// The type or assembly to revoke.
        /// </param>
        private static void RevokeInternal(object typeOrAssembly)
        {
            Lock.EnterWriteLock();

            try
            {
                MappersDictionary.Remove(typeOrAssembly);
            }
            finally
            {
                Lock.ExitWriteLock();
                ////FlushCaches();
            }
        }

        #endregion
    }
}