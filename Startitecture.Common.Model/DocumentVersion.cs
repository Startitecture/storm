// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersion.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SAF.StringResources;

    using Startitecture.Core;

    /// <summary>
    /// The document version.
    /// </summary>
    public class DocumentVersion : IEquatable<DocumentVersion>, IComparable, IComparable<DocumentVersion>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DocumentVersion, object>[] ComparisonProperties =
            {
                item => item.Document,
                item => item.Revision,
                item => item.Uri,
                item => item.RevisionTime,
                item => item.FileName
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersion"/> class.
        /// </summary>
        public DocumentVersion()
        {
            this.Uri = new Uri($"{Path.GetTempPath()}{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersion"/> class.
        /// </summary>
        /// <param name="documentVersionId">
        /// The document version ID.
        /// </param>
        public DocumentVersion(int? documentVersionId)
        {
            this.DocumentVersionId = documentVersionId;
        }

        /// <summary>
        /// Gets the document version ID.
        /// </summary>
        public int? DocumentVersionId { get; private set; }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public Document Document { get; private set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the uri.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// Gets the revision time.
        /// </summary>
        public DateTimeOffset RevisionTime { get; private set; }

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
        public static bool operator ==(DocumentVersion valueA, DocumentVersion valueB)
        {
            return EqualityComparer<DocumentVersion>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(DocumentVersion valueA, DocumentVersion valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(DocumentVersion valueA, DocumentVersion valueB)
        {
            return Comparer<DocumentVersion>.Default.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(DocumentVersion valueA, DocumentVersion valueB)
        {
            return Comparer<DocumentVersion>.Default.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance
        /// occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows
        /// <paramref name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This
        /// object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(DocumentVersion other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
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
            return this.Uri.ToString();
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
        public bool Equals(DocumentVersion other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

       /// <summary>
        /// Adds the current document version to the specified <paramref name="document"/>.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="document"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is null or whitespace.
        /// </exception>
        internal void AddToDocument(Document document, string fileName)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(fileName));
            }

            this.Document = document;
            this.Uri = new Uri($"{document.Container}{Path.AltDirectorySeparatorChar}{fileName}");
            this.Revision = document.CurrentVersion?.Revision ?? 0 + 1;
            this.RevisionTime = DateTimeOffset.Now;
            this.FileName = fileName;
        }
    }
}