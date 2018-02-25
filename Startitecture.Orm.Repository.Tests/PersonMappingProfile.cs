// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The person mapping profile.
    /// </summary>
    public class PersonMappingProfile : EntityMappingProfile<Person, ActionPrincipalRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonMappingProfile" /> class.
        /// </summary>
        public PersonMappingProfile()
        {
            this.SetPrimaryKey(person => person.PersonId, row => row.PersonId)
                .SetUniqueKey(person => person.AccountName)
                .IgnoreForDataItem(row => row.UserId, row => row.AccountName);
        }
    }
}