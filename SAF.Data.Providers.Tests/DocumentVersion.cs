// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersion.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A specific version of a document.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// A specific version of a document.
    /// </summary>
    public class DocumentVersion : IEquatable<DocumentVersion>, IComparable, IComparable<DocumentVersion>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DocumentVersion, object>[] ComparisonProperties =
            {
                item => item.Document,
                item => item.VersionNumber,
                item => item.Name
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersion"/> class.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="versionNumber">
        /// The version number.
        /// </param>
        public DocumentVersion(Document document, int versionNumber)
        {
            this.Document = document;
            this.VersionNumber = versionNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersion"/> class.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="versionNumber">
        /// The version.
        /// </param>
        /// <param name="documentVersionId">
        /// The id.
        /// </param>
        public DocumentVersion(Document document, int versionNumber, long documentVersionId)
        {
            this.Document = document;
            this.DocumentVersionId = documentVersionId;
            this.VersionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the document version id.
        /// </summary>
        public long? DocumentVersionId { get; private set; }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public Document Document { get; private set; }

        /// <summary>
        /// Gets the document id.
        /// </summary>
        public int? DocumentId
        {
            get
            {
                return this.Document?.DocumentId;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public int VersionNumber { get; private set; }

        /// <summary>
        /// Gets or sets the version name.
        /// </summary>
        public string Name { get; set; }

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
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal
        /// to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
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
            return $"{this.Name} ({this.Document?.Identifier} version {this.VersionNumber})";
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
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the
        /// current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        /// Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in
        /// the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows
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
        /// Associates the current document version with the specified document.
        /// </summary>
        /// <param name="document">
        /// The document to associate with.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="document"/> is null.
        /// </exception>
        internal void AssociateWith([NotNull] Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            this.Document = document;
        }
    }
}
