// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEventRepositoryFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Creates event repositories that use the tracing functionality in the System.Diagnostics namespace to save errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    /// <summary>
    /// Creates event repositories that use the tracing functionality in the System.Diagnostics namespace to save errors.
    /// </summary>
    public class TraceEventRepositoryFactory : IEventRepositoryFactory
    {
        /// <summary>
        /// Gets or sets a value indicating whether to suppress information events.
        /// </summary>
        public bool SuppressInformationEvents { get; set; }

        /// <summary>
        /// Resolves the event repository for the current application.
        /// </summary>
        /// <returns>
        /// The <see cref="SAF.ActionTracking.IEventRepository"/> for the current application.
        /// </returns>
        public IEventRepository Create()
        {
            return new TraceEventRepository(this.SuppressInformationEvents);
        }
    }
}
