// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityDefinitionProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the IEntityDefinitionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

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

        /// <summary>
        /// Gets an entity reference from the specified <paramref name="attributeExpression"/>.
        /// </summary>
        /// <param name="attributeExpression">
        /// The attribute expression.
        /// </param>
        /// <returns>
        /// An <see cref="EntityReference"/> based on the <paramref name="attributeExpression"/>.
        /// </returns>
        EntityReference GetEntityReference(LambdaExpression attributeExpression);

        /// <summary>
        /// Gets an entity reference from the specified <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// An <see cref="EntityReference"/> based on the <paramref name="propertyInfo"/>.
        /// </returns>
        EntityReference GetEntityReference(PropertyInfo propertyInfo);
    }
}
