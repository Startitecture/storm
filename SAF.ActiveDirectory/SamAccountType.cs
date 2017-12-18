// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamAccountType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Represents SAM Account Types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Represents SAM Account Types.
    /// </summary>
    public enum SamAccountType
    {
        /// <summary>
        /// The SAM Account type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Local Security group flag
        /// </summary>
        LocalSecurityGroup = 536870912,

        /// <summary>
        /// Global Security group flag
        /// </summary>
        GlobalSecurityGroup = 268435456,

        /// <summary>
        /// Local Distribution group flag
        /// </summary>
        LocalDistributionGroup = 536870913,

        /// <summary>
        /// Global and Universal Distribution group flag
        /// </summary>
        GlobalUniversalDistributionGroup = 268435457
    }
}
