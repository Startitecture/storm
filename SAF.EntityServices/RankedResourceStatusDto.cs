// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RankedResourceStatusDto.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A data contract (DTO) for ranked resource status items.
    /// </summary>
    [DataContract]
    public class RankedResourceStatusDto
    {
        /// <summary>
        /// Gets or sets the resource rank.
        /// </summary>
        [DataMember]
        public int ResourceRank { get; set; }

        /// <summary>
        /// Gets or sets the resource status.
        /// </summary>
        [DataMember]
        public ResourceStatusDto ResourceStatus { get; set; }
    }
}
