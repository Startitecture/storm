namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains event data releated to the start of a process.
    /// </summary>
    public sealed class ProcessStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents an event with no data.
        /// </summary>
        public static readonly new ProcessStartedEventArgs Empty = new ProcessStartedEventArgs();

        /// <summary>
        /// Prevents a default instance of the <see cref="ProcessStartedEventArgs"/> class from being created.
        /// </summary>
        private ProcessStartedEventArgs()
        {
        }
    }
}
