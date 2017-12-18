// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityValidationFault.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Fault contract for entity validation failures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Fault contract for entity validation failures.
    /// </summary>
    [DataContract]
    public class EntityValidationFault : FaultBase
    {
        /// <summary>
        /// The error count format.
        /// </summary>
        private const string ErrorCountFormat = "Error Count: {0}";

        /// <summary>
        /// The validation errors.
        /// </summary>
        private Collection<string> validationErrors = new Collection<string>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the validation errors.
        /// </summary>
        [DataMember]
        public Collection<string> ValidationErrors
        {
            get
            {
                return this.validationErrors ?? (this.validationErrors = new Collection<string>());
            }

            set
            {
                this.validationErrors = value ?? new Collection<string>();
                this.AdditionalData = String.Format(ErrorCountFormat, this.validationErrors.Count);
            }
        }

        #endregion

        /// <summary>
        /// Gets the additional data associated with the current fault.
        /// </summary>
        [DataMember]
        public override string AdditionalData { get; set; }
    }
}