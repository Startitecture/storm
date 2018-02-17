// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The field placement validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    using Startitecture.Core;

    /// <summary>
    /// The field placement validator.
    /// </summary>
    public class FieldPlacementValidator : AbstractValidator<FieldPlacement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacementValidator"/> class.
        /// </summary>
        public FieldPlacementValidator()
        {
            this.RuleFor(placement => placement.FieldPlacementId).GreaterThan(0).When(placement => placement.FieldPlacementId.HasValue);
            this.RuleFor(placement => placement.LayoutSection).SetValidator(Singleton<LayoutSectionValidator>.Instance);
        }
    }
}