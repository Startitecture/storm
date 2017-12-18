namespace SAF.ProcessEngine
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Base class for task results.
    /// </summary>
    /// <typeparam name="TDirective">The type of directive representing the task associated with this result.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    public class TaskResult<TDirective, TResult> : ItemResult<TResult>, ITaskResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult&lt;TDirective, TResult&gt;"/> class, with the specified directive,
        /// success predicate, task result and exception (if any) associated with the task result.
        /// </summary>
        /// <param name="directive">The directive associated with the task result.</param>
        /// <param name="result">The result of the task.</param>
        /// <param name="error">The exception, if any, associated with the task result.</param>
        protected TaskResult(TDirective directive, TResult result, Exception error)
            : base(result, error)
        {
            if (Evaluate.IsNull(directive))
            {
                throw new ArgumentNullException("directive");
            }

            this.Directive = directive;
        }

        /// <summary>
        /// Gets the directive associated with the task result.
        /// </summary>
        public TDirective Directive { get; private set; }
    }
}
