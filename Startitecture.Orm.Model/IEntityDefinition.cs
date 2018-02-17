// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityDefinition.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that define entities and their attributes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides an interface for classes that define entities and their attributes.
    /// </summary>
    public interface IEntityDefinition
    {
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        IEnumerable<EntityAttributeDefinition> AllAttributes { get; }

        /// <summary>
        /// Gets the returnable attributes of the data item.
        /// </summary>
        IEnumerable<EntityAttributeDefinition> ReturnableAttributes { get; }

        /// <summary>
        /// Gets the direct attributes of the data item.
        /// </summary>
        IEnumerable<EntityAttributeDefinition> DirectAttributes { get; }

        /// <summary>
        /// Gets the updateable attributes of the data item.
        /// </summary>
        IEnumerable<EntityAttributeDefinition> UpdateableAttributes { get; }

        /// <summary>
        /// Gets the entity container.
        /// </summary>
        string EntityContainer { get; }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// Gets the primary key attributes of the data item.
        /// </summary>
        IEnumerable<EntityAttributeDefinition> PrimaryKeyAttributes { get; }

        /// <summary>
        /// Gets the auto-number primary key of the data item, if any.
        /// </summary>
        EntityAttributeDefinition? AutoNumberPrimaryKey { get; }

        /// <summary>
        /// Finds the first <see cref="EntityAttributeDefinition"/> matching the property name.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="EntityAttributeDefinition"/> that matches the property name, or 
        /// <see cref="EntityAttributeDefinition.Empty"/> if the definition is not found.
        /// </returns>
        EntityAttributeDefinition Find([NotNull] string propertyName);

        /// <summary>
        /// Finds the first <see cref="EntityAttributeDefinition"/> matching the property name. Direct attributes are queried first.
        /// </summary>
        /// <param name="entityName">
        /// The entity Name.
        /// </param>
        /// <param name="propertyName">
        /// The property Name.
        /// </param>
        /// <returns>
        /// The first <see cref="EntityAttributeDefinition"/> that matches the entity alias or name and property name, or 
        /// <see cref="EntityAttributeDefinition.Empty"/> if the definition is not found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityName"/> or <paramref name="propertyName"/> is null.
        /// </exception>
        EntityAttributeDefinition Find([NotNull] string entityName, [NotNull] string propertyName);

        /// <summary>
        /// Finds the first <see cref="EntityAttributeDefinition"/> matching the attribute expression
        /// </summary>
        /// <param name="attributeExpression">
        /// The attribute expression to find.
        /// </param>
        /// <returns>
        /// The first <see cref="EntityAttributeDefinition"/> that matches the property name, or
        /// <see cref="EntityAttributeDefinition.Empty"/> if the definition is not found.
        /// </returns>
        /// <remarks>
        /// If the declaring type of <paramref name="attributeExpression"/> is not the same as the type of the entity definition, the
        /// attribute may not be found.
        /// </remarks>
        EntityAttributeDefinition Find([NotNull] LambdaExpression attributeExpression);
    }
}