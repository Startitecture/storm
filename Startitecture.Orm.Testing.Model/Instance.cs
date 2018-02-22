namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The instance.
    /// </summary>
    [TableName("Instance")]
    [ExplicitColumns]
    [PrimaryKey("InstanceId", AutoIncrement = true)]
    public class Instance : IEquatable<Instance>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Instance, object>[] ComparisonProperties =
            {
                item => item.InstanceId,
                item => item.Name,
                item => item.OwnerId,
                item => item.TemplateVersionId,
                item => item.TemplateVersion
            };

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        [Column]
        public int InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the owner id.
        /// </summary>
        [Column]
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the template version id.
        /// </summary>
        [Column]
        public int TemplateVersionId { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        [Relation]
        public TemplateVersion TemplateVersion { get; set; }

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
        public static bool operator ==(Instance valueA, Instance valueB)
        {
            return EqualityComparer<Instance>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(Instance valueA, Instance valueB)
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
            return this.Name;
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
        public bool Equals(Instance other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}