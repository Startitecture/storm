// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkResourceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maintains the status of a networked resource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System;
    using System.IO;

    using SAF.Core;

    /// <summary>
    /// Maintains the status of a networked resource.
    /// </summary>
    public class NetworkResourceMonitor : ResourceMonitor
    {
        /// <summary>
        /// The network server.
        /// </summary>
        private readonly string networkServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkResourceMonitor"/> class.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        public NetworkResourceMonitor(string location)
            : base(location, TimeSpan.FromSeconds(5))
        {
            this.networkServer = FileSystem.GetServerName(this.Location);
        }

        /// <summary>
        /// Gets a value indicating whether the service resource is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the resource is available; otherwise, <c>false</c>.
        /// </returns>
        protected override StatusResult CheckAvailability()
        {
            var checkAvailability = Directory.Exists(this.Location) || File.Exists(this.Location);
            return new StatusResult { IsAvailable = checkAvailability };
        }

        /// <summary>
        /// Calculates the weight of accessing the resource.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the relative weight of accessing the resource.
        /// </returns>
        protected override double CalculateWeight()
        {
            bool isLocal = FileSystem.IsPathLocallyTranslatable(this.Location)
                           || StringComparer.OrdinalIgnoreCase.Equals(this.networkServer, Environment.MachineName);

            return isLocal ? 1 : 5;
        }
    }
}
