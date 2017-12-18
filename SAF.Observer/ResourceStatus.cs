// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains the status of a cluster resource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains the status of a resource.
    /// </summary>
    public sealed class ResourceStatus : IEquatable<ResourceStatus>, IComparable, IComparable<ResourceStatus>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ResourceStatus, object>[] ComparisonProperties =
            {
                item => item.QualifiedName,
                item => item.IsAvailable,
                item => item.Weight
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStatus"/> class.
        /// </summary>
        /// <param name="resourceMonitor">
        /// The resource monitor that will update this status.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceMonitor"/> is null.
        /// </exception>
        public ResourceStatus(ResourceMonitor resourceMonitor)
            : this()
        {
            if (resourceMonitor == null)
            {
                throw new ArgumentNullException(nameof(resourceMonitor));
            }

            this.QualifiedName = string.Concat(resourceMonitor.GetType().FullName, ':', resourceMonitor.Location);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStatus"/> class.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name.
        /// </param>
        /// <param name="isAvailable">
        /// A value indicating whether the resource is available.
        /// </param>
        /// <param name="weight">
        /// The relative weight of accessing the resource.
        /// </param>
        public ResourceStatus(string qualifiedName, bool isAvailable, double weight)
        {
            this.QualifiedName = qualifiedName;
            this.IsAvailable = isAvailable;
            this.Weight = weight;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ResourceStatus"/> class from being created.
        /// </summary>
        private ResourceStatus()
        {
            this.IsAvailable = false;
            this.Weight = double.MaxValue;
        }

        /// <summary>
        /// Gets a value indicating whether the resource is available.
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        /// Gets the status error, if any, associated with the resource status.
        /// </summary>
        public Exception StatusError { get; private set; }

        /// <summary>
        /// Gets the relative weight of accessing the resource.
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Gets the qualified name of the resource.
        /// </summary>
        public string QualifiedName { get; private set; }

        #region Value Comparison

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ResourceStatus valueA, ResourceStatus valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Compares two values for inequality.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ResourceStatus valueA, ResourceStatus valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(ResourceStatus valueA, ResourceStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(ResourceStatus valueA, ResourceStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ResourceStatus"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="ResourceStatus"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="ResourceStatus"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="ResourceStatus"/>. </param><filterpriority>2</filterpriority>
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
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ResourceStatus other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ResourceStatus other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.QualifiedName;
        }

        /// <summary>
        /// Updates the current resource status.
        /// </summary>
        /// <param name="isAvailable">
        /// A value indicating whether the resource is available.
        /// </param>
        /// <param name="weight">
        /// The relative weight of accessing the resource.
        /// </param>
        public void Update(bool isAvailable, double weight)
        {
            this.Update(isAvailable, weight, null);
        }

        /// <summary>
        /// Updates the current resource status.
        /// </summary>
        /// <param name="isAvailable">
        /// A value indicating whether the resource is available.
        /// </param>
        /// <param name="weight">
        /// The relative weight of accessing the resource.
        /// </param>
        /// <param name="statusError">
        /// The status Error.
        /// </param>
        public void Update(bool isAvailable, double weight, Exception statusError)
        {
            this.IsAvailable = isAvailable;
            this.Weight = weight;
            this.StatusError = statusError;
        }
    }
}
