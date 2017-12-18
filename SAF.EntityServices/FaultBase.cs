// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for WCF faults.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Runtime.Serialization;

    using SAF.Core;

    /// <summary>
    /// Base class for WCF faults.
    /// </summary>
    [DataContract]
    public abstract class FaultBase : IActionFault
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat =
            "Action '{0}' on {1} '{2}' at '{3}' resulted in a(n) {4} [{5}]:{6}{7}: {8}{6}Additional Data:{6}{9}{6}Error Data:{6}{10}";

        /// <summary>
        /// Gets or sets the global identifier of the action that resulted in this specific fault.
        /// </summary>
        [DataMember]
        public Guid ActionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the fault time.
        /// </summary>
        [DataMember]
        public DateTimeOffset FaultTime { get; set; }

        /// <summary>
        /// Gets or sets the action source.
        /// </summary>
        [DataMember]
        public string ActionSource { get; set; }

        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        [DataMember]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [DataMember]
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the target entity.
        /// </summary>
        [DataMember]
        public string TargetEntity { get; set; }

        /// <summary>
        /// Gets or sets the reason for the fault.
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the additional data associated with the current fault.
        /// </summary>
        [DataMember]
        public abstract string AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        [DataMember]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        [DataMember]
        public string ErrorData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fault has been saved to some repository.
        /// </summary>
        [DataMember]
        public bool Saved { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="FaultBase"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="FaultBase"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(
                ToStringFormat,
                this.Action,
                this.EntityType,
                this.TargetEntity,
                this.FaultTime,
                this.GetType().ToRuntimeName(),
                this.ErrorCode,
                Environment.NewLine,
                this.ErrorType,
                this.Reason,
                this.AdditionalData,
                this.ErrorData);
        }
    }
}