// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The user mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The user mapping profile.
    /// </summary>
    public class UserMappingProfile : EntityMappingProfile<User, UserRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMappingProfile"/> class.
        /// </summary>
        public UserMappingProfile()
        {
            this.SetPrimaryKey(user => user.PersonId, row => row.UserId).SetUniqueKey(row => row.AccountName);
        }
    }
}