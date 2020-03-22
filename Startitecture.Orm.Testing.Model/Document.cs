// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Document.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a document in the system with one or more document versions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// Represents a document in the system with one or more document versions.
    /// </summary>
    public class Document : IEquatable<Document>, IComparable, IComparable<Document>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Document, object>[] ComparisonProperties = { item => item.Identifier };

        /// <summary>
        /// The document versions.
        /// </summary>
        private readonly SortedSet<DocumentVersion> documentVersions = new SortedSet<DocumentVersion>(DocumentVersionComparer.DocumentVersion);

        /// <summary>
        /// Initializes a new instance of the <see cref="Document" /> class.
        /// </summary>
        /// <param name="identifier">
        /// The document identifier.
        /// </param>
        public Document([NotNull] string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            this.Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document" /> class.
        /// </summary>
        /// <param name="identifier">
        /// The document identifier.
        /// </param>
        /// <param name="documentId">
        /// The document ID.
        /// </param>
        public Document(string identifier, int? documentId)
            : this(identifier)
        {
            this.DocumentId = documentId;
        }

        /// <summary>
        /// Gets the document id.
        /// </summary>
        public int? DocumentId { get; private set; }

        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Gets the current name of the document.
        /// </summary>
        public string Name
        {
            get
            {
                return this.CurrentVersion == null ? null : this.CurrentVersion.Name;
            }
        }

        /// <summary>
        /// Gets the document versions for the current document.
        /// </summary>
        public IEnumerable<DocumentVersion> DocumentVersions
        {
            get
            {
                return this.documentVersions;
            }
        }

        /// <summary>
        /// Gets the current document version.
        /// </summary>
        public DocumentVersion CurrentVersion
        {
            get
            {
                return this.documentVersions.LastOrDefault();
            }
        }

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
            return this.Identifier;
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
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Document other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Loads document versions into the current document.
        /// </summary>
        /// <param name="documentVersionService">
        /// A document version service to retrieve the versions.
        /// </param>
        public void Load([NotNull] IDocumentVersionService documentVersionService)
        {
            if (documentVersionService == null)
            {
                throw new ArgumentNullException(nameof(documentVersionService));
            }

            var versions = documentVersionService.GetAllVersions(this);
            this.documentVersions.Clear();

            foreach (var version in versions)
            {
                this.AddVersion(version);
            }
        }

        /// <summary>
        /// Adds a document version to the internal linked list.
        /// </summary>
        /// <param name="version">
        /// The document version to add.
        /// </param>
        public void AddVersion([NotNull] DocumentVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (this.documentVersions.Add(version) == false)
            {
                throw new BusinessException(version, "The document already contains this version.");
            }

            version.AssociateWith(this);
        }

        /// <summary>
        /// Revises the document, resulting in a new document version incremented from the most recent version.
        /// </summary>
        /// <returns>
        /// The new <see cref="DocumentVersion" /> for the current document.
        /// </returns>
        public DocumentVersion Revise()
        {
            return this.Revise(this.Name ?? this.Identifier);
        }

        /// <summary>
        /// Revises the document, resulting in a new document version incremented from the most recent version.
        /// </summary>
        /// <param name="name">
        /// The name of the new document version.
        /// </param>
        /// <returns>
        /// The new <see cref="DocumentVersion" /> for the current document.
        /// </returns>
        public DocumentVersion Revise(string name)
        {
            var versionNum = this.DocumentVersions.Any() ? this.DocumentVersions.Max(version => version.VersionNumber) + 1 : 1;
            var documentVersion = new DocumentVersion(this, versionNum) { Name = name };
            this.AddVersion(documentVersion);
            return documentVersion;
        }

        /// <summary>
        /// The document version comparer.
        /// </summary>
        private class DocumentVersionComparer : Comparer<DocumentVersion>
        {
            /// <summary>
            /// Gets a comparer that compares document versions by version number.
            /// </summary>
            public static DocumentVersionComparer DocumentVersion { get; } = new DocumentVersionComparer();

            /// <summary>
            /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
            /// indicating whether one object is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the
            /// following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />.Zero
            /// <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than
            /// <paramref name="y" />.
            /// </returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public override int Compare(DocumentVersion x, DocumentVersion y)
            {
                return Evaluate.Compare(x, y, version => version.VersionNumber);
            }
        }
    }
}
