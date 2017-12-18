// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityDefinitionProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the IEntityDefinitionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface to classes that define the definitions of entity types.
    /// </summary>
    public interface IEntityDefinitionProvider
    {
        /// <summary>
        /// Resolves the entity definition for the specified type.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the item to resolve.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEntityDefinition"/> for the specified type.
        /// </returns>
        IEntityDefinition Resolve<TItem>();

        /// <summary>
        /// Resolves the entity definition for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to resolve.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityDefinition"/> for the specified type.
        /// </returns>
        IEntityDefinition Resolve(Type type);

        /// <summary>
        /// Gets the entity location for the specified type.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference to retrieve the location for.
        /// </param>
        /// <returns>
        /// An <see cref="EntityLocation"/> instance with the location of the entity.
        /// </returns>
        EntityLocation GetEntityLocation(EntityReference entityReference);

        /// <summary>
        /// Returns a collection of <see cref="EntityAttributeDefinition"/> elements for the specified entity type.
        /// </summary>
        /// <param name="entityType">
        /// The type of the entity to resolve definitions for.
        /// </param>
        /// <returns>
        /// A collection of <see cref="EntityAttributeDefinition"/> based on the specified <paramref name="entityType"/>.
        /// </returns>
        IEnumerable<EntityAttributeDefinition> ResolveDefinitions(Type entityType);
    }
}
