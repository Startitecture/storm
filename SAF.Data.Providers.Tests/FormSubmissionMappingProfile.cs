// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The process activity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
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