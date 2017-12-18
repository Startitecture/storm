namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains data related to the addition of items to an existing collection.
    /// </summary>
    public sealed class ItemsAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents an event with no data.
        /// </summary>
        public static readonly new ItemsAddedEventArgs Empty = new ItemsAddedEventArgs();

        /// <summary>
        /// Prevents a default instance of the <see cref="ItemsAddedEventArgs"/> class from being created.
        /// </summary>
        private ItemsAddedEventArgs()
        {
        }
    }
}
