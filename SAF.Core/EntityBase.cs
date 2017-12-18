namespace SAF.Core
{
    using System;
    using System.Collections.Generic;

    using SAF.StringResources;

    /// <summary>
    /// Base class for all data entities supporting self-validation.
    /// </summary>
    public abstract class EntityBase : IValidatingEntity
    {
        /// <summary>
        /// A list of validation errors for the current entity.
        /// </summary>
        private List<string> errorList = new List<string>();

        /// <summary>
        /// Returns a list of validation errors for the current entity.
        /// </summary>
        /// <returns>
        /// A list of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /// </returns>
        public IEnumerable<string> Validate()
        {
            this.errorList.Clear();
            this.RunValidationChecks();
            return this.errorList;
        }

        /// <summary>
        /// Validates the current entity.
        /// </summary>
        protected abstract void RunValidationChecks();

        /// <summary>
        /// Appends validation errors for the specified entity to the base class's error list.
        /// </summary>
        /// <param name="entity">The <see cref="IValidatingEntity"/> to append validation errors from.</param>
        protected void AppendErrors(IValidatingEntity entity)
        {
            if (entity != null)
            {
                this.errorList.AddRange(entity.Validate());
            }
        }

        /// <summary>
        /// Appends an error message to a string collection if a validation check fails.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="entity">The entity to check.</param>
        /// <param name="validation">The validation to perform.</param>
        /// <param name="reason">The reason the validation failed.</param>
        protected void RunValidationCheck<T>(T entity, Func<T, bool> validation, string reason)
            where T : EntityBase
        {
            if (validation == null)
            {
                throw new ArgumentNullException("validation");
            }

            if (!validation(entity))
            {
                this.errorList.Add(String.Format(ValidationMessages.EntityValidationCheckFailed, typeof(T).Name, entity, reason));
            }
        }
    }
}
