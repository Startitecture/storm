// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The generic submission mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The generic submission mapping profile.
    /// </summary>
    public class GenericSubmissionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSubmissionMappingProfile"/> class.
        /// </summary>
        public GenericSubmissionMappingProfile()
        {
            this.CreateMap<DomainIdentity, DomainIdentityRow>();
            this.CreateMap<DomainIdentityRow, DomainIdentity>();

            this.CreateMap<GenericSubmission, GenericSubmissionRow>()
                .ForMember(
                    row => row.SubmittedByDomainIdentifierId,
                    expression => expression.MapFrom(submission => submission.SubmittedBy.DomainIdentityId.GetValueOrDefault()));

            this.CreateMap<GenericSubmissionRow, GenericSubmission>();

            this.CreateMap<Field, Field>();
            this.CreateMap<Field, FieldRow>();
            this.CreateMap<FieldRow, Field>();

            this.CreateMap<FieldTableTypeRow, Field>();

            this.CreateMap<FieldValueTableTypeRow, FieldValue>()
                .ForMember(value => value.Field, expression => expression.Ignore())
                .ForMember(value => value.LastModifiedBy, expression => expression.Ignore());

            this.CreateMap<GenericSubmissionValueRow, FieldValue>()
                .ForMember(value => value.Field, expression => expression.MapFrom(row => row.FieldValue.Field))
                .ForMember(value => value.LastModifiedBy, expression => expression.MapFrom(row => row.FieldValue.LastModifiedBy))
                .ForMember(value => value.LastModifiedTime, expression => expression.MapFrom(row => row.FieldValue.LastModifiedTime))
                .ForMember(value => value.FieldValueId, expression => expression.MapFrom(row => row.FieldValue.FieldValueId));

            this.CreateMap<FieldValueElementTableTypeRow, FieldValueElement>()
                .ForMember(element => element.FieldValue, expression => expression.Ignore());
        }
    }
}