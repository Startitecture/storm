// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueElement.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// The field value element.
    /// </summary>
    public class FieldValueElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueElement"/> class.
        /// </summary>
        /// <param name="element">
        /// The value element.
        /// </param>
        public FieldValueElement(object element)
        {
            this.Element = element;
        }

        /// <summary>
        /// Gets the field value element ID.
        /// </summary>
        public long? FieldValueElementId { get; private set; }

        /// <summary>
        /// Gets the value element.
        /// </summary>
        public object Element { get; }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        public FieldValue FieldValue { get; private set; }

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Adds the element to a value.
        /// </summary>
        /// <param name="fieldValue">
        /// The field value to add this element to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldValue"/> is null.
        /// </exception>
        internal void AddToValue([NotNull] FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            this.Order = fieldValue.Elements.Count();
        }
    }
}