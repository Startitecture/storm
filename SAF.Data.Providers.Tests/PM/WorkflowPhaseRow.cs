// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowPhaseRow.cs" company="Startitecture">
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
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The workflow phase row.
    /// </summary>
    public partial class WorkflowPhaseRow : ICompositeEntity, IEquatable<WorkflowPhaseRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<WorkflowPhaseRow, object>[] ComparisonProperties =
            {
                item => item.ProcessPhase,
                item => item.ProcessPhaseStatusId,
                item => item.EndTime,
                item => item.LegacyWorkflowPhaseId,
                item => item.PhaseOwner,
                item => item.PhaseOwnerUserId,
                item => item.ProcessPhaseId,
                item => item.StartTime,
                item => item.Workflow,
                item => item.WorkflowId,
                item => item.WorkflowPhaseRetractionProcessPhaseStatusId,
                item => item.WorkflowSignatureOption
            };

        /// <summary>
        /// The workflow phase relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> WorkflowPhaseRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new SqlFromClause<WorkflowPhaseRow>().InnerJoin(row => row.WorkflowId, row => row.Workflow.WorkflowId)
                        .LeftJoin<WorkflowPhaseRetractionRow>(row => row.WorkflowPhaseId, row => row.WorkflowPhaseRetractionId)
                        .LeftJoin(row => row.WorkflowPhaseId, row => row.WorkflowSignatureOption.WorkflowSignatureOptionId)
                        .LeftJoin(
                            row => row.WorkflowSignatureOption.SignatureOptionId,
                            row => row.WorkflowSignatureOption.SignatureOption.SignatureOptionId)
                        .InnerJoin(row => row.PhaseOwnerUserId, row => row.PhaseOwner.UserId)
                        .InnerJoin<ViewLegacyWorkflowPhaseRow>(row => row.WorkflowPhaseId, row => row.WorkflowPhaseId)
                        .InnerJoin(row => row.ProcessPhaseId, row => row.ProcessPhase.ProcessPhaseId)
                        .LeftJoin(row => row.ProcessPhase.ProcessPhaseId, row => row.ProcessPhase.PhaseActionDeadline.PhaseActionDeadlineId)
                        .LeftJoin<ProcessPhaseRow, ProcessSubPhaseRow>(row => row.ProcessPhaseId, row => row.ProcessSubPhaseId)
                        .InnerJoin<ProcessPhaseRow, ViewLegacyPhaseRow>(row => row.ProcessPhaseId, row => row.ProcessPhaseId)
                        .InnerJoin(row => row.Workflow.InitiatorUserId, row => row.Workflow.Initiator.UserId)
                        .InnerJoin(row => row.ProcessPhase.ProcessVersionId, row => row.ProcessPhase.ProcessVersion.ProcessVersionId)
                        .InnerJoin(row => row.Workflow.ProcessId, row => row.ProcessPhase.ProcessVersion.Process.ProcessId)
                        .Relations);

        /// <summary>
        /// Gets or sets the LegacyWorkflowPhaseId.
        /// </summary>
        [RelatedEntity(typeof(ViewLegacyWorkflowPhaseRow))]
        public int? LegacyWorkflowPhaseId { get; set; }

        /// <summary>
        /// Gets or sets the ProcessPhaseStatusId.
        /// </summary>
        [RelatedEntity(typeof(WorkflowPhaseRetractionRow), true)]
        public int? WorkflowPhaseRetractionProcessPhaseStatusId { get; set; }

        /// <summary>
        /// Gets or sets the phase owner.
        /// </summary>
        [Relation]
        public UserRow PhaseOwner { get; set; }

        /// <summary>
        /// Gets or sets the process phase.
        /// </summary>
        [Relation]
        public ProcessPhaseRow ProcessPhase { get; set; }

        /// <summary>
        /// Gets or sets the workflow signature option.
        /// </summary>
        [Relation]
        public WorkflowSignatureOptionRow WorkflowSignatureOption { get; set; }

        /// <summary>
        /// Gets or sets the workflow.
        /// </summary>
        [Relation]
        public WorkflowRow Workflow { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return WorkflowPhaseRelations.Value;
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
        public static bool operator ==(WorkflowPhaseRow valueA, WorkflowPhaseRow valueB)
        {
            return EqualityComparer<WorkflowPhaseRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(WorkflowPhaseRow valueA, WorkflowPhaseRow valueB)
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
            return $"{this.ProcessPhase?.Name ?? Convert.ToString(this.WorkflowPhaseId)}:Status={this.ProcessPhaseStatusId}";
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
        public bool Equals(WorkflowPhaseRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

    }
}