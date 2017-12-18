// -----------------------------------------------------------------------
// <copyright file="ValidationRequestFactory.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.Core
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Creates validation requests for the specified criteria.
    /// </summary>
    public static class ValidationRequestFactory
    {
        /// <summary>
        /// Generates matching validation request for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="matches">A <see cref="Regex"/> collection containing all valid matches.</param>
        /// <returns>The result of the validation request.</returns>
        public static ValidationRequest<T> ForMatches<T>(string propertyName, T value, params Regex[] matches)
        {
            return ValidationRequest<T>.CreateForMatchingRegex(propertyName, value, matches);
        }

        /// <summary>
        /// Generates a non-matching validation request for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="matches">A <see cref="Regex"/> collection containing all invalid matches.</param>
        /// <returns>The result of the validation request.</returns>
        public static ValidationRequest<T> ForNonMatches<T>(string propertyName, T value, params Regex[] matches)
        {
            return ValidationRequest<T>.CreateForNonMatchingRegex(propertyName, value, matches);
        }

        /// <summary>
        /// Generates a range validation request for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="minValue">The minimum allowed value.</param>
        /// <param name="maxValue">The maximum allowed value.</param>
        /// <returns>The result of the validation request.</returns>
        public static ValidationRequest<T> ForRange<T>(string propertyName, T value, T minValue, T maxValue)
        {
            return ValidationRequest<T>.CreateForRange(propertyName, value, minValue, maxValue);
        }
    }
}
