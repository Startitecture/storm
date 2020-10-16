// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmission.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The generic submission.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The generic submission.
    /// </summary>
    public class GenericSubmission : IEquatable<GenericSubmission>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<GenericSubmission, object>[] ComparisonProperties =
            {
                item => item.Subject,
                item => item.SubmittedBy,
                item => item.SubmittedTime,
                item => item.SubmissionValues
            };

        /// <summary>
        /// The submission values.
        /// </summary>
        private readonly List<FieldValue> submissionValues = new List<FieldValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSubmission"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="submittedBy">
        /// The submitted by identity.
        /// </param>
        public GenericSubmission(string subject, DomainIdentity submittedBy)
        {
            this.Subject = subject;
            this.SubmittedBy = submittedBy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSubmission"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="submittedBy">
        /// The submitted by identity.
        /// </param>
        /// <param name="genericSubmissionId">
        /// The generic Submission ID.
        /// </param>
        public GenericSubmission(string subject, DomainIdentity submittedBy, int genericSubmissionId)
            : this(subject, submittedBy)
        {
            this.GenericSubmissionId = genericSubmissionId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="GenericSubmission"/> class from being created.
        /// </summary>
        private GenericSubmission()
        {
        }

        /// <summary>
        /// Gets the generic submission id.
        /// </summary>
        public int? GenericSubmissionId { get; private set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Gets the submitted by.
        /// </summary>
        public DomainIdentity SubmittedBy { get; private set; }

        /// <summary>
        /// Gets the submission time.
        /// </summary>
        public DateTimeOffset SubmittedTime { get; private set; }

        /// <summary>
        /// Gets the submission values.
        /// </summary>
        public IEnumerable<FieldValue> SubmissionValues => this.submissionValues;

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
        public static bool operator ==(GenericSubmission valueA, GenericSubmission valueB)
        {
            return EqualityComparer<GenericSubmission>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(GenericSubmission valueA, GenericSubmission valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"'{this.Subject}' by {this.SubmittedBy} at {this.SubmittedTime}";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
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
        public bool Equals(GenericSubmission other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue([NotNull] Field field, object value)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            this.submissionValues.Add(new FieldValue(field).Set(value, this.SubmittedBy));
        }

        /// <summary>
        /// Finalizes the submission.
        /// </summary>
        public void Submit()
        {
            // Reducing resolution for the purposes of PostgreSQL compatibility.
            var now = DateTimeOffset.Now;
            this.SubmittedTime = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, now.Offset);
        }

        /// <summary>
        /// Loads existing values into the submission values collection.
        /// </summary>
        /// <param name="fieldValues">
        /// The field values to load.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldValues"/> is null.
        /// </exception>
        public void Load([NotNull] IEnumerable<FieldValue> fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            this.submissionValues.Clear();
            this.submissionValues.AddRange(fieldValues);
        }
    }
}