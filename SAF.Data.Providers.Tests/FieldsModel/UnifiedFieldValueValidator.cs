// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldValueValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified field value validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;

    using FluentValidation;

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