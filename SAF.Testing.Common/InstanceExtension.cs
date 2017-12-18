namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;
    using SAF.Data.Providers;

    /// <summary>
    /// The instance extension.
    /// </summary>
    [TableName("InstanceExtension")]
    [ExplicitColumns]
    [PrimaryKey("InstanceExtensionId", AutoIncrement = false)]
    public class InstanceExtension : IEquatable<InstanceExtension>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<InstanceExtension, object>[] ComparisonProperties =
            {
                item => item.InstanceExtensionId,
                item => item.Enabled
            };

        /// <summary>
        /// Gets or sets the instance extension id.
        /// </summary>
        [Column]
        public int InstanceExtensionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        [Column]
        public bool Enabled { get; set; }

        #region Equality and Comparison Methods

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
        public static bool operator ==(InstanceExtension valueA, InstanceExtension valueB)
        {
            return EqualityComparer<InstanceExtension>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(InstanceExtension valueA, InstanceExtension valueB)
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
            return $"{this.InstanceExtensionId}: {this.Enabled}";
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
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
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
        public bool Equals(InstanceExtension other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}