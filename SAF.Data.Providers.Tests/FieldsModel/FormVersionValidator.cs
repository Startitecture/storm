// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersionValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form version validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The form version validator.
    /// </summary>
    public class FormVersionValidator : AbstractValidator<FormVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersionValidator"/> class.
        /// </summary>
        public FormVersionValidator()
        {
            this.RuleFor(version => version.FormVersionId).GreaterThan(0).When(version => version.FormVersionId.HasValue);
            this.RuleFor(version => version.FormId).NotNull().GreaterThan(0);
            this.RuleFor(version => version.Title).NotEmpty();
            this.RuleFor(version => version.Revision).GreaterThan(0);
        }
    }
}