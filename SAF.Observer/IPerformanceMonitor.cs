// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerformanceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a performance monitor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System.Linq;

    /// <summary>
    /// Provides an interface to a performance monitor.
    /// </summary>
    public interface IPerformanceMonitor
    {
        /// <summary>
        /// Gets a list of recent CPU samples.
        /// </summary>
        IQueryable<double> ProcessorUsageSamples { get; }
    }
}