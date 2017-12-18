// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonValidator.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The person validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using FluentValidation;

    /// <summary>
    /// The person validator.
    /// </summary>
    public class PersonValidator : AbstractValidator<Person>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonValidator"/> class.
        /// </summary>
        public PersonValidator()
        {
            this.RuleFor(person => person.PersonId).GreaterThan(0).When(person => person.PersonId.HasValue);
            this.RuleFor(person => person.FirstName).NotEmpty();
            this.RuleFor(person => person.LastName).NotEmpty();
        }
    }
}