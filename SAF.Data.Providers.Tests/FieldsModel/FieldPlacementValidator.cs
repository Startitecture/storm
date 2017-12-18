// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The field placement validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    using SAF.Core;

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