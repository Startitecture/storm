namespace SAF.Core
{
    /// <summary>
    /// Contains all possible validation request types.
    /// </summary>
    public enum ValidationRequestType
    {
        /// <summary>
        /// No validation is required.
        /// </summary>
        None = 0,

        /// <summary>
        /// The value must match one or more provided regular expressions.
        /// </summary>
        MatchesRegex = 1,

        /// <summary>
        /// The value must not match any provided regular expression.
        /// </summary>
        DoesNotMatchRegex = 2,

        /// <summary>
        /// The value must be within a specified range.
        /// </summary>
        WithinRange = 3
    }
}
