// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldValidator.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The unified field validator.
    /// </summary>
    public class UnifiedFieldValidator : AbstractValidator<UnifiedField>
    {
        /// <summary>
        /// The source type namespaces.
        /// </summary>
        private readonly List<string> sourceTypeNamespaces = new List<string> { typeof(Testing.Model.Contract).Namespace, typeof(Testing.Model.PM.Person).Namespace };

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