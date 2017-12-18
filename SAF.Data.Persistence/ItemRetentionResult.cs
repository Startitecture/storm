namespace SAF.Data.Persistence
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Represents the result of a retention policy action.
    /// </summary>
    /// <typeparam name="TItem">The type of target the policy is applied to.</typeparam>
    public class ItemRetentionResult<TItem> 
        : TaskResult<ItemRetentionDirective<TItem>, PolicyResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRetentionResult&lt;TItem&gt;"/> class with the specified 
        /// directive, action result, effective date and associated error (if any).
        /// </summary>
        /// <param name="directive">The retention directive associated with this result.</param>
        /// <param name="result">The result of the action.</param>
        /// <param name="effectiveDate">The effective date and time of the action.</param>
        /// <param name="error">The error, if any, assoicated with the retention action.</param>
        public ItemRetentionResult(
            ItemRetentionDirective<TItem> directive, 
            PolicyResult result, 
            DateTimeOffset effectiveDate, 
            Exception error)
            : base(directive, result, error)
        {
            this.EffectiveDate = effectiveDate;
        }

        /// <summary>
        /// Gets the date and time when the policy was applied.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; private set; }
    }
}
