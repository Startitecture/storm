// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Attachment.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Startitecture.Core;

    /// <summary>
    /// The attachment.
    /// </summary>
    public class Attachment : IEquatable<Attachment>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Attachment, object>[] ComparisonProperties =
            {
                item => item.Subject,
                item => item.DocumentType,
                item => item.SortOrder,
                item => item.CreatedBy,
                item => item.CreatedTime,
                item => item.LastModifiedBy,
                item => item.LastModifiedTime
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        public Attachment(string subject, DocumentType documentType)
            : this(subject, documentType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        /// <param name="attachmentId">
        /// The attachment id.
        /// </param>
        public Attachment(string subject, DocumentType documentType, long? attachmentId)
        {
            this.DocumentType = documentType;
            this.Subject = subject;
            this.AttachmentId = attachmentId;
            this.UpdateChangeTracking();
        }

        /// <summary>
        /// Gets the attachment id.
        /// </summary>
        public long? AttachmentId { get; private set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Gets the document type id.
        /// </summary>
        public DocumentType DocumentType { get; private set; }

        /// <summary>
        /// Gets the document type id.
        /// </summary>
        public int? DocumentTypeId
        {
            get
            {
                return this.DocumentType?.DocumentTypeId;
            }
        }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Gets the created by.
        /// </summary>
        public string CreatedBy { get; private set; }

        /// <summary>
        /// Gets the created time.
        /// </summary>
        public DateTimeOffset CreatedTime { get; private set; }

        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        public string LastModifiedBy { get; private set; }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTimeOffset LastModifiedTime { get; private set; }

        #region Equality Methods and Operators

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
        public static bool operator ==(Attachment valueA, Attachment valueB)
        {
            return EqualityComparer<Attachment>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(Attachment valueA, Attachment valueB)
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
            return this.Subject;
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
        public bool Equals(Attachment other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Changes the sort order for the attachment.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public void ChangeSortOrder(int order)
        {
            this.SortOrder = order;
            this.UpdateChangeTracking();
        }

        /// <summary>
        /// Sets the subject for the current attachment.
        /// </summary>
        /// <param name="subject">
        /// The subject to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subject"/> is null, empty or whitespace.
        /// </exception>
        public void SetSubject(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            this.Subject = subject;
            this.UpdateChangeTracking();
        }

        /// <summary>
        /// Updates change tracking for this attachment.
        /// </summary>
        protected void UpdateChangeTracking()
        {
            var identityName = Thread.CurrentPrincipal.Identity.Name;
            var modifiedTime = DateTimeOffset.Now;

            if (string.IsNullOrWhiteSpace(this.CreatedBy))
            {
                this.CreatedBy = identityName;
                this.CreatedTime = modifiedTime;
            }

            this.LastModifiedBy = identityName;
            this.LastModifiedTime = modifiedTime;
        }
    }
}