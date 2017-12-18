// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionPrincipal.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains information about a person or user associated with an action.
    /// </summary>
    public class ActionPrincipal : Person, IEquatable<ActionPrincipal>
    {
        /// <summary>
        /// The principal comparison properties.
        /// </summary>
        private static readonly Func<ActionPrincipal, object>[] PrincipalComparisonProperties =
            {
                item => item.Email,
                item => item.Title,
                item => item.FirstName,
                item => item.LastName,
                item => item.MiddleName,
                item => item.AccountName,
                item => item.UserId,
                item => item.FullName
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPrincipal" /> class.
        /// </summary>
        public ActionPrincipal()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPrincipal"/> class.
        /// </summary>
        /// <param name="personId">
        /// The person ID.
        /// </param>
        public ActionPrincipal(int? personId)
            : base(personId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPrincipal"/> class.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="personId">
        /// The person ID.
        /// </param>
        public ActionPrincipal(string accountName, int? userId, int? personId)
            : base(personId)
        {
            this.AccountName = accountName;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets the user ID, if the action principal is a user of the system.
        /// </summary>
        public int? UserId { get; private set; }

        /// <summary>
        /// Gets the account name, if the action principal is a user of the system.
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.AccountName ?? this.Email;
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
            return Evaluate.GenerateHashCode(this, PrincipalComparisonProperties);
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
        public bool Equals(ActionPrincipal other)
        {
            return Evaluate.Equals(this, other, PrincipalComparisonProperties);
        }
    }
}