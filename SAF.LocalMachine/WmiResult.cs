namespace SAF.LocalMachine
{
    /// <summary>
    /// Contains the result of a <see cref="WmiQuery"/>.
    /// </summary>
    public class WmiResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiResult"/> class.
        /// </summary>
        /// <param name="query">The associated query.</param>
        /// <param name="results">The properties returned by the query.</param>
        internal WmiResult(WmiQuery query, WmiPropertyCollection results)
        {
            this.Query = query;
            this.Properties = results;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="WmiResult"/> class from being created.
        /// </summary>
        private WmiResult()
        {
        }

        /// <summary>
        /// Gets the associated query.
        /// </summary>
        public WmiQuery Query { get; private set; }

        /// <summary>
        /// Gets the properties retrieved by the query.
        /// </summary>
        public WmiPropertyCollection Properties { get; private set; }
    }
}
