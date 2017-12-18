﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessPhaseRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The process phase row.
    /// </summary>
    public partial class ProcessPhaseRow : ICompositeEntity, IEquatable<ProcessPhaseRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ProcessPhaseRow, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.LegacyPhaseId,
                item => item.ParentProcessPhaseId,
                item => item.PhaseActionDeadline,
                item => item.PhaseTypeId,
                item => item.ProcessPhaseId,
                item => item.ProcessVersion,
                item => item.ProcessVersionId,
                item => item.SignatureOption,
                item => item.WorkflowOrder
            };

        /// <summary>
        /// The process phase relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> ProcessPhaseRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new TransactSqlFromClause<ProcessPhaseRow>()

                        // Here we directly select the IDs of the tables to join. The "raised" POCOs are evaluated to automatically 
                        // create a JOIN on the desired table and alias if necessary.
                        .InnerJoin(row => row.ProcessVersionId, row => row.ProcessVersion.ProcessVersionId)
                        .InnerJoin(row => row.ProcessVersion.ProcessId, row => row.ProcessVersion.Process.ProcessId)

                        // These next two rows are not "raised" on the POCO because they are intersection rows, so we use the old 
                        // approach to join them.
                        .InnerJoin<ViewLegacyPhaseRow>(row => row.ProcessPhaseId, row => row.ProcessPhaseId)
                        .LeftJoin<ProcessSubPhaseRow>(row => row.ProcessPhaseId, row => row.ProcessSubPhaseId)
                        .LeftJoin(row => row.ProcessPhaseId, row => row.PhaseActionDeadline.PhaseActionDeadlineId)
                        .LeftJoin(row => row.ProcessPhaseId, row => row.SignatureOption.SignatureOptionId)
                        .Relations);

        /// <summary>
        /// Gets or sets the LegacyWorkflowPhaseId.
        /// </summary>
        [RelatedEntity(typeof(ViewLegacyPhaseRow))]
        public int? LegacyPhaseId { get; set; }

        /// <summary>
        /// Gets or sets the phase action deadline.
        /// </summary>
        [Relation]
        public PhaseActionDeadlineRow PhaseActionDeadline { get; set; }

        /// <summary>
        /// Gets or sets the ParentProcessPhaseId.
        /// </summary>
        [RelatedEntity(typeof(ProcessSubPhaseRow))]
        public int? ParentProcessPhaseId { get; set; }

        /// <summary>
        /// Gets or sets the process version.
        /// </summary>
        [Relation]
        public ProcessVersionRow ProcessVersion { get; set; }

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
                return ProcessPhaseRelations.Value;
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
        public static bool operator ==(ProcessPhaseRow valueA, ProcessPhaseRow valueB)
        {
            return EqualityComparer<ProcessPhaseRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ProcessPhaseRow valueA, ProcessPhaseRow valueB)
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
        public bool Equals(ProcessPhaseRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}