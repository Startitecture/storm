// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSliceInstance.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Common.Model
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The data slice instance.
    /// </summary>
    public class DataSliceInstance : IEquatable<DataSliceInstance>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DataSliceInstance, object>[] ComparisonProperties =
            {
                item => item.Identifier,
                item => item.DataSlice
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSliceInstance"/> class.
        /// </summary>
        /// <param name="dataSlice">
        /// The data slice.
        /// </param>
        public DataSliceInstance(DataSlice dataSlice)
            : this(dataSlice, Guid.NewGuid(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSliceInstance"/> class.
        /// </summary>
        /// <param name="dataSlice">
        /// The data slice.
        /// </param>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="dataSliceInstanceId">
        /// The data slice instance ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataSlice"/> is null.
        /// </exception>
        public DataSliceInstance(DataSlice dataSlice, Guid identifier, int? dataSliceInstanceId)
        {
            if (dataSlice == null)
            {
                throw new ArgumentNullException(nameof(dataSlice));
            }

            this.DataSlice = dataSlice;
            this.Identifier = identifier;
            this.DataSliceInstanceId = dataSliceInstanceId;
        }

        /// <summary>
        /// Gets the data slice instance ID.
        /// </summary>
        public int? DataSliceInstanceId { get; private set; }

        /// <summary>
        /// Gets the data slice.
        /// </summary>
        public DataSlice DataSlice { get; private set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public Guid Identifier { get; private set; }

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
        public static bool operator ==(DataSliceInstance valueA, DataSliceInstance valueB)
        {
            return EqualityComparer<DataSliceInstance>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(DataSliceInstance valueA, DataSliceInstance valueB)
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
            return $"{this.DataSlice} ({this.Identifier})";
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
        public bool Equals(DataSliceInstance other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}