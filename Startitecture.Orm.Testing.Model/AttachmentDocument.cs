// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocument.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// The document attachment.
    /// </summary>
    public class AttachmentDocument : Attachment, IEquatable<AttachmentDocument>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<AttachmentDocument, object>[] ComparisonProperties =
            {
                item => item.DocumentVersion
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentDocument"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="documentVersion">
        /// The document version.
        /// </param>
        /// <param name="documentType">
        /// The document type id.
        /// </param>
        public AttachmentDocument(string subject, DocumentVersion documentVersion, DocumentType documentType)
            : this(subject, documentVersion, documentType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentDocument"/> class.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="documentVersion">
        /// The document version.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        /// <param name="attachmentId">
        /// The attachment id.
        /// </param>
        public AttachmentDocument(string subject, DocumentVersion documentVersion, DocumentType documentType, long? attachmentId)
            : base(subject, documentType, attachmentId)
        {
            this.DocumentVersion = documentVersion;
        }

        /// <summary>
        /// Gets the attachment document id.
        /// </summary>
        public long? AttachmentDocumentId
        {
            get
            {
                return this.AttachmentId;
            }
        }

        /// <summary>
        /// Gets the document version.
        /// </summary>
        public DocumentVersion DocumentVersion { get; private set; }

        /// <summary>
        /// Gets the document version ID.
        /// </summary>
        public long? DocumentVersionId
        {
            get
            {
                return this.DocumentVersion?.DocumentVersionId;
            }
        }

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
        public static bool operator ==(AttachmentDocument valueA, AttachmentDocument valueB)
        {
            return EqualityComparer<AttachmentDocument>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(AttachmentDocument valueA, AttachmentDocument valueB)
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
            return $"{base.ToString()}: {this.DocumentVersion}";
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
        public bool Equals(AttachmentDocument other)
        {
            return base.Equals(other) && Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}