// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form submission validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The form submission validator.
    /// </summary>
    public class FormSubmissionValidator : AbstractValidator<FormSubmission>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionValidator"/> class.
        /// </summary>
        public FormSubmissionValidator()
        {
            this.RuleFor(submission => submission.FormSubmissionId).GreaterThan(0).When(submission => submission.FormSubmissionId.HasValue);
            this.RuleFor(submission => submission.Name).NotEmpty();
            this.RuleFor(submission => submission.SubmittedBy).NotEmpty();
        }
    }
}