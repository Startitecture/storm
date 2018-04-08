// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniqueQuery.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    using System.Diagnostics;
    using System.Linq;

    using Startitecture.Orm.Model;

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
        public UniqueQuery(IEntityDefinitionProvider definitionProvider, TItem item)
            : base(definitionProvider)
        {
            var entityDefinition = definitionProvider.Resolve<TItem>();
            foreach (var keyAttribute in entityDefinition.PrimaryKeyAttributes)
            {
                this.AddFilter(new ValueFilter(keyAttribute, FilterType.Equality, keyAttribute.GetValueDelegate.DynamicInvoke(item)));
            }

            // Use all available values if no keys are defined. Trace a warning becuase that's a problem.
            if (this.Filters.Any() == false)
            {
                Trace.TraceWarning($"{typeof(TItem).FullName} does not have any key attributes defined.");

                foreach (var attribute in entityDefinition.DirectAttributes)
                {
                    this.AddFilter(new ValueFilter(attribute, FilterType.Equality, attribute.GetValueDelegate.DynamicInvoke(item)));
                }
            }
        }
    }
}