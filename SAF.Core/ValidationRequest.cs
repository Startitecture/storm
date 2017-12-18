namespace SAF.Core
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a validation request for a specific property.
    /// </summary>
    /// <typeparam name="T">The type of property to validate.</typeparam>
    public sealed class ValidationRequest<T> ////: IValidationRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRequest&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="requestType">The type of request.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="minValue">The minimum value of the validated property.</param>
        /// <param name="maxValue">The maximum value of the validated property.</param>
        /// <param name="matches">A collection of regular expressions to match against.</param>
        internal ValidationRequest(
            ValidationRequestType requestType, 
            string propertyName, 
            T propertyValue, 
            T minValue, 
            T maxValue, 
            params Regex[] matches)
        {
            this.RequestType = requestType;
            this.PropertyName = propertyName;
            this.PropertyValue = propertyValue;
            this.Matches = new List<Regex>(matches);
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Gets the type of validation request.
        /// </summary>
        public ValidationRequestType RequestType { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public T PropertyValue { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="Regex"/> matches for the validation request.
        /// </summary>
        public ICollection<Regex> Matches { get; private set; }

        /// <summary>
        /// Gets the minimum allowed value, or the default value if no value was set.
        /// </summary>
        public T MinValue { get; private set; }

        /// <summary>
        /// Gets the maximum allowed value, or the default value if no value was set.
        /// </summary>
        public T MaxValue { get; private set; }

        /// <summary>
        /// Creates a request to validate that a value does not match a specified set of regular expressions.
        /// </summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="matches">The expressions to match.</param>
        /// <returns>A validation request for the specified value and non-matching regular expressions.</returns>
        internal static ValidationRequest<T> CreateForNonMatchingRegex(string name, T value, params Regex[] matches)
        {
            return new ValidationRequest<T>(ValidationRequestType.DoesNotMatchRegex, name, value, default(T), default(T), matches);
        }

        /// <summary>
        /// Creates a request to validate that a value matches a specified set of regular expressions.
        /// </summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="matches">The expressions to match.</param>
        /// <returns>A validation request for the specified value and matching regular expressions.</returns>
        internal static ValidationRequest<T> CreateForMatchingRegex(string name, T value, params Regex[] matches)
        {
            return new ValidationRequest<T>(ValidationRequestType.MatchesRegex, name, value, default(T), default(T), matches);
        }

        /// <summary>
        /// Creates a request to validate a value is within a specified range.
        /// </summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum acceptable value.</param>
        /// <param name="maxValue">The maximum acceptable value.</param>
        /// <returns>A validation request for the specified value and range.</returns>
        internal static ValidationRequest<T> CreateForRange(string name, T value, T minValue, T maxValue)
        {
            return new ValidationRequest<T>(ValidationRequestType.WithinRange, name, value, minValue, maxValue, new Regex[] { });
        }
    }
}
