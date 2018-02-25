namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The market.
    /// </summary>
    public class Market
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Market"/> class.
        /// </summary>
        public Market()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Market"/> class.
        /// </summary>
        /// <param name="marketId">
        /// The market ID.
        /// </param>
        public Market(int? marketId)
        {
            this.MarketId = marketId;
        }

        /// <summary>
        /// Gets the market ID.
        /// </summary>
        public int? MarketId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}