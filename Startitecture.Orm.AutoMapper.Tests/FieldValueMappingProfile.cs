// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.AutoMapper.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The field value mapping profile.
    /// </summary>
    public class FieldValueMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueMappingProfile"/> class.
        /// </summary>
        public FieldValueMappingProfile()
        {
            this.CreateMap<FieldRow, Field>();
            this.CreateMap<FieldValueRow, FieldValue>().ResolveByKey(row => row.Field, value => value.Field, "field", row => row.FieldId);
            this.CreateMap<DomainIdentityRow, DomainIdentity>();
        }
    }
}