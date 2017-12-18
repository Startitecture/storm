// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalOperationFault.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains details about an operation failure due to an unexpected condition internal to the service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains details about an operation failure due to an unexpected condition internal to the service.
    /// </summary>
    public class InternalOperationFault : FaultBase
    {
        /// <summary>
        /// Gets the additional data associated with the current fault.
        /// </summary>
        [DataMember]
        public override string AdditionalData
        {
            get
            {
                return null;
            }

            set
            {
            }
        }
    }
}
