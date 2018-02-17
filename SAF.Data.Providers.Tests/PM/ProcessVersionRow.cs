// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessVersionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The process version row.
    /// </summary>
    public partial class ProcessVersionRow : ICompositeEntity, IEquatable<ProcessVersionRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ProcessVersionRow, object>[] ComparisonProperties =
            {
                item => item.Process,
                item => item.ProcessId,
                item => item.UseProcessActivities,
                item => item.VersionNumber
            };

        /// <summary>
        /// The process version relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> ProcessVersionRelations =
            new TransactSqlFromClause<ProcessVersionRow>()
                .InnerJoin(row => row.ProcessId, row => row.Process.ProcessId)
                .Relations;

        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        [Relation]
        public ProcessRow Process { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return ProcessVersionRelations;
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
        public static bool operator ==(ProcessVersionRow valueA, ProcessVersionRow valueB)
        {
            return EqualityComparer<ProcessVersionRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ProcessVersionRow valueA, ProcessVersionRow valueB)
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
            return base.ToString();
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
        public bool Equals(ProcessVersionRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

    }
}