// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeResourceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using SAF.Core;
    using SAF.Observer;

    /// <summary>
    /// The fake resource monitor.
    /// </summary>
    public class FakeResourceMonitor : ResourceMonitor
    {
        /// <summary>
        /// The availability.
        /// </summary>
        private bool availability;

        /// <summary>
        /// The weight.
        /// </summary>
        private double weight;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeResourceMonitor"/> class.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        public FakeResourceMonitor(string location)
            : base(location)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether availability.
        /// </summary>
        public bool Availability
        {
            get
            {
                return this.availability;
            }

            set
            {
                this.availability = value;
                this.UpdateStatus(this.ResourceStatus);
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public double Weight
        {
            get
            {
                return this.weight;
            }

            set
            {
                this.weight = value;
                this.UpdateStatus(this.ResourceStatus);
            }
        }

        /// <summary>
        /// The check availability.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override StatusResult CheckAvailability()
        {
            return new StatusResult { IsAvailable = this.availability };
        }

        /// <summary>
        /// The calculate weight.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        protected override double CalculateWeight()
        {
            return this.weight;
        }
    }
}
