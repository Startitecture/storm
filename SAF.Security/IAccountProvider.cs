// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that locate user accounts of a specific type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System.Security.Principal;

    using SAF.Core;

    /// <summary>
    /// Provides an interface for classes that locate user accounts of a specific type.
    /// </summary>
    /// <typeparam name="TAccount">
    /// The type of account located by the provider.
    /// </typeparam>
    public interface IAccountProvider<out TAccount> : IAccountInfoProvider
        where TAccount : IAccountInfo
    {
        #region Public Methods and Operators

        /// <summary>
        /// Finds the account that matches the specified identifier.
        /// </summary>
        /// <param name="accountIdentifier">
        /// The identifier of the account to locate.
        /// </param>
        /// <returns>
        /// The account associated with the identifier.
        /// </returns>
        TAccount Find(string accountIdentifier);

        /// <summary>
        /// Finds the account that matches the specified identifier.
        /// </summary>
        /// <param name="accountIdentity">
        /// The identity of the account to locate.
        /// </param>
        /// <returns>
        /// The account associated with the identity.
        /// </returns>
        TAccount Find(IIdentity accountIdentity);

        #endregion
    }
}