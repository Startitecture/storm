// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="">
//   
// </copyright>
// <summary>
//   The extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using FluentValidation;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks the specified <paramref name="entity"/> for validation errors and throws a <see cref="BusinessException"/>
        /// if the check fails.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity to check.
        /// </typeparam>
        /// <param name="validator">
        /// The validator to use to validate the entity.
        /// </param>
        /// <param name="entity">
        /// The entity to check.
        /// </param>
        /// <exception cref="BusinessException">
        /// The entity had one or more validation errors.
        /// </exception>
        public static void ThrowOnValidationFailure<T>([NotNull] this AbstractValidator<T> validator, T entity)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var errors = GetValidationErrors(validator, entity);
            var entityErrors = errors as IList<string> ?? Enumerable.ToList<string>(errors);

            if (!entityErrors.Any())
            {
                return;
            }

            throw new BusinessException(entity, entityErrors);
        }

        /// <summary>
        /// Gets the validation errors for the specified model.
        /// </summary>
        /// <param name="validator">
        /// The validator to use to validate the model.
        /// </param>
        /// <param name="model">
        /// The model to validate.
        /// </param>
        /// <typeparam name="T">
        /// The type of model to validate.
        /// </typeparam>
        /// <returns>
        /// A collection of formatted validation errors.
        /// </returns>
        public static IEnumerable<string> GetValidationErrors<T>(this AbstractValidator<T> validator, T model)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            var result = validator.Validate(model);
            return from e in result.Errors
                   select String.Format(ValidationMessages.ItemValidationFailed, e.PropertyName, e.AttemptedValue, e.ErrorMessage);
        }
    }
}