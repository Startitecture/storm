// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmission.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

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
                item => item.ProcessFormId,
                item => item.SubmittedByPersonId,
                item => item.SubmittedTime
            };

        /// <summary>
        /// The unified field values.
        /// </summary>
        private readonly List<FormSubmissionValue> formSubmissionValues = new List<FormSubmissionValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmission"/> class.
        /// </summary>
        /// <param name="processFormId">
        /// The process form ID.
        /// </param>
        /// <param name="submittedByPersonId">
        /// The submitted by person ID.
        /// </param>
        /// ///
        public FormSubmission(int processFormId, int submittedByPersonId)
            : this(processFormId, submittedByPersonId, DateTimeOffset.MinValue, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmission"/> class.
        /// </summary>
        /// <param name="processFormId">
        /// The process form ID.
        /// </param>
        /// <param name="submittedByPersonId">
        /// The submitted by person ID.
        /// </param>
        /// <param name="submittedTime">
        /// The submission time.
        /// </param>
        /// <param name="formSubmissionId">
        /// The form submission ID.
        /// </param>
        /// ///
        public FormSubmission(int processFormId, int submittedByPersonId, DateTimeOffset submittedTime, long? formSubmissionId)
        {
            this.ProcessFormId = processFormId;
            this.FormSubmissionId = formSubmissionId;
            this.SubmittedByPersonId = submittedByPersonId;
            this.SubmittedTime = submittedTime;
            this.Name = $"{submittedByPersonId} - {processFormId} - {submittedTime}";
        }

        /// <summary>
        /// Gets the form submission ID.
        /// </summary>
        public long? FormSubmissionId { get; private set; }

        /// <summary>
        /// Gets the process form ID.
        /// </summary>
        public int ProcessFormId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the submitted by person ID.
        /// </summary>
        public int SubmittedByPersonId { get; private set; }

        /// <summary>
        /// Gets the submitted time.
        /// </summary>
        public DateTimeOffset SubmittedTime { get; private set; }

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
            var propertyHash = Evaluate.GenerateHashCode(this, ComparisonProperties);
            var valuesHash = Evaluate.GenerateHashCode(this.FormSubmissionValues.OrderBy(x => x.UnifiedField.Name));
            return Evaluate.GenerateHashCode(propertyHash, valuesHash);
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
            // Here, order of values isn't important and will likely not remain the same.
            return Evaluate.Equals(this, other, ComparisonProperties)
                   && Evaluate.CollectionEquals(this.FormSubmissionValues, other.FormSubmissionValues);
        }

        /// <summary>
        /// Loads unified field values into the current submission.
        /// </summary>
        /// <param name="values">
        /// The values to load.
        /// </param>
        public void Load(IEnumerable<FormSubmissionValue> values)
        {
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

            if (person.PersonId.HasValue == false)
            {
                throw new BusinessException(person, "Person has no ID set.");
            }

            if (this.SubmittedTime > DateTimeOffset.Now)
            {
                throw new BusinessException(this, "Form already submitted.");
            }

            // Only clear the values if there's something to clear. But either way, "submit" the existing values.
            if (values.Any())
            {
                this.formSubmissionValues.Clear();
                this.formSubmissionValues.AddRange(values);
            }

            foreach (var formSubmissionValue in this.formSubmissionValues)
            {
                formSubmissionValue.LastModifiedTime = DateTimeOffset.Now;
                formSubmissionValue.LastModifiedPerson = person;
                formSubmissionValue.SubmitWith(this);
            }

            this.SubmittedByPersonId = person.PersonId.GetValueOrDefault();
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