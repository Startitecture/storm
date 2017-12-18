namespace SAF.Core
{
    /// <summary>
    /// Contains the result of a validation.
    /// </summary>
    /// <typeparam name="T">The type of property that was validated.</typeparam>
    public class ValidationResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="request">The processed request.</param>
        /// <param name="valueState">The validation state of the item.</param>
        internal ValidationResult(ValidationRequest<T> request, ValidationState valueState)
        {
            this.Request = request;
            this.ValueState = valueState;
        }

        /// <summary>
        /// Gets the request for validation.
        /// </summary>
        public ValidationRequest<T> Request { get; private set; }

        /// <summary>
        /// Gets the validation state of the item.
        /// </summary>
        public ValidationState ValueState { get; private set; }
    }
}
