// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStatusDto.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A data contract (DTO) for resource status.
    /// </summary>
    [DataContract]
    public class ResourceStatusDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the resource is available.
        /// </summary>
        [DataMember]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the relative weight of accessing the resource.
        /// </summary>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the qualified name of the resource.
        /// </summary>
        [DataMember]
        public string QualifiedName { get; set; }
    }
}
