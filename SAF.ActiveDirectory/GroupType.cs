// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Active Directory group types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Active Directory group types.
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// The group type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Local distribution group type
        /// </summary>
        LocalDistribution = 4,

        /// <summary>
        /// Global distribution group type
        /// </summary>
        GlobalDistribution = 2,

        /// <summary>
        /// Universal distribution group type
        /// </summary>
        UniversalDistribution = 8,

        /// <summary>
        /// Built-in security group type
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming", 
            "CA1702:CompoundWordsShouldBeCasedCorrectly", 
            MessageId = "InSecurity", 
            Justification = "This word is supposed to be Built-In Security, not 'Insecurity'.")]
        BuiltInSecurity = -2147483643,

        /// <summary>
        /// Local security group type
        /// </summary>
        LocalSecurity = -2147483644,

        /// <summary>
        /// Global security group type
        /// </summary>
        GlobalSecurity = -2147483646
    }
}
