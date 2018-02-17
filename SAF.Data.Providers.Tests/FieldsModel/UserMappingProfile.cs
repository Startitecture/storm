// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The user mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Repository.Mapping;

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