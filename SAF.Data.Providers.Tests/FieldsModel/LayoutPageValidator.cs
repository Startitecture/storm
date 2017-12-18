// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The layout page validator.
    /// </summary>
    public class LayoutPageValidator : AbstractValidator<LayoutPage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageValidator"/> class.
        /// </summary>
        public LayoutPageValidator()
        {
            this.RuleFor(page => page.LayoutPageId).GreaterThan(0).When(page => page.LayoutPageId.HasValue);
            this.RuleFor(page => page.FormLayout).NotEmpty();
            this.RuleFor(page => page.Name).NotEmpty();
            this.RuleFor(page => page.Order).GreaterThan((short)0);
        }
    }
}