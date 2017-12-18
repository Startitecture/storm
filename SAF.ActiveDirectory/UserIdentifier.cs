// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserIdentifier.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains properties that can be used to look up a user in Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains properties that can be used to look up a user in Active Directory.
    /// </summary>
    public class UserIdentifier : AccountIdentifier
    {
        /// <summary>
        /// The <see cref="UserIdentifier"/> of the current user.
        /// </summary>
        private static readonly UserIdentifier CurrentId =
            new UserIdentifier()
            {
                AccountName = Environment.UserName,
                Domain = Environment.UserDomainName,
                UserPrincipalName = String.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName)
            };

        /// <summary>
        /// Gets the <see cref="UserIdentifier"/> of the current user.
        /// </summary>
        public static UserIdentifier Current
        {
            get { return CurrentId; }
        }

        /// <summary>
        /// Gets or sets the user principal name of the user.
        /// </summary>
        public string UserPrincipalName { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="HostIdentifier"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representation of this <see cref="HostIdentifier"/>.</returns>
        public override string ToString()
        {
            return String.IsNullOrEmpty(this.UserPrincipalName) ? base.ToString() : this.UserPrincipalName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="UserIdentifier"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="UserIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is UserIdentifier) && this.Equals(obj as UserIdentifier);
        }

        /// <summary>
        /// Determines whether the specified <see cref="UserIdentifier"/> is equal to the current <see cref="UserIdentifier"/>.
        /// </summary>
        /// <param name="obj">The <see cref="UserIdentifier"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="UserIdentifier"/> is equal to the current <see cref="UserIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(UserIdentifier obj)
        {
            if (obj == null)
            {
                return false;
            }

            return base.Equals(obj) && this.UserPrincipalName.Equals(obj.UserPrincipalName);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="HostIdentifier"/>.</returns>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Evaluate.GenerateHashCode(this.UserPrincipalName);
        }

        /// <summary>
        /// Returns a list of validation errors for the current entity.
        /// </summary>
        /// <param name="messages">
        /// A list of existing messages. New messages should be added to this list.
        /// </param>
        /// <returns>
        /// A collection of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /// </returns>
        protected override IEnumerable<string> Validate(IList<string> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            if (String.IsNullOrWhiteSpace(this.UserPrincipalName))
            {
                messages.Add(ValidationMessages.UserIdentifierRequiresUpn);
            }

            return messages;
        }

        #endregion
    }
}
