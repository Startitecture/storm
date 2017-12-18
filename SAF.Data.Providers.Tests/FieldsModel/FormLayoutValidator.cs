// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form layout validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The form layout validator.
    /// </summary>
    public class FormLayoutValidator : AbstractValidator<FormLayout>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayoutValidator"/> class.
        /// </summary>
        public FormLayoutValidator()
        {
            this.RuleFor(layout => layout.FormLayoutId).GreaterThan(0).When(layout => layout.FormLayoutId.HasValue);
            this.RuleFor(layout => layout.FormVersionId).GreaterThan(0);
            this.RuleFor(layout => layout.Name).NotEmpty();
        }
    }
}