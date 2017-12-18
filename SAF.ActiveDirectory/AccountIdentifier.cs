// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountIdentifier.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains properties that can be used to look up an account in Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// Contains properties that can be used to look up an account in Active Directory.
    /// </summary>
    public class AccountIdentifier : IValidatingEntity
    {
        /// <summary>
        /// A factory to create directory account principals.
        /// </summary>
        private readonly Factories.DirectoryAccountPrincipalFactory principalFactory = 
            new Factories.DirectoryAccountPrincipalFactory();

        /// <summary>
        /// Gets or sets the domain of the account.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the account name of the account.
        /// </summary>
        public string AccountName { get; protected set; }

        /// <summary>
        /// Returns a <see cref="DirectoryAccountPrincipal"/> for the current <see cref="AccountIdentifier"/> using the
        /// current domain's context.
        /// </summary>
        /// <returns>A <see cref="DirectoryAccountPrincipal"/> that represents the current <see cref="AccountIdentifier"/>, 
        /// or null if the principal could not be found.</returns>
        [PermissionSet(SecurityAction.LinkDemand)]
        public DirectoryAccountPrincipal Find()
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                return this.Find(context);
            }
        }

        /// <summary>
        /// Returns a <see cref="DirectoryAccountPrincipal"/> for the current <see cref="AccountIdentifier"/> using the
        /// specified context.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> to locate the principal in.</param>
        /// <returns>A <see cref="DirectoryAccountPrincipal"/> that represents the current <see cref="AccountIdentifier"/>, 
        /// or null if the principal could not be found.</returns>
        [PermissionSet(SecurityAction.LinkDemand)]
        public DirectoryAccountPrincipal Find(PrincipalContext context)
        {
            if (String.IsNullOrEmpty(this.AccountName))
            {
                throw new InvalidOperationException(ErrorMessages.AccountNameRequired);
            }

            using 
                (var principal = 
                    (AuthenticablePrincipal)Principal.FindByIdentity(
                        context, IdentityType.SamAccountName, this.AccountName))
            {
                return principal == null ? null : this.principalFactory.CreateFrom(principal);
            }
        }

        #region Base Class Overrides

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="AccountIdentifier"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representation of the current <see cref="AccountIdentifier"/>.</returns>
        public override string ToString()
        {
            return String.Format(@"{0}\{1}", this.Domain, this.AccountName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="AccountIdentifier"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="AccountIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj is AccountIdentifier) && this.Equals(obj as AccountIdentifier);
        }

        /// <summary>
        /// Determines whether the specified <see cref="AccountIdentifier"/> is equal to the current <see cref="AccountIdentifier"/>.
        /// </summary>
        /// <param name="obj">The <see cref="AccountIdentifier"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="AccountIdentifier"/> is equal to the current <see cref="AccountIdentifier"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(AccountIdentifier obj)
        {
            if (obj == null)
            {
                return false;
            }

            var thisProperties =
                new object[] 
                {
                    this.AccountName,
                    this.Domain
                };

            var objProperties =
                new object[]
                {
                    obj.AccountName,
                    obj.Domain
                };

            return Evaluate.CollectionEquals(thisProperties, objProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="AccountIdentifier"/>.</returns>
        public override int GetHashCode()
        {
            return
                Evaluate.GenerateHashCode(
                    this.AccountName,
                    this.Domain);
        }

        /// <summary>
        /// Returns a list of validation errors for the current entity.
        /// </summary>
        /// <returns>
        /// A collection of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /// </returns>
        public IEnumerable<string> Validate()
        {
            return this.Validate(new List<string>());
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
        protected virtual IEnumerable<string> Validate(IList<string> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            if (String.IsNullOrEmpty(this.AccountName))
            {
                messages.Add(ValidationMessages.AccountIdentifierRequiresAccountName);
            }

            return messages;
        }

        #endregion
    }
}
