namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Event data related to an items consumption event.
    /// </summary>
    public sealed class ItemsConsumedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents an event with no data.
        /// </summary>
        public static readonly new ItemsConsumedEventArgs Empty = new ItemsConsumedEventArgs();

        /// <summary>
        /// Prevents a default instance of the <see cref="ItemsConsumedEventArgs"/> class from being created.
        /// </summary>
        private ItemsConsumedEventArgs()
        {
        }
    }
}
