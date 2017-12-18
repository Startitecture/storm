// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryFault.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Describes the result of a failed operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes the result of a failed persistence operation.
    /// </summary>
    [DataContract]
    public class EntityRepositoryFault : FaultBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the repository identifier.
        /// </summary>
        [DataMember]
        public string RepositoryIdentifier { get; set; }

        #endregion

        /// <summary>
        /// Gets the additional data associated with the current fault.
        /// </summary>
        [DataMember]
        public override string AdditionalData
        {
            get
            {
                return this.RepositoryIdentifier;
            }

            set
            {
                this.RepositoryIdentifier = value;
            }
        }
    }
}