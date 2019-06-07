// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityDefinition.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// Contains metadata for a data entity.
    /// TODO: Do sorted set by column ordinals
    /// </summary>
    public class EntityDefinition : IEntityDefinition
    {
        /// <summary>
        /// The definition collection.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> allAttributes;

        /// <summary>
        /// The returnable attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> returnableAttributes;

        /// <summary>
        /// The direct attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> directAttributes;

        /// <summary>
        /// The primary key attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> primaryKeyAttributes;

        /// <summary>
        /// The auto number primary key.
        /// </summary>
        private readonly Lazy<EntityAttributeDefinition?> autoNumberPrimaryKey;

        /// <summary>
        /// The updateable attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> updateableAttributes;

        /// <summary>
        /// The entity name.
        /// </summary>
        private readonly Lazy<EntityLocation> entityLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDefinition"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="type">
        /// The type of the entity.
        /// </param>
        public EntityDefinition([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] Type type)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var entityReference = new EntityReference { EntityType = type };
            this.DefinitionProvider = definitionProvider;
            this.allAttributes = new Lazy<List<EntityAttributeDefinition>>(
                () => new List<EntityAttributeDefinition>(this.DefinitionProvider.ResolveDefinitions(type)));

            // Do not include mapped attributes.
            this.returnableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.allAttributes.Value.Where(x => x.IsMetadata == false).ToList);

            // Do not include related attributes.
            this.directAttributes = new Lazy<List<EntityAttributeDefinition>>(this.returnableAttributes.Value.Where(x => x.IsDirect).ToList);
            this.primaryKeyAttributes = new Lazy<List<EntityAttributeDefinition>>(this.directAttributes.Value.Where(x => x.IsPrimaryKey).ToList);
            this.autoNumberPrimaryKey = new Lazy<EntityAttributeDefinition?>(this.GetAutoNumberPrimaryKey);

            // Do not include primary keys. // TODO: Also identify calculated columns.
            this.updateableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.directAttributes.Value.Except(this.primaryKeyAttributes.Value).ToList);

            this.entityLocation = new Lazy<EntityLocation>(() => definitionProvider.GetEntityLocation(entityReference));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDefinition"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        public EntityDefinition([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] EntityReference entityReference)
        {
            if (entityReference == null)
            {
                throw new ArgumentNullException(nameof(entityReference));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.DefinitionProvider = definitionProvider;
            this.allAttributes = new Lazy<List<EntityAttributeDefinition>>(
                () => new List<EntityAttributeDefinition>(this.DefinitionProvider.ResolveDefinitions(entityReference.EntityType)));

            // Do not include mapped attributes.
            this.returnableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.allAttributes.Value.Where(x => x.IsMetadata == false).ToList);

            // Do not include related attributes.
            this.directAttributes = new Lazy<List<EntityAttributeDefinition>>(this.returnableAttributes.Value.Where(x => x.IsDirect).ToList);
            this.primaryKeyAttributes = new Lazy<List<EntityAttributeDefinition>>(this.directAttributes.Value.Where(x => x.IsPrimaryKey).ToList);
            this.autoNumberPrimaryKey = new Lazy<EntityAttributeDefinition?>(this.GetAutoNumberPrimaryKey);

            // Do not include primary keys. // TODO: Also identify calculated columns.
            this.updateableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.directAttributes.Value.Except(this.primaryKeyAttributes.Value).ToList);

            this.entityLocation = new Lazy<EntityLocation>(() => definitionProvider.GetEntityLocation(entityReference));
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> AllAttributes => this.allAttributes.Value;

        /// <summary>
        /// Gets the returnable attributes of the data item.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> ReturnableAttributes => this.returnableAttributes.Value;

        /// <summary>
        /// Gets the direct attributes of the data item.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> DirectAttributes => this.directAttributes.Value;

        /// <summary>
        /// Gets the primary key attributes of the data item.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> PrimaryKeyAttributes => this.primaryKeyAttributes.Value;

        /// <summary>
        /// Gets the auto-number primary key of the data item, if any.
        /// </summary>
        public EntityAttributeDefinition? AutoNumberPrimaryKey => this.autoNumberPrimaryKey.Value;

        /// <inheritdoc />
        /// TODO: Use a more generic representation
        public string QualifiedName => $"[{this.EntityContainer}].[{this.EntityName}]";

        /// <summary>
        /// Gets the updateable attributes of the data item.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> UpdateableAttributes => this.updateableAttributes.Value;

        /// <summary>
        /// Gets the entity container.
        /// </summary>
        public string EntityContainer => this.entityLocation.Value.Container;

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName => this.entityLocation.Value.Name;

        /// <summary>
        /// Gets the definition provider for this definition.
        /// </summary>
        private IEntityDefinitionProvider DefinitionProvider { get; }

        /// <inheritdoc />
        public EntityAttributeDefinition Find(AttributeLocation attributeLocation)
        {
            if (attributeLocation == null)
            {
                throw new ArgumentNullException(nameof(attributeLocation));
            }

            // TODO: Dictionary this!! Also lazy the lookup
            var reference = this.DefinitionProvider.GetEntityReference(attributeLocation.PropertyInfo);
            var location = this.DefinitionProvider.GetEntityLocation(reference);
            var entityName = attributeLocation.EntityReference.EntityAlias ?? location.Alias ?? location.Name;
            
            return this.returnableAttributes.Value.FirstOrDefault(
                definition => (definition.Entity.Alias ?? definition.Entity.Name) == entityName
                              && definition.PropertyName == attributeLocation.PropertyInfo.Name);
        }

        /// <summary>
        /// Finds the first <see cref="EntityAttributeDefinition"/> matching the property name. Direct attributes are queried
        /// first.
        /// </summary>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The first <see cref="EntityAttributeDefinition"/> that matches the entity alias or name and property name, or
        /// <see cref="EntityAttributeDefinition.Empty"/> if the definition is not found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityName"/> or <paramref name="propertyName"/> is null.
        /// </exception>
        public EntityAttributeDefinition Find(string entityName, string propertyName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            // TODO: Dictionary this!!
            return
                this.returnableAttributes.Value.FirstOrDefault(
                    definition => (definition.Entity.Alias ?? definition.Entity.Name) == entityName && definition.PropertyName == propertyName);
        }

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
        public EntityAttributeDefinition Find(LambdaExpression attributeExpression)
        {
            if (attributeExpression == null)
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            var reference = this.DefinitionProvider.GetEntityReference(attributeExpression);
            var location = this.DefinitionProvider.GetEntityLocation(reference);
            var propertyName = attributeExpression.GetPropertyName();

            // TODO: This is iterating more than necessary based on what is probably the real-world usage.
            var attributeDefinition =
                this.allAttributes.Value.FirstOrDefault(
                    x => Singleton<EntityLocationEqualityComparer>.Instance.Equals(x.Entity, location) && x.PropertyName == propertyName);

            return attributeDefinition;
        }

        /// <summary>
        /// Gets the auto number primary key, if any, in the definition.
        /// </summary>
        /// <returns>
        /// The primary key definition, or null if the object does not have a primary key definition.
        /// </returns>
        private EntityAttributeDefinition? GetAutoNumberPrimaryKey()
        {
            var entityAttributeDefinition = this.directAttributes.Value.FirstOrDefault(x => x.IsDirect && x.IsIdentityColumn);
            return entityAttributeDefinition == EntityAttributeDefinition.Empty ? default(EntityAttributeDefinition?) : entityAttributeDefinition;
        }

        /// <summary>
        /// The entity location equality comparer.
        /// </summary>
        private class EntityLocationEqualityComparer : EqualityComparer<EntityLocation>
        {
            /// <summary>
            /// Determines whether two <see cref="EntityLocation"/> values represent the same location.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">
            /// The first object to compare.
            /// </param>
            /// <param name="y">
            /// The second object to compare.
            /// </param>
            public override bool Equals(EntityLocation x, EntityLocation y)
            {
                return Evaluate.Equals(
                    x,
                    y,
                    location => location.EntityType,
                    location => location.Alias,
                    location => location.Name,
                    location => location.Container);
            }

            /// <summary>
            /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms
            /// and data structures, such as a hash table.
            /// </summary>
            /// <returns>A hash code for the specified object.</returns>
            /// <param name="obj">The object for which to get a hash code.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// The type of <paramref name="obj" /> is a reference type and
            /// <paramref name="obj" /> is null.
            /// </exception>
            public override int GetHashCode(EntityLocation obj)
            {
                return Evaluate.GenerateHashCode(obj.EntityType, obj.Alias, obj.Name, obj.Container);
            }
        }
    }
}