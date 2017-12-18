namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to an event trigger.
    /// </summary>
    public interface IEventTrigger
    {
        /// <summary>
        /// Gets a value that determines whether the condition defined in the trigger matches the specified event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        /// <returns>True if the event matches the condition, otherwise false.</returns>
        bool IsConditionTrue(object sender, EventArgs e);
    }
}
