// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The process activity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The process activity mapping profile.
    /// </summary>
    public class FormSubmissionMappingProfile : EntityMappingProfile<FormSubmission, FormSubmissionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionMappingProfile"/> class.
        /// </summary>
        public FormSubmissionMappingProfile()
        {
            this.SetPrimaryKey(submission => submission.FormSubmissionId, row => row.FormSubmissionId);
        }
    }
}