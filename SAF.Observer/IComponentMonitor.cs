namespace SAF.Observer
{
    using System;

    public interface IComponentMonitor : IDisposable
    {
        /// <summary>
        /// Occurs when the resource becomes available.
        /// </summary>
        event EventHandler ComponentAvailable;

        /// <summary>
        /// Occurs when the resource becomes unavailable.
        /// </summary>
        event EventHandler ComponentUnavailable;

        /// <summary>
        /// Gets the status of the current component.
        /// </summary>
        ComponentStatus Status { get; }
    }
}