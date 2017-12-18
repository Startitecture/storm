// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAccountControlSettings.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains User Account Control (UAC) flags for Active Directory account principals.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;

    /// <summary>
    /// Contains User Account Control (UAC) flags for Active Directory account principals.
    /// </summary>
    [Flags]
    public enum UserAccountControlSettings
    {
        /// <summary>
        /// UAC flag for script.
        /// </summary>
        Script = 0x1,

        /// <summary>
        /// UAC flag indicating whether the account is disabled (true) or not (false).
        /// </summary>
        AccountDisabled = 0x2,

        /// <summary>
        /// UAC flag indicating whether the account requires a home directory (true) or not (false).
        /// </summary>
        RequiresHomeDirectory = 0x8,

        /// <summary>
        /// UAC flag indicating whether the account is locked out (true) or not (false).
        /// </summary>
        AccountLockedOut = 0x10,

        /// <summary>
        /// UAC flag indicating whether the account does not require a password (true) or if it does (false).
        /// </summary>
        PasswordNotRequired = 0x20,

        /// <summary>
        /// UAC flag indicating whether the account can change their password (true) or not (false).
        /// </summary>
        UserCannotChangePassword = 0x40,

        /// <summary>
        /// UAC flag indicating whether the account allows an encrypted text password (true) or not (false).
        /// </summary>
        EncryptedTextPasswordAllowed = 0x80,

        /// <summary>
        /// UAC flag indicating whether the account is temporarily duplicated (true) or not (false).
        /// </summary>
        TemporarilyDuplicatedAccount = 0x100,

        /// <summary>
        /// UAC flag indicating whether the account is a normal account (true) or not (false).
        /// </summary>
        NormalAccount = 0x200,

        /// <summary>
        /// UAC flag indicating whether the account is an inter-domain trust account (true) or not (false).
        /// </summary>
        InterDomainTrustAccount = 0x800,

        /// <summary>
        /// UAC flag indicating whether this is a workstation trust account (true) or not (false).
        /// </summary>
        WorkstationTrustAccount = 0x1000,

        /// <summary>
        /// UAC flag indicating whether this is a server trust account (true) or not (false).
        /// </summary>
        ServerTrustAccount = 0x2000,

        /// <summary>
        /// UAC flag indicating whether the account password never expires (true) or if it does (false).
        /// </summary>
        PasswordNeverExpires = 0x10000,

        /// <summary>
        /// UAC flag for Majority Node Set logon account.
        /// </summary>
        MajorityNodeSetLogOnAccount = 0x20000,

        /// <summary>
        /// UAC flag indicating whether the account requires a smart card (true) or not (false).
        /// </summary>
        RequiresSmartcard = 0x40000,

        /// <summary>
        /// UAC flag indicating whether the account is trusted for delegation (true) or not (false).
        /// </summary>
        TrustedForDelegation = 0x80000,

        /// <summary>
        /// UAC flag indicating whether the account is not delegated (true) or is (false).
        /// </summary>
        NotDelegated = 0x100000,

        /// <summary>
        /// UAC flag indicating whether the account uses a DES key only (true) or not (false).
        /// </summary>
        UseDesKeyOnly = 0x200000,

        /// <summary>
        /// UAC flag indicating whether the account does not require pre-authentication (true) or does (false).
        /// </summary>
        DoNotRequirePreAuthentication = 0x400000,

        /// <summary>
        /// UAC flag indicating whether the account password is expired (true) or not (false).
        /// </summary>
        PasswordExpired = 0x800000,

        /// <summary>
        /// UAC flag indicating the account is trusted to authorize for delegation.
        /// </summary>
        TrustedToAuthorizeDelegation = 0x1000000
    }
}
