// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The form validator.
    /// </summary>
    public class FormValidator : AbstractValidator<Form>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormValidator"/> class.
        /// </summary>
        public FormValidator()
        {
            this.RuleFor(form => form.FormId).GreaterThan(0).When(form => form.FormId.HasValue);
            this.RuleFor(form => form.Name).NotEmpty();
        }
    }
}