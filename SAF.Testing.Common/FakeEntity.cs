// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeEntity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A fake entity for tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A fake entity for tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeEntity : IEquatable<FakeEntity>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        public DateTime DateValue { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fake entity id.
        /// </summary>
        public int? FakeEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(FakeEntity left, FakeEntity right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(FakeEntity left, FakeEntity right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(FakeEntity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.FakeEntityId == other.FakeEntityId && string.Equals(this.Name, other.Name)
                   && string.Equals(this.Description, other.Description, StringComparison.OrdinalIgnoreCase)
                   && this.DateValue.Equals(other.DateValue);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((FakeEntity)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.FakeEntityId.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397)
                           ^ (this.Description != null
                                  ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Description)
                                  : 0);
                hashCode = (hashCode * 397) ^ this.DateValue.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format("[{0}] {1}: {2}", this.FakeEntityId, this.Name, this.Description);
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (this.FakeEntityId.HasValue && this.FakeEntityId.GetValueOrDefault() < 1)
            {
                errors.Add("The ID has to be greater than zero if set.");
            }

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                errors.Add("The name of the entity must be set.");
            }

            if (this.DateValue > DateTime.Now)
            {
                errors.Add("There's no way DateValue can ever be higher than DateTime.Now.");
            }

            return errors;
        }

        #endregion
    }
}