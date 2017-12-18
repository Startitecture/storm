// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The workflow row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The workflow row.
    /// </summary>
    [TableName("[dbo].[cc_process_workflow]")]
    [PrimaryKey("ccpw_id")]
    [ExplicitColumns]
    public class WorkflowRow : ITransactionContext, IEquatable<WorkflowRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<WorkflowRow, object>[] ComparisonProperties =
            {
                item => item.Subject,
                item => item.EndDate,
                item => item.Initiator,
                item => item.InitiatorUserId,
                item => item.IsCorrupt,
                item => item.IsDraft,
                item => item.ProcessId,
                item => item.ProcessStatusId,
                item => item.ProcessVersion,
                item => item.StartDate,
                item => item.WorkflowId
            };

        /// <summary>
        /// Gets or sets the workflow id.
        /// </summary>
        [Column("ccpw_id")]
        public int WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [Column("ccpw_subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the process id.
        /// </summary>
        [Column("ccpw_ccp_id")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the initiator user id.
        /// </summary>
        [Column("ccpw_initiator_u_id")]
        public int InitiatorUserId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [Column("ccpw_start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [Column("ccpw_end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the process status id.
        /// </summary>
        [Column("ccpw_ccps_id")]
        public int ProcessStatusId { get; set; }

        ////[Column]
        ////public string ccpw_status_comment { get; set; }
        ////[Column]
        ////public int? ccpw_u_id { get; set; }
        ////[Column]
        ////public int ccpw_ccd_id { get; set; }
        ////[Column]
        ////public DateTime? ccpw_comment_date { get; set; }

        /// <summary>
        /// Gets or sets the is draft.
        /// </summary>
        [Column("ccpw_is_draft")]
        public bool? IsDraft { get; set; }

        ////[Column]
        ////public DateTime? ccpw_revive_date { get; set; }
        ////[Column]
        ////public int? ccpw_on_reject_ccps_id { get; set; }
        ////[Column]
        ////public int? ccpw_c_id { get; set; }
        ////[Column]
        ////public bool ccpw_c_import { get; set; }
        ////[Column]
        ////public int? ccpw_od_id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is corrupt.
        /// </summary>
        [Column("ccpw_is_corrupt")]
        public bool IsCorrupt { get; set; }

        /// <summary>
        /// Gets or sets the process version.
        /// </summary>
        [Relation]
        public ProcessVersionRow ProcessVersion { get; set; }

        /// <summary>
        /// Gets or sets the initiator.
        /// </summary>
        [Relation]
        public UserRow Initiator { get; set; }

        /// <summary>
        /// Gets the provider for the current transaction.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }

        ////public bool ccpw_expertrfp_pending { get; set; }
        ////[Column]
        ////public int? ccpw_taw_s_id { get; set; }
        ////[Column]


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
        public static bool operator ==(WorkflowRow valueA, WorkflowRow valueB)
        {
            return EqualityComparer<WorkflowRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(WorkflowRow valueA, WorkflowRow valueB)
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
        public bool Equals(WorkflowRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

    }
}
