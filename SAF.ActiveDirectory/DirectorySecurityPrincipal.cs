// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectorySecurityPrincipal.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains basic properties for entities that are directory security principals, such as users, computers and
//   security groups.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains basic properties for entities that are directory security principals, such as users, computers and 
    /// security groups.
    /// </summary>
    public class DirectorySecurityPrincipal : DirectoryPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySecurityPrincipal"/> class.
        /// </summary>
        public DirectorySecurityPrincipal()
        {
        }

        /// <summary>
        /// Gets or sets the display name of the directory object.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the directory object.
        /// </summary>
        public string Description { get; set; }

        #region Base Class Overrides

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="DirectorySecurityPrincipal"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="DirectorySecurityPrincipal"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }

            return (obj is DirectorySecurityPrincipal) ? this.Equals(obj as DirectorySecurityPrincipal) : false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DirectorySecurityPrincipal"/> is equal to the current <see cref="DirectorySecurityPrincipal"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectorySecurityPrincipal"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DirectorySecurityPrincipal"/> is equal to the current <see cref="DirectorySecurityPrincipal"/>; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public bool Equals(DirectorySecurityPrincipal obj)
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
                    this.AccountName,
                    this.DisplayName,
                    this.Description
                };

            object[] objProperties =
                new object[]
                {
                    obj.AccountName,
                    obj.DisplayName,
                    obj.Description
                };

            return Evaluate.CollectionEquals(thisProperties, objProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="DirectoryEntity"/>.</returns>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Evaluate.GenerateHashCode(
                    this.AccountName,
                    this.DisplayName,
                    this.Description);
        }

        #endregion
    }
}
