// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValue.cs" company="Startitecture">
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
                item => item.LastModifiedPerson,
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
        /// <param name="formSubmissionId">
        /// The form submission ID.
        /// </param>
        /// <param name="unifiedFieldValueId">
        /// The unified field value ID.
        /// </param>
        public FormSubmissionValue(UnifiedField unifiedField, long formSubmissionId, long? unifiedFieldValueId)
            : base(unifiedField, unifiedFieldValueId)
        {
            this.FormSubmissionId = formSubmissionId;
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
        /// Gets the form submission ID.
        /// </summary>
        public long? FormSubmissionId { get; private set; }

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
            return Evaluate.Equals(this, other, ComparisonProperties) && this.FieldValues.SequenceEqual(other.FieldValues);
        }

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

            this.FormSubmissionId = submission.FormSubmissionId;
        }
    }
}