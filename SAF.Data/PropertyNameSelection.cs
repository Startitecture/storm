// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNameSelection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains a collection of property names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    using SAF.Core;

    /// <summary>
    /// Contains a collection of property names.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to store property names for.
    /// </typeparam>
    public class PropertyNameSelection<TItem> : IPropertyNameSelection
    {
        #region Fields

        /// <summary>
        /// The property names.
        /// </summary>
        private readonly Collection<string> propertyNames = new Collection<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameSelection{TItem}"/> class.
        /// </summary>
        /// <param name="propertyNames">
        /// The property names.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyNames"/> is null.
        /// </exception>
        public PropertyNameSelection(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }

            foreach (var propertyName in propertyNames)
            {
                this.propertyNames.Add(propertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameSelection{TItem}"/> class.
        /// </summary>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attributes"/> is null.
        /// </exception>
        public PropertyNameSelection(params EntityAttributeDefinition[] attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            foreach (var attribute in attributes)
            {
                this.propertyNames.Add(attribute.PropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameSelection{TItem}"/> class.
        /// </summary>
        /// <param name="selections">
        /// The selections.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selections"/> is null.
        /// </exception>
        public PropertyNameSelection(params Expression<Func<TItem, object>>[] selections)
        {
            if (selections == null)
            {
                throw new ArgumentNullException("selections");
            }

            foreach (var expression in selections)
            {
                this.propertyNames.Add(expression.GetPropertyName());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameSelection{TItem}"/> class.
        /// </summary>
        public PropertyNameSelection()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the property names stored in the collection.
        /// </summary>
        public Collection<string> PropertiesToInclude
        {
            get
            {
                return new Collection<string>(this.SelectProperties().ToList());
            }
        }

        #endregion

        /// <summary>
        /// Selects the properties to return.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="String"/> elements representing the property names.
        /// </returns>
        protected virtual IEnumerable<string> SelectProperties()
        {
            return this.propertyNames;
        }
    }
}