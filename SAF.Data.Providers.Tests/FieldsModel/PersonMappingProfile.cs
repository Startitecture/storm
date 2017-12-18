// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The person mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
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
