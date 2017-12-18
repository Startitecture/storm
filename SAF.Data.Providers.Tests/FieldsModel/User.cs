// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;

    /// <summary>
    /// The user.
    /// </summary>
    public class User : Person
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        public User(string accountName)
            : this(accountName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        public User(string accountName, int? userId)
            : base(userId)
        {
            this.AccountName = accountName;
        }

        /// <summary>
        /// Gets the user ID.
        /// </summary>
        public int? UserId
        {
            get
            {
                return this.PersonId;
            }
        }

        /// <summary>
        /// Gets the account name.
        /// </summary>
        public string AccountName { get; private set; }
    }
}