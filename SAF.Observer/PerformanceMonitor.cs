// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Monitors the performance of the current system's CPU.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Timers;

    /// <summary>
    /// Monitors the performance of the current system's CPU.
    /// </summary>
    public sealed class PerformanceMonitor : IDisposable, IPerformanceMonitor
    {
        /// <summary>
        /// The current monitor.
        /// </summary>
        private static readonly PerformanceMonitor CurrentMonitor = new PerformanceMonitor();

        /// <summary>
        /// A performance counter for total CPU usage.
        /// </summary>
        private readonly Lazy<PerformanceCounter> cpuTotal = new Lazy<PerformanceCounter>(CreateCpuPerformanceCounter);

        /// <summary>
        /// The resource check timer.
        /// </summary>
        private readonly Timer resourceCheckTimer = new Timer(100);

        /// <summary>
        /// A stack of recent CPU samples.
        /// </summary>
        private readonly ConcurrentQueue<double> cpuSamples = new ConcurrentQueue<double>();

        /// <summary>
        /// Prevents a default instance of the <see cref="PerformanceMonitor"/> class from being created.
        /// </summary>
        private PerformanceMonitor()
        {
            this.resourceCheckTimer.Elapsed += this.CheckResources;
            this.resourceCheckTimer.Start();
        }

        /// <summary>
        /// Gets the current performance monitor.
        /// </summary>
        public static PerformanceMonitor Current
        {
            get
            {
                return CurrentMonitor;
            }
        }

        /// <summary>
        /// Gets a list of recent CPU samples.
        /// </summary>
        public IQueryable<double> ProcessorUsageSamples
        {
            get
            {
                return this.cpuSamples.AsQueryable();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.resourceCheckTimer != null)
            {
                this.resourceCheckTimer.Dispose();
            }
        }

        /// <summary>
        /// Creates a CPU performance counter.
        /// </summary>
        /// <returns>
        /// A <see cref="PerformanceCounter"/> for total processor time.
        /// </returns>
        private static PerformanceCounter CreateCpuPerformanceCounter()
        {
            return new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        /// <summary>
        /// Checks resources to determine whether more queues can be added.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// Event data associated with the event.
        /// </param>
        private void CheckResources(object sender, ElapsedEventArgs e)
        {
            var value = Double.Parse(Convert.ToString(this.cpuTotal.Value.NextValue()));
            this.cpuSamples.Enqueue(value);

            if (this.cpuSamples.Count <= 600)
            {
                return;
            }

            lock (this.ProcessorUsageSamples)
            {
                double dequeue;
                this.cpuSamples.TryDequeue(out dequeue);
            }
        }
    }
}
