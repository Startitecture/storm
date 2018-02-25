// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowSignatureOptionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.DocumentEntities.PM
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The workflow signature option row.
    /// </summary>
    public partial class WorkflowSignatureOptionRow : ICompositeEntity, IEquatable<WorkflowSignatureOptionRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<WorkflowSignatureOptionRow, object>[] ComparisonProperties =
            {
                item => item.WorkflowPhaseId,
                item => item.EnableSignedDocumentRecipients,
                item => item.SignatureOption,
                item => item.SignatureOptionId,
                item => item.WorkflowSignatureOptionId
            };

        /// <summary>
        /// The process phase relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> WorkflowSignatureOptionRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                new SqlFromClause<WorkflowSignatureOptionRow>()
                    .InnerJoin(row => row.SignatureOptionId, row => row.SignatureOption.SignatureOptionId)
                    .Relations);

        /// <summary>
        /// Gets or sets the signature option.
        /// </summary>
        [Relation]
        public SignatureOptionRow SignatureOption { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return WorkflowSignatureOptionRelations.Value;
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
        public static bool operator ==(WorkflowSignatureOptionRow valueA, WorkflowSignatureOptionRow valueB)
        {
            return EqualityComparer<WorkflowSignatureOptionRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(WorkflowSignatureOptionRow valueA, WorkflowSignatureOptionRow valueB)
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
            return Convert.ToString((int)this.SignatureOptionId);
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
        public bool Equals(WorkflowSignatureOptionRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}