// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentNote.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The note attachment.
    /// </summary>
    public class AttachmentNote : Attachment, IEquatable<AttachmentNote>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<AttachmentNote, object>[] ComparisonProperties =
            {
                item => item.Content
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentNote"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject of the attachment note.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        public AttachmentNote(string subject, DocumentType documentType, string content)
            : this(subject, documentType, content, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentNote"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject of the attachment note.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="attachmentId">
        /// The attachment id.
        /// </param>
        public AttachmentNote(string subject, DocumentType documentType, [NotNull] string content, long? attachmentId)
            : base(subject, documentType, attachmentId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.Content = content;
        }

        /// <summary>
        /// Gets the attachment note ID.
        /// </summary>
        public long? AttachmentNoteId
        {
            get
            {
                return this.AttachmentId;
            }
        }

        /// <summary>
        /// Gets the content of the note.
        /// </summary>
        public string Content { get; private set; }

        #region Equality and Comparison Methods

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
        public static bool operator ==(AttachmentNote valueA, AttachmentNote valueB)
        {
            return EqualityComparer<AttachmentNote>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(AttachmentNote valueA, AttachmentNote valueB)
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
            var baseCode = base.GetHashCode();
            var thisCode = Evaluate.GenerateHashCode(this, ComparisonProperties);
            return Evaluate.GenerateHashCode(baseCode, thisCode);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
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
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AttachmentNote other)
        {
            return base.Equals(other) && Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Sets the content of the note.
        /// </summary>
        /// <param name="content">
        /// The content to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is null, empty or whitespace.
        /// </exception>
        public void SetContent([NotNull] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.Content = content;
            this.UpdateChangeTracking();
        }
    }
}