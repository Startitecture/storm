namespace SAF.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.StringResources;

    /// <summary>
    /// Creates validation messages based on <see cref="ValidationRequest&lt;T&gt;"/>s.
    /// </summary>
    public static class ValidationMessageFactory
    {
        /// <summary>
        /// Returns a validation message failure if the property is not set.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The property to validate.</param>
        /// <returns>
        /// A message that the property failed validation if it is null or white space; otherwise, null.
        /// </returns>
        public static string ForValidation(string propertyName, string property)
        {
            if (String.IsNullOrEmpty(property))
            {
                return String.Format(ValidationMessages.PropertyMustBeSet, propertyName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a validation message failure if the property is set to its default value.
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The property to validate.</param>
        /// <returns>
        /// A message that the property failed validation if it is set to its default value; otherwise, null.
        /// </returns>
        public static string ForValidation<T>(string propertyName, T property)
        {
            if (Evaluate.IsDefaultValue(property))
            {
                return String.Format(ValidationMessages.PropertyMustBeSet, propertyName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a validation failure message if the predicate doesn't match.
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The property to validate.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="description">A description of the validation check.</param>
        /// <returns>
        /// A message that the property failed validation due to matching the predicate, otherwise, null.
        /// </returns>
        public static string ForValidation<T>(string propertyName, T property, Func<T, bool> predicate, string description)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (predicate(property))
            {
                return null;
            }
            else
            {
                return String.Format(ValidationMessages.PropertyValidationFailed, propertyName, property, description);
            }
        }

        /// <summary>
        /// Adds a message to an existing collection of messages if the specified validation request fails.
        /// </summary>
        /// <typeparam name="T">The type of item being validated.</typeparam>
        /// <param name="request">The validation request.</param>
        /// <param name="invalidStates">The states which should produce a validation message.</param>
        /// <returns>A validation message based on the specified validation request.</returns>
        public static string ForValidation<T>(ValidationRequest<T> request, params ValidationState[] invalidStates)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (invalidStates == null)
            {
                throw new ArgumentNullException("invalidStates");
            }

            ValidationResult<T> result = Evaluate.WithValidationRequest(request);

            if (invalidStates.Contains(result.ValueState))
            {
                return ForValidation<T>(result, invalidStates);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a mesage from the result of a validation.
        /// </summary>
        /// <typeparam name="T">The type of item to validate.</typeparam>
        /// <param name="result">The result of the validation.</param>
        /// <param name="invalidStates">The validation results that indicate an error.</param>
        /// <returns>A string containing the validation message.</returns>
        public static string ForValidation<T>(ValidationResult<T> result, params ValidationState[] invalidStates)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (invalidStates == null)
            {
                throw new ArgumentNullException("invalidStates");
            }

            string message;

            switch (result.ValueState)
            {
                case ValidationState.Null:
                    message = String.Format(ValidationMessages.PropertyMustBeSet, result.Request.PropertyName);
                    break;
                case ValidationState.Empty:

                    if (invalidStates.Contains(ValidationState.Null))
                    {
                        message = String.Format(ValidationMessages.PropertyMustBeSet, result.Request.PropertyName);
                    }
                    else
                    {
                        message = String.Format(ValidationMessages.UnsetPropertyShouldBeNull, result.Request.PropertyName);
                    }

                    break;
                case ValidationState.InvalidFormat:
                    message =
                        String.Format(
                            ValidationMessages.PropertyValueIsInvalidFormat,
                            result.Request.PropertyName,
                            result.Request.PropertyValue);

                    break;
                case ValidationState.LessThanMinRange:
                    message =
                        String.Format(
                            ValidationMessages.PropertyValueOutOfRange,
                            result.Request.PropertyName,
                            result.Request.PropertyValue,
                            result.Request.MinValue,
                            result.Request.MaxValue);

                    break;
                case ValidationState.GreaterThanMaxRange:
                    message =
                        String.Format(
                            ValidationMessages.PropertyValueOutOfRange,
                            result.Request.PropertyName,
                            result.Request.PropertyValue,
                            result.Request.MinValue,
                            result.Request.MaxValue);

                    break;

                case ValidationState.Valid:
                    message = null;
                    break;

                default:
                    message = String.Format(
                        ValidationMessages.UnknownValidationState, result.Request.PropertyName, result.Request.PropertyValue);

                    break;
            }

            return message;
        }

        /// <summary>
        /// Adds a validation failure message to the collection if the property isn't set.
        /// </summary>
        /// <typeparam name="T">The type of item to validate.</typeparam>
        /// <param name="messages">The existing collection of messages.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The property to validate.</param>
        public static void AddMessage<T>(ICollection<string> messages, string propertyName, T property)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            string message = ForValidation(propertyName, property);

            if (!String.IsNullOrEmpty(message))
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// Adds a validation failure message to the collection if the predicate doesn't match.
        /// </summary>
        /// <typeparam name="T">The type of item to validate.</typeparam>
        /// <param name="messages">The existing collection of messages.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The property to validate.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="description">A description of the validation check.</param>
        public static void AddMessage<T>(
            ICollection<string> messages,
            string propertyName,
            T property,
            Func<T, bool> predicate,
            string description)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            string message = ForValidation(propertyName, property, predicate, description);

            if (message != null)
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// Adds a validation failure message to the collection if the validation operation fails. 
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="messages">The existing collection of messages.</param>
        /// <param name="request">The validation request, which will be executed to determine the message.</param>
        /// <param name="invalidStates">The states that indicate validation has failed.</param>
        public static void AddMessage<T>(
            ICollection<string> messages,
            ValidationRequest<T> request,
            params ValidationState[] invalidStates)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            string message = ForValidation(request, invalidStates);

            if (message != null)
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// Adds a validation failure message to the collection if the validation operation has failed. 
        /// </summary>
        /// <typeparam name="T">The type of property to validate.</typeparam>
        /// <param name="messages">The existing collection of messages.</param>
        /// <param name="result">The results of a previously executed validation..</param>
        /// <param name="invalidStates">The states that indicate validation has failed.</param>
        public static void AddMessage<T>(
            ICollection<string> messages,
            ValidationResult<T> result,
            params ValidationState[] invalidStates)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            string message = ForValidation(result, invalidStates);

            if (message != null)
            {
                messages.Add(message);
            }
        }
    }
}
