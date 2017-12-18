namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains data related to a process stop event.
    /// </summary>
    public class ProcessStoppedEventArgs : EventArgs, IErrorEvent
    {
        /// <summary>
        /// Represents a <see cref="ProcessStoppedEventArgs"/> event with no error.
        /// </summary>
        public static readonly new ProcessStoppedEventArgs Empty = new ProcessStoppedEventArgs(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStoppedEventArgs"/> class with the specified error
        /// (if any) associated with the event.
        /// </summary>
        /// <param name="error">The error, if any, associated with the event.</param>
        public ProcessStoppedEventArgs(Exception error)
        {
            this.EventError = error;
        }

        /// <summary>
        /// Gets the error, if any, associated with this event.
        /// </summary>
        public Exception EventError { get; private set; }
    }
}
