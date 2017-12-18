// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmission.cs" company="">
//   
// </copyright>
// <summary>
//   The form submission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The form submission.
    /// </summary>
    public class FormSubmission : IEquatable<FormSubmission>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FormSubmission, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.SubmittedBy,
                item => item.SubmittedTime
            };

        /// <summary>
        /// The unified field values.
        /// </summary>
        private readonly List<FormSubmissionValue> formSubmissionValues = new List<FormSubmissionValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmission"/> class.
        /// </summary>
        /// ///
        public FormSubmission()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmission"/> class.
        /// </summary>
        /// <param name="submittedBy">
        /// The submitted By.
        /// </param>
        /// <param name="submittedTime">
        /// The submission time.
        /// </param>
        /// <param name="formSubmissionId">
        /// The form submission ID.
        /// </param>
        /// ///
        public FormSubmission(Person submittedBy, DateTimeOffset submittedTime, long? formSubmissionId)
        {
            this.FormSubmissionId = formSubmissionId;
            this.SubmittedBy = submittedBy;
            this.SubmittedTime = submittedTime;
            this.Name = $"{submittedBy} - {submittedTime}";
        }

        /// <summary>
        /// Gets the form submission ID.
        /// </summary>
        public long? FormSubmissionId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the submitted by person ID.
        /// </summary>
        public Person SubmittedBy { get; private set; }

        /// <summary>
        /// Gets the submitted time.
        /// </summary>
        public DateTimeOffset SubmittedTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current submission has been submitted.
        /// </summary>
        public bool IsSubmitted
        {
            get
            {
                return this.SubmittedTime > DateTimeOffset.MinValue;
            }
        }

        /// <summary>
        /// Gets the unified field values for the form submission.
        /// </summary>
        public IEnumerable<FormSubmissionValue> FormSubmissionValues
        {
            get
            {
                return this.formSubmissionValues;
            }
        }

        #region Equality and Comparison Methods and Operators

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(FormSubmission valueA, FormSubmission valueB)
        {
            return EqualityComparer<FormSubmission>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(FormSubmission valueA, FormSubmission valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(FormSubmission other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Loads the values into the submission using the specified <paramref name="valueService"/>.
        /// </summary>
        /// <param name="valueService">
        /// The value service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueService"/> is null.
        /// </exception>
        public void Load([NotNull] IFormSubmissionValueService valueService)
        {
            if (valueService == null)
            {
                throw new ArgumentNullException(nameof(valueService));
            }

            var values = valueService.GetValues(this);
            this.formSubmissionValues.Clear();
            this.formSubmissionValues.AddRange(values);
        }

        /// <summary>
        /// Saves a draft of the form by associating the unified field values.
        /// </summary>
        /// <param name="person">
        /// The person submitting the form.
        /// </param>
        /// <param name="values">
        /// The values to submit.
        /// </param>
        public void SaveDraft([NotNull] Person person, [NotNull] params FormSubmissionValue[] values)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (person.PersonId < 1)
            {
                throw new BusinessException(person, FieldsMessages.IdValueLessThanOne);
            }

            if (this.SubmittedTime > DateTimeOffset.Now)
            {
                throw new BusinessException(this, FieldsMessages.FormAlreadySubmitted);
            }

            // Only clear the values if there's something to clear. But either way, "submit" the existing values.
            if (values.Any())
            {
                this.formSubmissionValues.Clear();
                this.formSubmissionValues.AddRange(values);
            }

            foreach (var formSubmissionValue in this.formSubmissionValues)
            {
                formSubmissionValue.SubmitWith(this);
            }

            this.SubmittedBy = person;
        }

        /// <summary>
        /// Submits the form by associating the unified field values.
        /// </summary>
        /// <param name="person">
        /// The person submitting the form.
        /// </param>
        /// <param name="values">
        /// The values to submit.
        /// </param>
        public void Submit([NotNull] Person person, [NotNull] params FormSubmissionValue[] values)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.SaveDraft(person, values);
            this.SubmittedTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// Associates current form submission object with each value.
        /// </summary>
        public void RefreshValues()
        {
            foreach (var formSubmissionValue in this.formSubmissionValues)
            {
                formSubmissionValue.SubmitWith(this);
            }
        }
    }
}
