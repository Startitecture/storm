// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Observer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;

    /// <summary>
    /// Contains the combined status of one or more resources.
    /// </summary>
    public class ComponentStatus : IEquatable<ComponentStatus>, IComparable, IComparable<ComponentStatus>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} [{1}, {2}]";

        #region Static Fields

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ComponentStatus, object>[] ComparisonProperties =
            {
                item => item.QualifiedName, 
                item => item.IsAvailable, 
                item => item.resources
            };

        /// <summary>
        /// The ranked resource weight selector.
        /// </summary>
        private static readonly Func<RankedResourceStatus, double> RankedResourceWeightSelector =
            x => x.ResourceStatus.Weight / (x.ResourceRank + 1);

        /// <summary>
        /// The resource weight selector.
        /// </summary>
        private static readonly Func<RankedResourceStatus, double> ResourceWeightSelector = x => x.ResourceStatus.Weight;

        /// <summary>
        /// The availability selector.
        /// </summary>
        private static readonly Func<RankedResourceStatus, bool> AvailabilitySelector = x => x.ResourceStatus.IsAvailable;

        #endregion

        #region Fields

        /// <summary>
        /// The resources.
        /// </summary>
        private readonly List<RankedResourceStatus> resources;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentStatus"/> class.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name of the component.
        /// </param>
        /// <param name="statuses">
        /// The resource statuses that are part of the component.
        /// </param>
        public ComponentStatus(string qualifiedName, params ResourceStatus[] statuses)
        {
            this.QualifiedName = qualifiedName;
            this.resources = new List<RankedResourceStatus>(statuses.Select((status, i) => new RankedResourceStatus(i, status)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the qualified name of the component.
        /// </summary>
        public string QualifiedName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the component is available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return this.resources.All(AvailabilitySelector);
            }
        }

        /// <summary>
        /// Gets the resource weight of the current component.
        /// </summary>
        public double ResourceWeight
        {
            get
            {
                return this.resources.Sum(ResourceWeightSelector);
            }
        }

        /// <summary>
        /// Gets the ranked resource weight of the current component.
        /// </summary>
        public double RankedResourceWeight
        {
            get
            {
                return this.resources.Sum(RankedResourceWeightSelector);
            }
        }

        /// <summary>
        /// Gets the dependent resources of the component.
        /// </summary>
        public IEnumerable<RankedResourceStatus> Resources
        {
            get
            {
                return this.resources;
            }
        }

        #endregion

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
        public static bool operator ==(ComponentStatus valueA, ComponentStatus valueB)
        {
            return Evaluate.Equals(valueA, valueB);
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
        public static bool operator >(ComponentStatus valueA, ComponentStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
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
        public static bool operator !=(ComponentStatus valueA, ComponentStatus valueB)
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
        public static bool operator <(ComponentStatus valueA, ComponentStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the
        /// current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        /// Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to
        /// <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance. 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
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
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal
        /// to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(ComponentStatus other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current
        /// <see cref="ComponentStatus"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="ComponentStatus"/>;
        /// otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="ComponentStatus"/>. 
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
        public bool Equals(ComponentStatus other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ComponentStatus" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="ComponentStatus"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="ComponentStatus"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(
                ToStringFormat, 
                this.QualifiedName, 
                this.ResourceWeight, 
                this.RankedResourceWeight);
        }

        #endregion
    }
}