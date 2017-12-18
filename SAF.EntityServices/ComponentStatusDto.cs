// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentStatusDto.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A data contract (DTO) for component status items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A data contract (DTO) for component status items.
    /// </summary>
    [DataContract]
    public class ComponentStatusDto
    {
        /// <summary>
        /// Gets or sets the qualified name of the component.
        /// </summary>
        [DataMember]
        public string QualifiedName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component is available.
        /// </summary>
        [DataMember]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the resource weight of the current component.
        /// </summary>
        [DataMember]
        public double ResourceWeight { get; set; }

        /// <summary>
        /// Gets or sets the ranked resource weight of the current component.
        /// </summary>
        [DataMember]
        public double RankedResourceWeight { get; set; }

        /// <summary>
        /// Gets or sets the dependent resources of the component.
        /// </summary>
        [DataMember]
        public List<RankedResourceStatusDto> Resources { get; set; }
    }
}
