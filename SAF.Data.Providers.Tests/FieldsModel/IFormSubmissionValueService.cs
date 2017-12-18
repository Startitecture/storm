using System.Collections.Generic;

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The FormSubmissionValueService interface.
    /// </summary>
    public interface IFormSubmissionValueService
    {
        /// <summary>
        /// Gets the values for a specific form submission.
        /// </summary>
        /// <param name="submission">
        /// The form submission to retrieve the values for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of the <see cref="FormSubmissionValue"/> items for the specified 
        /// <paramref name="submission"/>.
        /// </returns>
        IEnumerable<FormSubmissionValue> GetValues(FormSubmission submission);
    }
}
