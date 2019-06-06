// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PocoDelegateInfo.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// CContains metadata for a POCO delegate.
    /// </summary>
    public class PocoDelegateInfo
    {
        /// <summary>
        /// The attribute definitions.
        /// </summary>
        private readonly List<EntityAttributeDefinition> attributeDefinitions = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The mapped definitions.
        /// </summary>
        private readonly List<EntityAttributeDefinition> mappedDefinitions = new List<EntityAttributeDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PocoDelegateInfo"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="pocoType">
        /// The POCO type.
        /// </param>
        /// <param name="attributeDefinitions">
        /// The attribute definitions.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pocoType"/> or <paramref name="attributeDefinitions"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="methodName"/> is null or white space.
        /// </exception>
        public PocoDelegateInfo(
            [NotNull] string methodName,
            [NotNull] Type pocoType,
            [NotNull] IEnumerable<EntityAttributeDefinition> attributeDefinitions)
        {
            if (pocoType == null)
            {
                throw new ArgumentNullException(nameof(pocoType));
            }

            if (attributeDefinitions == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinitions));
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(methodName));
            }

            this.MethodName = methodName;
            this.PocoType = pocoType;
            this.attributeDefinitions.AddRange(attributeDefinitions);
        }

        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Gets the POCO type.
        /// </summary>
        public Type PocoType { get; }

        /// <summary>
        /// The attribute definitions.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> AttributeDefinitions => this.attributeDefinitions;

        /// <summary>
        /// The mapped definitions.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> MappedDefinitions => this.mappedDefinitions;

        /// <summary>
        /// Gets the mapping delegate.
        /// </summary>
        public Delegate MappingDelegate { get; private set; }

        /// <summary>
        /// The map definition.
        /// </summary>
        /// <param name="attributeDefinition">
        /// The attribute definition.
        /// </param>
        public void MapDefinition(EntityAttributeDefinition attributeDefinition)
        {
            this.mappedDefinitions.Add(attributeDefinition);
        }

        /// <summary>
        /// Sets the mapping delegate for this POCO type.
        /// </summary>
        /// <param name="mappingDelegate">
        /// The mapping delegate to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mappingDelegate"/> is null.
        /// </exception>
        public void SetDelegate([NotNull] Delegate mappingDelegate)
        {
            if (mappingDelegate == null)
            {
                throw new ArgumentNullException(nameof(mappingDelegate));
            }

            this.MappingDelegate = mappingDelegate;
        }
    }
}