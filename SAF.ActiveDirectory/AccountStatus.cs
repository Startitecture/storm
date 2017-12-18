// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Status of an Active Directory account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Status of an Active Directory account.
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// The account status is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The account is in Active Directory and not expired or disabled.
        /// </summary>
        Active = 1,

        /// <summary>
        /// The account is expired (not valid for computer accounts).
        /// </summary>
        Expired = 2,

        /// <summary>
        /// The account is disabled.
        /// </summary>
        Disabled = 3,

        /// <summary>
        /// The account cannot be found in Active Directory.
        /// </summary>
        Missing = 4
    }
}
