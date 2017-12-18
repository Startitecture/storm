// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSectionValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout section validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The layout section validator.
    /// </summary>
    public class LayoutSectionValidator : AbstractValidator<LayoutSection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSectionValidator"/> class.
        /// </summary>
        public LayoutSectionValidator()
        {
            this.RuleFor(section => section.LayoutSectionId).GreaterThan(0).When(section => section.LayoutSectionId.HasValue);
            this.RuleFor(section => section.Name).NotEmpty();
        }
    }
}