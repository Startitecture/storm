// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormSubmissionValueService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormSubmissionValueService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.Collections.Generic;

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
