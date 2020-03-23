// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Document.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Common.Model
{
    using System;
    using System.Collections.Generic;

    using Core;

    using Startitecture.Resources;

    /// <summary>
    /// The document.
    /// </summary>
    public class Document : IEquatable<Document>, IComparable, IComparable<Document>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Document, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.Container,
                item => item.ResourceClassification
            };

        /// <summary>
        /// The document versions.
        /// </summary>
        private readonly SortedSet<DocumentVersion> documentVersions = new SortedSet<DocumentVersion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public Document(Container container)
            : this(container, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="documentId">
        /// The document ID.
        /// </param>
        public Document(Container container, int? documentId)
        {
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
            this.DocumentId = documentId;
        }

        /// <summary>
        /// Gets the DocumentID.
        /// </summary>
        public int? DocumentId { get; private set; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public Container Container { get; private set; }

        /// <summary>
        /// Gets or sets the ResourceClassificationID.
        /// </summary>
        public ResourceClassification ResourceClassification { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The document versions.
        /// </summary>
        public IEnumerable<DocumentVersion> DocumentVersions => this.documentVersions;

        /// <summary>
        /// The current version.
        /// </summary>
        public DocumentVersion CurrentVersion => this.documentVersions.Max;

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
        public static bool operator ==(Document valueA, Document valueB)
        {
            return EqualityComparer<Document>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(Document valueA, Document valueB)
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
        public static bool operator <(Document valueA, Document valueB)
        {
            return Comparer<Document>.Default.Compare(valueA, valueB) < 0;
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
        public static bool operator >(Document valueA, Document valueB)
        {
            return Comparer<Document>.Default.Compare(valueA, valueB) > 0;
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
        /// <exception cref="ArgumentException">
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
        public int CompareTo(Document other)
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
        public bool Equals(Document other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Revises the current document with the specified file name.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentVersion"/> created by the revision.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is null or white space.
        /// </exception>
        /// <exception cref="Startitecture.Core.OperationException">
        /// The new revision could not be added to the sorted set because such a revision already exists.
        /// </exception>
        public DocumentVersion Revise(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(fileName));
            }

            var documentVersion = new DocumentVersion();
            documentVersion.AddToDocument(this, fileName);

            if (this.documentVersions.Add(documentVersion))
            {
                return documentVersion;
            }

            throw new OperationException(documentVersion, ErrorMessages.ItemCouldNotBeAddedToCollection);
        }
    }
}