// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniqueQuery.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Automatically generates a unique query based on the primary keys of the <typeparamref name="TItem"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to generate a unique query for.
    /// </typeparam>
    public class UniqueQuery<TItem> : ItemSelection<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueQuery{TItem}"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public UniqueQuery([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] TItem item)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entityDefinition = definitionProvider.Resolve<TItem>();

            foreach (var keyAttribute in entityDefinition.PrimaryKeyAttributes)
            {
                var entityReference = new EntityReference { EntityAlias = keyAttribute.Entity.Alias, EntityType = keyAttribute.Entity.EntityType };
                var attributeLocation = new AttributeLocation(keyAttribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, keyAttribute.GetValueDelegate.DynamicInvoke(item));

                this.AddFilter(valueFilter);
            }

            // Use all available values if no keys are defined.
            if (this.Filters.Any())
            {
                return;
            }

            Trace.TraceWarning($"{typeof(TItem).FullName} does not have any key attributes defined.");

            foreach (var attribute in entityDefinition.DirectAttributes)
            {
                var entityReference = new EntityReference { EntityAlias = attribute.Entity.Alias, EntityType = attribute.Entity.EntityType };
                var attributeLocation = new AttributeLocation(attribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, attribute.GetValueDelegate.DynamicInvoke(item));
                this.AddFilter(valueFilter);
            }
        }
    }
}