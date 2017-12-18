// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationConfigurationFault.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Describes an error related to a failed configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes an error related to a failed service configuration.
    /// </summary>
    [DataContract]
    public class ApplicationConfigurationFault : FaultBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the configuration area.
        /// </summary>
        [DataMember]
        public string ConfigurationArea { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the additional data associated with the current fault.
        /// </summary>
        [DataMember]
        public override string AdditionalData
        {
            get
            {
                return this.ConfigurationArea;
            }

            set
            {
                this.ConfigurationArea = value;
            }
        }
    }
}