namespace SAF.Data.Persistence
{
    using System;
    using System.Collections.Generic;

    using SAF.ProcessEngine;

    /// <summary>
    /// Contains the results of a retention policy directive.
    /// </summary>
    /// <typeparam name="TContainer">The type of container the retention policy was applied to.</typeparam>
    /// <typeparam name="TItemResult">The type of result produced by the policy.</typeparam>
    public class RetentionPolicyResult<TContainer, TItemResult> : 
        TaskResult<RetentionPolicyDirective<TContainer>, IEnumerable<TItemResult>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetentionPolicyResult&lt;TContainer, TItemResult&gt;"/> class.
        /// </summary>
        /// <param name="directive">The directive associated with the task result.</param>
        /// <param name="result">The result of the task.</param>
        /// <param name="error">The exception, if any, associated with the task result.</param>
        protected RetentionPolicyResult(
            RetentionPolicyDirective<TContainer> directive,
            IEnumerable<TItemResult> result,
            Exception error)
            : base(directive, result, error)
        {
        }
    }
}
