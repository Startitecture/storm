// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form submission validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

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