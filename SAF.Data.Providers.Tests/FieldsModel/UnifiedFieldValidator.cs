// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified field validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using FluentValidation;

    /// <summary>
    /// The unified field validator.
    /// </summary>
    public class UnifiedFieldValidator : AbstractValidator<UnifiedField>
    {
        /// <summary>
        /// The source type namespaces.
        /// </summary>
        private readonly List<string> sourceTypeNamespaces = new List<string> { typeof(Contract).Namespace, typeof(Person).Namespace };

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldValidator"/> class.
        /// </summary>
        public UnifiedFieldValidator()
        {
            this.RuleFor(field => field.UnifiedFieldId).GreaterThan(0).When(field => field.UnifiedFieldId.HasValue);
            this.RuleFor(field => field.Name).NotEmpty();
            this.RuleFor(field => field.UnifiedFieldType).NotEqual(UnifiedFieldType.Unknown);
            this.RuleFor(field => field.UnifiedFieldType)
                .Equal(UnifiedFieldType.DatePicker)
                .When(field => field.UnifiedValueType == UnifiedValueType.Date);

            this.RuleFor(field => field.UnifiedValueType)
                .Equal(UnifiedValueType.Date)
                .When(field => field.UnifiedFieldType == UnifiedFieldType.DatePicker);

            this.RuleFor(field => field.UnifiedValueType).NotEqual(UnifiedValueType.Unknown);
            this.RuleFor(field => field.SourceType)
                .Must(s => this.sourceTypeNamespaces.Any(s.StartsWith))
                .WithMessage(FieldsMessages.UnifiedFieldSourceTypeNotSupported);
        }
    }
}