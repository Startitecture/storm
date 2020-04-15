// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmission.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The generic submission.
    /// </summary>
    public class GenericSubmission
    {
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
        /// The submission values.
        /// </summary>
        public IEnumerable<FieldValue> SubmissionValues => this.submissionValues;

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
            this.SubmittedTime = DateTimeOffset.Now;
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
        public void Load([NotNull] List<FieldValue> fieldValues)
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