// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountInfo.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains basic account information for a specific user account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains basic account information for a specific user account.
    /// </summary>
    public abstract class AccountInfo : IEquatable<AccountInfo>, IAccountInfo
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<AccountInfo, object>[] ComparisonProperties =
            {
                item => item.AccountName, 
                item => item.Email, 
                item => item.FullName, 
                item => item.Title
            };

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        public abstract string AccountName { get; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Gets the job title of the user.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public abstract string Email { get; }

        /// <summary>
        /// Gets a list of group memberships for the user account.
        /// </summary>
        public abstract IEnumerable<GroupMembership> GroupMemberships { get; }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="AccountInfo"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="AccountInfo"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="AccountInfo"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="AccountInfo"/>. 
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
        public virtual bool Equals(AccountInfo other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}