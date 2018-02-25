// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The person mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

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
