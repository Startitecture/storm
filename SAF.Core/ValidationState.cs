namespace SAF.Core
{
    /// <summary>
    /// The possible states of validation for an item.
    /// </summary>
    public enum ValidationState
    {
        /// <summary>
        /// The state of the item is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The item is null.
        /// </summary>
        Null = 1,

        /// <summary>
        /// The item is empty.
        /// </summary>
        Empty = 2,

        /// <summary>
        /// The item is not in a valid format.
        /// </summary>
        InvalidFormat = 3,

        /// <summary>
        /// The item is less than its minimum allowed value.
        /// </summary>
        LessThanMinRange = 4,

        /// <summary>
        /// The item is greater than its maximum allowed value.
        /// </summary>
        GreaterThanMaxRange = 5,

        /// <summary>
        /// The item has passed all validation checks.
        /// </summary>
        Valid = 6
    }
}
