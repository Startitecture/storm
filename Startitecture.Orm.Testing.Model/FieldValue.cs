// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValue.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The field value.
    /// </summary>
    public class FieldValue
    {
        /// <summary>
        /// The elements.
        /// </summary>
        private readonly List<FieldValueElement> elements = new List<FieldValueElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValue"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public FieldValue(Field field)
        {
            this.Field = field;
        }

        /// <summary>
        /// Gets the field value id.
        /// </summary>
        public long? FieldValueId { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public Field Field { get; private set; }

        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        public DomainIdentity LastModifiedBy { get; private set; }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTimeOffset LastModifiedTime { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public IEnumerable<FieldValueElement> Elements => this.elements;

        /// <summary>
        /// Sets the value of the field.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="identity">
        /// The identity setting the value.
        /// </param>
        /// <returns>
        /// The current <see cref="FieldValue"/>.
        /// </returns>
        public FieldValue Set(object value, DomainIdentity identity)
        {
            this.elements.Clear();

            if (value is ICollection list)
            {
                foreach (var item in list)
                {
                    var element = new FieldValueElement(value);
                    this.elements.Add(element);
                    element.AddToValue(this);
                }
            }
            else
            {
                var element = new FieldValueElement(value);
                this.elements.Add(element);
                element.AddToValue(this);
            }

            this.LastModifiedBy = identity;
            this.LastModifiedTime = DateTimeOffset.Now;
            return this;
        }
    }
}