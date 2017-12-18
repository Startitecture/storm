namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains a static collection of LINQ queries to prevent dynamic invocation.
    /// </summary>
    internal static class Queries
    {
        /// <summary>
        /// Gets a selector on a <see cref="ProcessRegistration"/> for the registered <see cref="IProcessEngine"/>.
        /// </summary>
        public static readonly Func<ProcessRegistration, IProcessEngine> EngineSelector = x => x.Process;

        /// <summary>
        /// Gets a predicate selecting <see cref="ProcessRegistration"/>s that are registered as producers.
        /// </summary>
        public static readonly Func<ProcessRegistration, bool> ProducerSelector = x => x.ProcessType == ProcessType.Producer;

        /// <summary>
        /// Gets a predicate selecting <see cref="ProcessRegistration"/>s that are registered as consumers.
        /// </summary>
        public static readonly Func<ProcessRegistration, bool> ConsumerSelector = x => x.ProcessType == ProcessType.Consumer;

        /// <summary>
        /// Gets a predicate selecting <see cref="ProcessRegistration"/>s that are registered as auxiliaries.
        /// </summary>
        public static readonly Func<ProcessRegistration, bool> AuxiliarySelector = x => x.ProcessType == ProcessType.Auxiliary;

        /// <summary>
        /// Gets a predicate selecting <see cref="ITaskEngine"/>s with tasks greater than zero.
        /// </summary>
        public static readonly Func<ITaskEngine, bool> TasksGreaterThanZero = x => x.TotalTasks > 0;

        /// <summary>
        /// Gets a function that selects the total tasks from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> TotalTasks = x => x.TotalTasks;

        /// <summary>
        /// Gets a function that selects the waiting tasks from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> WaitingTasks = x => x.WaitingTasks;

        /// <summary>
        /// Gets a function that selects the completed tasks from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> CompletedTasks = x => x.CompletedTasks;

        /// <summary>
        /// Gets a function that selects the progress from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, double> Progress = x => x.Progress;

        /// <summary>
        /// Gets a function that selects the successful results from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> SuccessfulResults = x => x.SuccessfulResults;

        /// <summary>
        /// Gets a function that selects the empty results from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> EmptyResults = x => x.EmptyResults;

        /// <summary>
        /// Gets a function that selects the failed results from an <see cref="ITaskEngine"/>.
        /// </summary>
        public static readonly Func<ITaskEngine, long> FailedResults = x => x.FailedResults;
    }
}
