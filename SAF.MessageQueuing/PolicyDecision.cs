// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolicyDecision.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Contains the result of a policy decision.
    /// </summary>
    public class PolicyDecision : IEquatable<PolicyDecision>
    {
        #region Constants

        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0}: {1}";

        #endregion

        #region Static Fields

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<PolicyDecision, object>[] ComparisonProperties = { item => item.IsApproved, item => item.Reason };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyDecision"/> class.
        /// </summary>
        /// <param name="isApproved">
        /// The is approved.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public PolicyDecision(bool isApproved, string reason)
        {
            this.Reason = reason;
            this.IsApproved = isApproved;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether is approved.
        /// </summary>
        public bool IsApproved { get; private set; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether two values of the same type are equal.
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
        public static bool operator ==(PolicyDecision valueA, PolicyDecision valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines whether two values of the same type are not equal.
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
        public static bool operator !=(PolicyDecision valueA, PolicyDecision valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise,
        /// false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
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
        public bool Equals(PolicyDecision other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(
                ToStringFormat, 
                this.IsApproved ? ActionMessages.PolicyAllowsRequest : ActionMessages.PolicyDeniesRequest, 
                this.Reason);
        }

        #endregion
    }
}