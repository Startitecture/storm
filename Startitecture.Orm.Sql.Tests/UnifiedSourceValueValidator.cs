// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedSourceValueValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified source value validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The unified source value validator.
    /// </summary>
    public class UnifiedSourceValueValidator : AbstractValidator<UnifiedSourceValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedSourceValueValidator"/> class.
        /// </summary>
        public UnifiedSourceValueValidator()
        {
            this.RuleFor(value => value.UnifiedSourceValueId).GreaterThan(0).When(value => value.UnifiedSourceValueId.HasValue);
            this.RuleFor(value => value.Name).NotEmpty();
            this.RuleFor(value => value.StringValue).NotEmpty();
        }
    }
}