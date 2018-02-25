// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValue.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form submission value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The form submission value.
    /// </summary>
    public class FormSubmissionValue : UnifiedFieldValue, IEquatable<FormSubmissionValue>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FormSubmissionValue, object>[] ComparisonProperties =
            {
                item => item.FormSubmissionId,
                item => item.UnifiedField,
                item => item.LastModifiedBy,
                item => item.LastModifiedTime
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        public FormSubmissionValue(UnifiedField unifiedField)
            : base(unifiedField)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        /// <param name="formSubmission">
        /// The form submission ID.
        /// </param>
        /// <param name="lastModifiedBy">
        /// The last modified by.
        /// </param>
        /// <param name="lastModifiedTime">
        /// The last modified time.
        /// </param>
        /// <param name="unifiedFieldValueId">
        /// The unified field value ID.
        /// </param>
        public FormSubmissionValue(
            [NotNull] UnifiedField unifiedField,
            [NotNull] FormSubmission formSubmission,
            [NotNull] PM.Person lastModifiedBy,
            DateTimeOffset lastModifiedTime,
            long? unifiedFieldValueId)
            : base(unifiedField, lastModifiedBy, lastModifiedTime, unifiedFieldValueId)
        {
            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            if (formSubmission == null)
            {
                throw new ArgumentNullException(nameof(formSubmission));
            }

            if (lastModifiedBy == null)
            {
                throw new ArgumentNullException(nameof(lastModifiedBy));
            }

            this.FormSubmission = formSubmission;
        }

        /// <summary>
        /// Gets the form submission value ID.
        /// </summary>
        public long? FormSubmissionValueId
        {
            get
            {
                return this.UnifiedFieldValueId;
            }
        }

        /// <summary>
        /// Gets the form submission.
        /// </summary>
        public FormSubmission FormSubmission { get; private set; }

        /// <summary>
        /// Gets the form submission ID.
        /// </summary>
        public long? FormSubmissionId
        {
            get
            {
                return this.FormSubmission?.FormSubmissionId;
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
        public static bool operator ==(FormSubmissionValue valueA, FormSubmissionValue valueB)
        {
            return EqualityComparer<FormSubmissionValue>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FormSubmissionValue valueA, FormSubmissionValue valueB)
        {
            return !(valueA == valueB);
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
            var valuesHash = Evaluate.GenerateHashCode(this.FieldValues);

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
        public bool Equals(FormSubmissionValue other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties) && this.FieldValues.SequenceEqual(other?.FieldValues ?? new List<object>());
        }

        #endregion

        /// <summary>
        /// Submits the value to the specified form.
        /// </summary>
        /// <param name="submission">
        /// The submission.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="submission"/> is null.
        /// </exception>
        internal void SubmitWith([NotNull] FormSubmission submission)
        {
            if (submission == null)
            {
                throw new ArgumentNullException(nameof(submission));
            }

            this.FormSubmission = submission;
        }
    }
}
