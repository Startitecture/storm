namespace SAF.Data.Persistence
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Contains action result data for an entity update task.
    /// </summary>
    /// <typeparam name="T">The type of entity that was updated.</typeparam>
    public class EntityUpdateResult<T> : TaskResult<EntityUpdateDirective<T>, EntityAction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdateResult&lt;T&gt;"/> class with the 
        /// specified entity update directive, entity update action result and error (if any) associated with the
        /// entity update action.
        /// </summary>
        /// <param name="directive">The directive associated with the entity update action.</param>
        /// <param name="result">The result of the entity update action.</param>
        /// <param name="error">The error, if any, associated with the entity update action.</param>
        public EntityUpdateResult(
            EntityUpdateDirective<T> directive, EntityAction result, Exception error)
            : base(directive, result, error)
        {
        }
    }
}
