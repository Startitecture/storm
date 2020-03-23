// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalResource.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Common.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// The external resource.
    /// </summary>
    public class ExternalResource : IEquatable<ExternalResource>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ExternalResource, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.Uri,
                item => item.ResourceClassification
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalResource"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="resourceClassification">
        /// The resource classification.
        /// </param>
        public ExternalResource(string name, ResourceClassification resourceClassification)
            : this(name, resourceClassification, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalResource"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="resourceClassification">
        /// The resource classification.
        /// </param>
        /// <param name="externalResourceId">
        /// The external resource ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceClassification"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public ExternalResource(string name, ResourceClassification resourceClassification, int? externalResourceId)
        {
            if (resourceClassification == null)
            {
                throw new ArgumentNullException(nameof(resourceClassification));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            this.Name = name;
            this.ResourceClassification = resourceClassification;
            this.ExternalResourceId = externalResourceId;
        }

        /// <summary>
        /// Gets the external resource ID.
        /// </summary>
        public int? ExternalResourceId { get; private set; }

        /// <summary>
        /// Gets the resource classification.
        /// </summary>
        public ResourceClassification ResourceClassification { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        public Uri Uri { get; set; }

        #region Equality and Comparison Methods and Operators

        /// <summary>
        /// Determines if two values of the same type are equal.
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
        public static bool operator ==(ExternalResource valueA, ExternalResource valueB)
        {
            return EqualityComparer<ExternalResource>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
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
        public static bool operator !=(ExternalResource valueA, ExternalResource valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{this.Name} ({this.Uri})";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
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
        public bool Equals(ExternalResource other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}