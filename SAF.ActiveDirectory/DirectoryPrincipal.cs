// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryPrincipal.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for Active Directory principal wrappers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Base class for Active Directory principal wrappers.
    /// </summary>
    public abstract class DirectoryPrincipal : DirectoryEntity
    {
        /// <summary>
        /// Gets or sets the account name used to log into the domain.
        /// </summary>
        public string AccountName { get; set; }

        /////// <summary>
        /////// Returns a <see cref="Principal"/> instance matching the current <see cref="DirectorySecurityPrincipal"/>'s identifiers.
        /////// </summary>
        /////// <param name="context">The <see cref="PrincipalContext"/> to perform the search in.</param>
        /////// <returns>A <see cref="Principal"/> matching the current <see cref="DirectorySecurityPrincipal"/>'s identifiers, or 
        /////// null if no match was found.</returns>
        /////// <exception cref="InvalidOperationException">
        /////// The current <see cref="DirectorySecurityPrincipal"/> does not have at least one of its directory identifier 
        /////// properties (<see cref="M:DirectoryGuid"/> or <see cref="M:DistinguishedName"/>) set.</exception>
        ////[DirectoryServicesPermission(SecurityAction.LinkDemand)]
        ////public Principal FindPrincipal(PrincipalContext context)
        ////{
        ////    if (this.DirectoryGuid == null)
        ////    {
        ////        if (String.IsNullOrEmpty(this.DistinguishedName))
        ////        {
        ////            throw new InvalidOperationException(ErrorMessages.DirectoryIdentifersRequired);
        ////        }
        ////        else
        ////        {
        ////            return Principal.FindByIdentity(context, IdentityType.DistinguishedName, this.DistinguishedName);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        return Principal.FindByIdentity(context, IdentityType.Guid, this.DirectoryGuid.ToString());
        ////    }
        ////}
    }
}
