// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form version mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The form version mapping profile.
    /// </summary>
    public class FormVersionMappingProfile : EntityMappingProfile<FormVersion, FormVersionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersionMappingProfile" /> class.
        /// </summary>
        public FormVersionMappingProfile()
        {
            this.SetPrimaryKey(version => version.FormVersionId, row => row.FormVersionId)
                .MapRelation(version => version.Form, row => row.Form, row => row.FormId)
                .MapRelation(version => version.CreatedBy, row => row.CreatedBy, row => row.PersonId)
                .MapRelation(version => version.LastModifiedBy, row => row.LastModifiedBy, row => row.PersonId);
        }
    }
}