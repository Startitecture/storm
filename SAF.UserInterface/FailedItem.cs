namespace SAF.UserInterface
{
    using System;

    /// <summary>
    /// Contains failed items.
    /// </summary>
    /////// <typeparam name="T">The type of item that failed.</typeparam>
    public class FailedItem////<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedItem"/> class.
        /// </summary>
        /// <param name="item">The item that caused the failure.</param>
        /// <param name="error">The exception associated with the failure.</param>
        public FailedItem(object item, Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.Item = item;
            this.ItemError = error.Message;
        }

        /// <summary>
        /// Gets the item that cuased the failure.
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Gets the exception associated with the failure.
        /// </summary>
        public string ItemError { get; private set; }
    }
}
