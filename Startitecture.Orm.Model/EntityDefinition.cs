// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityDefinition.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
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
    /// TODO: Do sorted set by column ordinals.
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
        private readonly Lazy<EntityAttributeDefinition?> rowIdentity;

        /// <summary>
        /// The insertable attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> insertableAttributes;

        /// <summary>
        /// The updateable attributes.
        /// </summary>
        private readonly Lazy<List<EntityAttributeDefinition>> updateableAttributes;

        /// <summary>
        /// The entity name.
        /// </summary>
        private readonly Lazy<EntityLocation> entityLocation;

        /// <summary>
        /// The default relations.
        /// </summary>
        private readonly Lazy<List<IEntityRelation>> defaultRelations;

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

            this.DefinitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.allAttributes = new Lazy<List<EntityAttributeDefinition>>(
                () => new List<EntityAttributeDefinition>(this.DefinitionProvider.ResolveDefinitions(entityReference.EntityType)));

            // Do not include mapped attributes.
            this.returnableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.allAttributes.Value.Where(x => x.IsMetadata == false).ToList);

            // Do not include related attributes.
            this.directAttributes = new Lazy<List<EntityAttributeDefinition>>(this.returnableAttributes.Value.Where(x => x.IsDirect).ToList);
            this.primaryKeyAttributes = new Lazy<List<EntityAttributeDefinition>>(this.directAttributes.Value.Where(x => x.IsPrimaryKey).ToList);
            this.rowIdentity = new Lazy<EntityAttributeDefinition?>(this.GetRowIdentity);

            // Do not include identity columns.
            this.insertableAttributes = new Lazy<List<EntityAttributeDefinition>>(
                this.directAttributes.Value.Where(definition => definition.IsIdentityColumn == false && definition.IsComputed == false).ToList);

            // Do not include primary keys.
            this.updateableAttributes =
                new Lazy<List<EntityAttributeDefinition>>(this.insertableAttributes.Value.Except(this.primaryKeyAttributes.Value).ToList);

            this.entityLocation = new Lazy<EntityLocation>(() => definitionProvider.GetEntityLocation(entityReference));

            var type = entityReference.EntityType;
            this.defaultRelations = new Lazy<List<IEntityRelation>>(
                () =>
                    {
                        if (typeof(IEntityAggregate).IsAssignableFrom(type) && type.GetConstructor(Array.Empty<Type>()) != null)
                        {
                            // We have to make an instance to get the relations.
                            // TODO: Use T4 template in conjunction with property declarations to build this without instantiation
                            var entity = Activator.CreateInstance(type);
                            return ((IEntityAggregate)entity).EntityRelations.ToList();
                        }

                        return new List<IEntityRelation>();
                    });
        }

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> AllAttributes => this.allAttributes.Value;

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> ReturnableAttributes => this.returnableAttributes.Value;

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> DirectAttributes => this.directAttributes.Value;

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> InsertableAttributes => this.insertableAttributes.Value;

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> PrimaryKeyAttributes => this.primaryKeyAttributes.Value;

        /// <inheritdoc />
        public EntityAttributeDefinition? RowIdentity => this.rowIdentity.Value;

        /// <inheritdoc />
        public string QualifiedName => $"{this.EntityContainer}.{this.EntityName}";

        /// <inheritdoc />
        public IEnumerable<IEntityRelation> DefaultRelations => this.defaultRelations.Value;

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> UpdateableAttributes => this.updateableAttributes.Value;

        /// <inheritdoc />
        public string EntityContainer => this.entityLocation.Value.Container;

        /// <inheritdoc />
        public string EntityName => this.entityLocation.Value.Name;

        /// <summary>
        /// Gets the definition provider for this definition.
        /// </summary>
        private IEntityDefinitionProvider DefinitionProvider { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.QualifiedName;
        }

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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                    x => EntityLocationEqualityComparer.TypeAndName.Equals(x.Entity, location) && x.PropertyName == propertyName);

            return attributeDefinition;
        }

        /// <inheritdoc />
        public bool IsUpdateable(EntityAttributeDefinition attributeDefinition)
        {
            return attributeDefinition.IsDirect && attributeDefinition.IsIdentityColumn == false && attributeDefinition.IsPrimaryKey == false
                   && attributeDefinition.IsComputed == false;
        }

        /// <summary>
        /// Gets the auto number primary key, if any, in the definition.
        /// </summary>
        /// <returns>
        /// The primary key definition, or null if the object does not have a primary key definition.
        /// </returns>
        private EntityAttributeDefinition? GetRowIdentity()
        {
            var entityAttributeDefinition = this.directAttributes.Value.FirstOrDefault(x => x.IsDirect && x.IsIdentityColumn);

            if (entityAttributeDefinition == default)
            {
                return null;
            }

            return entityAttributeDefinition;
        }

        /// <summary>
        /// The entity location equality comparer.
        /// </summary>
        private class EntityLocationEqualityComparer : EqualityComparer<EntityLocation>
        {
            /// <summary>
            /// Gets an equality comparer that bases equality on the entity location type, container, name and alias.
            /// </summary>
            public static EqualityComparer<EntityLocation> TypeAndName { get; } = new EntityLocationEqualityComparer();

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
            /// <exception cref="ArgumentNullException">
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