// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using SAF.Data.Providers.Tests.FieldsModel;

    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model.Models;

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