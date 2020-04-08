// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAttributeDefinition.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the definition of an entity attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// Contains the definition of an entity attribute.
    /// </summary>
    public struct EntityAttributeDefinition : IEquatable<EntityAttributeDefinition>, IComparable, IComparable<EntityAttributeDefinition>
    {
        /// <summary>
        /// Represents an empty entity attribute.
        /// </summary>
        public static readonly EntityAttributeDefinition Empty = new EntityAttributeDefinition();

        /// <summary>
        /// The attribute name format.
        /// </summary>
        private const string AttributeNameFormat = "{0}/{1}";

        /// <summary>
        /// The attribute name format.
        /// </summary>
        private const string AliasedNameFormat = "{0}/{1}:({2})";

        /// <summary>
        /// The delegate cache.
        /// </summary>
        private static readonly MemoryCache<string, Delegate> DelegateMemoryCache  = new MemoryCache<string, Delegate>();

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EntityAttributeDefinition, object>[] ComparisonProperties =
            {
                item => item.Entity, 
                item => item.ReferenceNode?.Value,
                item => item.Ordinal,
                item => item.PropertyName,
                item => item.PhysicalName, 
                item => item.Alias,
                item => item.AttributeTypes
            };

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly Lazy<int> hashCode;

        /// <summary>
        /// The entity path.
        /// </summary>
        private readonly LinkedList<EntityLocation> entityPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAttributeDefinition"/> struct.
        /// </summary>
        /// <param name="entityPath">
        /// The entity Path.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info for the attribute.
        /// </param>
        /// <param name="physicalName">
        /// The name of the attribute.
        /// </param>
        /// <param name="attributeTypes">
        /// The type of the attribute.
        /// </param>
        /// <param name="ordinal">
        /// The attribute ordinal on the entity.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityPath"/>, <paramref name="propertyInfo"/>, or <paramref name="physicalName"/> is null or empty or whitespace.
        /// </exception>
        public EntityAttributeDefinition(
            [NotNull] LinkedList<EntityLocation> entityPath,
            [NotNull] PropertyInfo propertyInfo,
            [NotNull] string physicalName,
            EntityAttributeTypes attributeTypes,
            int ordinal)
            : this(entityPath, propertyInfo, physicalName, attributeTypes, ordinal, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAttributeDefinition"/> struct.
        /// </summary>
        /// <param name="entityPath">
        /// The entity Path.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info for the attribute.
        /// </param>
        /// <param name="physicalName">
        /// The name of the attribute.
        /// </param>
        /// <param name="attributeTypes">
        /// The type of the attribute.
        /// </param>
        /// <param name="ordinal">
        /// The attribute ordinal on the entity.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityPath"/>, <paramref name="propertyInfo"/>, or <paramref name="physicalName"/> is null or empty or whitespace.
        /// </exception>
        public EntityAttributeDefinition(
            [NotNull] LinkedList<EntityLocation> entityPath,
            [NotNull] PropertyInfo propertyInfo,
            [NotNull] string physicalName,
            EntityAttributeTypes attributeTypes,
            int ordinal,
            string alias)
            : this()
        {
            if (entityPath == null)
            {
                throw new ArgumentNullException(nameof(entityPath));
            }

            if (string.IsNullOrWhiteSpace(physicalName))
            {
                throw new ArgumentNullException(nameof(physicalName));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            // Clone the path in case it changes.
            this.entityPath = new LinkedList<EntityLocation>(entityPath);

            this.PropertyName = propertyInfo.Name;
            this.PhysicalName = physicalName;
            this.AttributeTypes = attributeTypes;
            this.Ordinal = ordinal;

            // If the alias matches the physical name then it isn't an alias.
            this.Alias = this.PhysicalName == alias ? null : alias;

            this.IsDirect = this.AttributeTypes.HasFlag(EntityAttributeTypes.DirectAttribute);
            this.IsPrimaryKey = this.AttributeTypes.HasFlag(EntityAttributeTypes.PrimaryKey);
            this.IsIdentityColumn = this.AttributeTypes.HasFlag(EntityAttributeTypes.IdentityColumn);
            this.IsReferencedDirect = this.IsDirect || this.AttributeTypes.HasFlag(EntityAttributeTypes.ExplicitRelatedAttribute);
            this.IsMetadata = this.AttributeTypes.HasFlag(EntityAttributeTypes.Relation)
                              || this.AttributeTypes.HasFlag(EntityAttributeTypes.MappedAttribute);

            this.AbsoluteLocation = new AttributeLocation(propertyInfo, new EntityReference { EntityType = this.Entity.EntityType });

            // TODO: Cache methods/delegates per type/property.
            this.PropertyInfo = propertyInfo;
            var getMethodInfo = propertyInfo.GetGetMethod(true);
            this.GetValueMethod = getMethodInfo;
            this.GetValueDelegate = DelegateMemoryCache.Get(
                $"{nameof(getMethodInfo)}.{propertyInfo.DeclaringType}.{propertyInfo.Name}",
                () => CreateFunctionDelegate(propertyInfo, getMethodInfo));

            var setMethodInfo = propertyInfo.GetSetMethod(true);
            this.SetValueMethod = setMethodInfo ?? throw new InvalidOperationException(
                    $"The property '{propertyInfo.PropertyType.Name}.{propertyInfo.Name}' requires a set method for this delegate to be built.");

            this.SetValueDelegate = DelegateMemoryCache.Get(
                $"{nameof(setMethodInfo)}.{propertyInfo.DeclaringType}.{propertyInfo.Name}",
                () => CreateActionDelegate(propertyInfo, setMethodInfo));

            this.hashCode = new Lazy<int>(this.CreateHashCode);
        }

        /// <summary>
        /// Gets the entity that contains this attribute.
        /// </summary>
        public EntityLocation Entity => this.EntityNode?.Value ?? default;

        /// <summary>
        /// Gets the reference node that this attribute is located on in the object graph.
        /// </summary>
        public LinkedListNode<EntityLocation> ReferenceNode
        {
            get
            {
                var referenceNode = this.entityPath?.Last;

                if (referenceNode == null)
                {
                    return null;
                }

                // Work backwards to the first non-virtual node. Because it's nullable we have to evaluate against true explicitly.
                // NOTE: Here we are assuming that only the last node will be virtual, based on DataItemDefinitionProvider behavior.
                while (referenceNode?.Value.IsVirtual == true)
                {
                    referenceNode = referenceNode.Previous;
                }

                return referenceNode;
            }
        }

        /// <summary>
        /// Gets the entity node that contains this attribute.
        /// </summary>
        public LinkedListNode<EntityLocation> EntityNode => this.entityPath?.Last;

        /// <summary>
        /// Gets the name of the entity attribute.
        /// </summary>
        public string PhysicalName { get; }

        /// <summary>
        /// Gets the absolute location or the entity attribute without aliases.
        /// </summary>
        public AttributeLocation AbsoluteLocation { get; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the attribute alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the ordinal of the entity attribute.
        /// </summary>
        public int Ordinal { get; }

        /// <summary>
        /// Gets the name by which the column will referenced in the current instance.
        /// </summary>
        public string ReferenceName => this.Alias ?? this.PhysicalName;

        /// <summary>
        /// Gets the entity attribute type.
        /// </summary>
        public EntityAttributeTypes AttributeTypes { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute is direct on the <see cref="Entity"/>.
        /// </summary>
        public bool IsDirect { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute is an identity column.
        /// </summary>
        public bool IsIdentityColumn { get; }

        /// <summary>
        /// Gets a value indicating whether the attribute is a direct reference (property) on the POCO.
        /// </summary>
        public bool IsReferencedDirect { get; }

        /// <summary>
        /// Gets a value indicating whether the definition is metadata for the entity definition.
        /// </summary>
        public bool IsMetadata { get; }

        /// <summary>
        /// Gets the property info for the attribute.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the get value method for the property.
        /// </summary>
        public MethodInfo GetValueMethod { get; }

        /// <summary>
        /// Gets the get value delegate.
        /// </summary>
        public Delegate GetValueDelegate { get; }

        /// <summary>
        /// Gets the set value method for the property.
        /// </summary>
        public MethodInfo SetValueMethod { get; }

        /// <summary>
        /// Gets the set value delegate.
        /// </summary>
        public Delegate SetValueDelegate { get; }

        /// <summary>
        /// Compares the equality of two values of the same type.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EntityAttributeDefinition valueA, EntityAttributeDefinition valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Compares the inequality of two values of the same type.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(EntityAttributeDefinition valueA, EntityAttributeDefinition valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(EntityAttributeDefinition valueA, EntityAttributeDefinition valueB)
        {
            return Comparer<EntityAttributeDefinition>.Default.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(EntityAttributeDefinition valueA, EntityAttributeDefinition valueB)
        {
            return Comparer<EntityAttributeDefinition>.Default.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance
        /// occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows
        /// <paramref name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This
        /// object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(EntityAttributeDefinition other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Alias == null
                       ? string.Format(CultureInfo.CurrentCulture, AttributeNameFormat, this.Entity, this.PropertyName)
                       : string.Format(CultureInfo.CurrentCulture, AliasedNameFormat, this.Entity, this.PropertyName, this.Alias);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.hashCode.Value;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// Another object to compare to.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(EntityAttributeDefinition other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Creates a function delegate to get a property value.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="getMethodInfo">
        /// The get method info.
        /// </param>
        /// <returns>
        /// A <see cref="System.Delegate"/> that gets the value of the property for any object of the declaring type.
        /// </returns>
        private static Delegate CreateFunctionDelegate(PropertyInfo propertyInfo, MethodInfo getMethodInfo)
        {
            return Delegate.CreateDelegate(
                Expression.GetFuncType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                null,
                getMethodInfo);
        }

        /// <summary>
        /// Creates a function delegate to set a property value.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="getMethodInfo">
        /// The set method info.
        /// </param>
        /// <returns>
        /// A <see cref="System.Delegate"/> that sets the value of the property for any object of the declaring type.
        /// </returns>
        private static Delegate CreateActionDelegate(PropertyInfo propertyInfo, MethodInfo getMethodInfo)
        {
            return Delegate.CreateDelegate(
                Expression.GetActionType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                null,
                getMethodInfo);
        }

        /// <summary>
        /// Creates hash code.
        /// </summary>
        /// <returns>
        /// The hash code for this value as an <see cref="int" />.
        /// </returns>
        private int CreateHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }
    }
}