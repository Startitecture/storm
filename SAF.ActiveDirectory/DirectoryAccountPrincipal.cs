// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryAccountPrincipal.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains directory properties for entities that are directory account principals, such as users and computers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains directory properties for entities that are directory account principals, such as users and computers.
    /// </summary>
    public class DirectoryAccountPrincipal : DirectorySecurityPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryAccountPrincipal"/> class.
        /// </summary>
        public DirectoryAccountPrincipal()
        {
        }

        /// <summary>
        /// Gets or sets the account status of the directory account principal.
        /// </summary>
        public AccountStatus AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the user principal name of the directory account principal.
        /// </summary>
        public string UserPrincipalName { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="DirectoryAccountPrincipal"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="DirectoryAccountPrincipal"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is DirectoryAccountPrincipal) ? this.Equals(obj as DirectoryAccountPrincipal) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DirectoryAccountPrincipal"/> is equal to the current <see cref="DirectoryAccountPrincipal"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryAccountPrincipal"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DirectoryAccountPrincipal"/> is equal to the current <see cref="DirectoryAccountPrincipal"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(DirectoryAccountPrincipal obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            object[] thisProperties =
                new object[] 
                {
                    this.AccountStatus,
                    this.UserPrincipalName
                };

            object[] objProperties =
                new object[]
                {
                    obj.AccountStatus,
                    obj.UserPrincipalName
                };

            return Evaluate.CollectionEquals(thisProperties, objProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="DirectoryAccountPrincipal"/>.</returns>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Evaluate.GenerateHashCode(
                    this.AccountStatus,
                    this.UserPrincipalName);
        }

        #endregion
    }
}
