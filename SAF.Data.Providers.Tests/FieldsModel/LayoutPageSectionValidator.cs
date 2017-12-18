// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page section validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The layout page section validator.
    /// </summary>
    public class LayoutPageSectionValidator : AbstractValidator<LayoutPageSection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSectionValidator"/> class.
        /// </summary>
        public LayoutPageSectionValidator()
        {
            this.RuleFor(section => section.LayoutPageSectionId).GreaterThan(0).When(section => section.LayoutPageSectionId.HasValue);
            this.RuleFor(section => section.LayoutPage).NotEmpty();
            this.RuleFor(section => section.LayoutPageId).NotNull().GreaterThan(0);
            this.RuleFor(section => section.LayoutSection).NotEmpty();
            this.RuleFor(section => section.LayoutSectionId).NotNull().GreaterThan(0);
            this.RuleFor(section => section.Order).GreaterThan((short)0);
        }
    }
}