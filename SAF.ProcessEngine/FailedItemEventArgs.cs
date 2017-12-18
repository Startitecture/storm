namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains data related to an item failure.
    /// </summary>
    /// <typeparam name="TItem">The type of item associated with the failure.</typeparam>
    public class FailedItemEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedItemEventArgs&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="item">The item associated with the failure.</param>
        /// <param name="error">The exception that caused the failure.</param>
        public FailedItemEventArgs(TItem item, Exception error)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.FailedItem = item;
            this.ItemError = error;
        }

        /// <summary>
        /// Gets the item associated with the failure.
        /// </summary>
        public TItem FailedItem { get; private set; }

        /// <summary>
        /// Gets the exception that caused the failure.
        /// </summary>
        public Exception ItemError { get; private set; }
    }
}
