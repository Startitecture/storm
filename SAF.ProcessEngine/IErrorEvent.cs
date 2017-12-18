namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to event arguments that contain exceptions.
    /// </summary>
    public interface IErrorEvent
    {
        /// <summary>
        /// Gets the exception, if any, associated with the event arguments.
        /// </summary>
        Exception EventError { get; }
    }
}
