// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.DocumentEntities
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The attachment row.
    /// </summary>
    public partial class AttachmentRow : ICompositeEntity, IEquatable<AttachmentRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<AttachmentRow, object>[] ComparisonProperties =
            {
                item => item.CreatedBy,
                item => item.CreatedTime,
                item => item.DocumentType,
                item => item.DocumentTypeId,
                item => item.LastModifiedBy,
                item => item.LastModifiedTime,
                item => item.SortOrder,
                item => item.Subject
            };

        /// <summary>
        /// The attachment relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> AttachmentRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () => new SqlFromClause<AttachmentRow>()
                    .InnerJoin(row => row.DocumentTypeId, row => row.DocumentType.DocumentTypeId)
                    .Relations);

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        [Relation]
        public DocumentTypeRow DocumentType { get; set; }

        /// <summary>
        /// The entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations => AttachmentRelations.Value;

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
        public static bool operator ==(AttachmentRow valueA, AttachmentRow valueB)
        {
            return EqualityComparer<AttachmentRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(AttachmentRow valueA, AttachmentRow valueB)
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
        public bool Equals(AttachmentRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}