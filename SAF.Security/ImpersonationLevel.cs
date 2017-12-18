// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpersonationLevel.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    /// <summary>
    /// The impersonation level.
    /// </summary>
    public enum ImpersonationLevel
    {
        /// <summary>
        /// The security anonymous.
        /// </summary>
        SecurityAnonymous = 0, 

        /// <summary>
        /// The security identification.
        /// </summary>
        SecurityIdentification = 1, 

        /// <summary>
        /// The security impersonation.
        /// </summary>
        SecurityImpersonation = 2, 

        /// <summary>
        /// The security delegation.
        /// </summary>
        SecurityDelegation = 3
    }
}