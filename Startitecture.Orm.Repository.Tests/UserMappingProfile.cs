// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The user mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

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