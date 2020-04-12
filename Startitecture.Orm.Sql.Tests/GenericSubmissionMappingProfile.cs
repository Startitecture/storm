// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using AutoMapper;

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
            this.CreateMap<DomainIdentity, DomainIdentityRow>().ForMember(row => row.TransactionProvider, expression => expression.Ignore());
            this.CreateMap<DomainIdentityRow, DomainIdentity>();

            this.CreateMap<GenericSubmission, GenericSubmissionRow>()
                .ForMember(
                    row => row.SubmittedByDomainIdentiferId,
                    expression => expression.MapFrom(submission => submission.SubmittedBy.DomainIdentityId.GetValueOrDefault()))
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<GenericSubmissionRow, GenericSubmission>();

            this.CreateMap<Field, Field>();
            this.CreateMap<Field, FieldRow>().ForMember(row => row.TransactionProvider, expression => expression.Ignore());
            this.CreateMap<FieldRow, Field>();

            this.CreateMap<FieldValueTableTypeRow, FieldValue>()
                .ForMember(value => value.Field, expression => expression.Ignore())
                .ForMember(value => value.LastModifiedBy, expression => expression.Ignore());

            this.CreateMap<FieldValueElementTableTypeRow, FieldValueElement>()
                .ForMember(element => element.FieldValue, expression => expression.Ignore());
        }
    }
}