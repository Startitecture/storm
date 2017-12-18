// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistributionChannelDto.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A data contract (DTO) for a distribution channel.
    /// </summary>
    [DataContract]
    public class DistributionChannelDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionChannelDto"/> class.
        /// </summary>
        public DistributionChannelDto()
        {
            this.ComponentStatus = new ComponentStatusDto();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the distribution channel ID.
        /// </summary>
        [DataMember]
        public int? DistributionChannelId { get; set; }

        /// <summary>
        /// Gets or sets the location of the channel.
        /// </summary>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the distribution channel is administratively offline.
        /// </summary>
        [DataMember]
        public bool AdministrativelyOffline { get; set; }

        /// <summary>
        /// Gets or sets the component status for the channel.
        /// </summary>
        [DataMember]
        public ComponentStatusDto ComponentStatus { get; set; }

        #endregion
    }
}