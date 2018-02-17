// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The person mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The person mapping profile.
    /// </summary>
    public class PersonMappingProfile : EntityMappingProfile<Person, PersonRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonMappingProfile"/> class.
        /// </summary>
        public PersonMappingProfile()
        {
            this.SetPrimaryKey(person => person.PersonId, row => row.PersonId);
        }
    }
}
