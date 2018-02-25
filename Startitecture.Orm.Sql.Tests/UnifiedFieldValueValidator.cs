// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldValueValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field value validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;

    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The unified field value validator.
    /// </summary>
    public class UnifiedFieldValueValidator : AbstractValidator<UnifiedFieldValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldValueValidator"/> class.
        /// </summary>
        public UnifiedFieldValueValidator()
        {
            this.RuleFor(value => value.UnifiedFieldId).GreaterThan(0).When(value => value.UnifiedFieldId.HasValue);
            this.RuleFor(value => value.LastModifiedBy).NotEmpty();
            this.RuleFor(value => value.LastModifiedPersonId).NotNull().GreaterThan(0);
            this.RuleFor(value => value.LastModifiedTime).GreaterThan(DateTimeOffset.MinValue);
        }
    }
}