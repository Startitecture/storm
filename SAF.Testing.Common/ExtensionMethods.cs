// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="">
//   
// </copyright>
// <summary>
//   The extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using FluentValidation;

    using JetBrains.Annotations;

    using SAF.StringResources;

    using Startitecture.Core;

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

            var errors = validator.GetValidationErrors(entity);
            var entityErrors = errors as IList<string> ?? errors.ToList();

            if (!entityErrors.Any())
            {
                return;
            }

            throw new BusinessException(entity, entityErrors);
        }

        /// <summary>
        /// Sets the property value for the specified property.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="itemProperty">
        /// The item property.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <typeparam name="T">
        /// The type of the item to set a property on.
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent use of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Allows fluent use of the method.")]
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> itemProperty, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (itemProperty == null)
            {
                throw new ArgumentNullException(nameof(itemProperty));
            }

            MemberExpression memberSelectorExpression;

            if (itemProperty.Body.NodeType == ExpressionType.Convert)
            {
                if (!(itemProperty.Body is UnaryExpression unaryExpression))
                {
                    throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
                }

                memberSelectorExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberSelectorExpression = itemProperty.Body as MemberExpression;
            }

            if (memberSelectorExpression == null)
            {
                throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
            }

            var property = memberSelectorExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new BusinessException(itemProperty, ValidationMessages.SelectorCannotBeEvaluated);
            }

            property.SetValue(target, value, null);
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
                   select string.Format(ValidationMessages.ItemValidationFailed, e.PropertyName, e.AttemptedValue, e.ErrorMessage);
        }

        /// <summary>
        /// Gets the property differences between two objects of the same type.
        /// </summary>
        /// <param name="baseline">
        /// The baseline object.
        /// </param>
        /// <param name="comparison">
        /// The comparison object.
        /// </param>
        /// <param name="propertiesToCompare">
        /// The properties to compare.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to compare.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="Startitecture.Core.PropertyComparisonResult"/> items containing the non-equivalent property values of the two 
        /// items.
        /// </returns>
        public static IEnumerable<PropertyComparisonResult> GetDifferences<TItem>(
            this TItem baseline, 
            TItem comparison, 
            params string[] propertiesToCompare)
        {
            if (Evaluate.IsNull(baseline))
            {
                throw new ArgumentNullException(nameof(baseline));
            }

            if (Evaluate.IsNull(comparison))
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            if (Evaluate.IsNull(propertiesToCompare))
            {
                throw new ArgumentNullException(nameof(propertiesToCompare));
            }

            var allProperties = GetAllProperties<TItem>(propertiesToCompare);

            var originalProperties = allProperties.ToDictionary(info => info.Name, info => info.GetPropertyValue(baseline));

            var newProperties = allProperties.ToDictionary(info => info.Name, info => info.GetPropertyValue(comparison));

            return (from propertyName in allProperties.Select(x => x.Name)
                    let originalValue = originalProperties[propertyName]
                    let newValue = newProperties[propertyName]
                    where !ReferenceEquals(originalValue, newValue)
                    where (originalValue != null && !originalValue.Equals(newValue)) || !newValue.Equals(originalValue)
                    select
                        new PropertyComparisonResult
                            {
                                PropertyName = propertyName, 
                                OriginalValue = originalValue, 
                                NewValue = newValue
                            }).ToList();
        }

        /// <summary>
        /// Gets all of the properties for the array of string properties to compare.
        /// </summary>
        /// <param name="propertiesToCompare">
        /// The properties to compare.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to evaluate.
        /// </typeparam>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="PropertyInfo"/> items matching the <paramref name="propertiesToCompare"/>.
        /// </returns>
        private static List<PropertyInfo> GetAllProperties<TItem>(string[] propertiesToCompare)
        {
            var allProperties = propertiesToCompare.Any()
                                    ? typeof(TItem).GetProperties().Where(x => propertiesToCompare.Contains(x.Name) && !x.GetIndexParameters().Any())
                                        .OrderBy(x => x.Name).ToList()
                                    : typeof(TItem).GetProperties().Where(x => !x.GetIndexParameters().Any()).OrderBy(x => x.Name).ToList();
            return allProperties;
        }
    }
}