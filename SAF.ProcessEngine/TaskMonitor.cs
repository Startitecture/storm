namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Monitors an <see cref="ITaskEngine"/> for property changes and notifies subscribers. 
    /// </summary>
    public class TaskMonitor : MonitorBase
    {
        /// <summary>
        /// The task engine to monitor.
        /// </summary>
        private readonly ITaskEngine taskEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMonitor"/> class.
        /// </summary>
        /// <param name="taskEngine">The <see cref="ITaskEngine"/> to monitor.</param>
        public TaskMonitor(ITaskEngine taskEngine)
        {
            if (taskEngine == null)
            {
                throw new ArgumentNullException("taskEngine");
            }

            this.Name = taskEngine.Name;
            this.taskEngine = taskEngine;
        }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task engine is busy (true) or not (false).
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// Gets the amount of time spent processing tasks.
        /// </summary>
        public TimeSpan ProcessTime { get; private set; }

        /// <summary>
        /// Gets the health of the current process in terms of whether tasks or queues have failed.
        /// </summary>
        public UserResultState ProcessHealth { get; private set; }

        /// <summary>
        /// Gets the number of items added to the collection.
        /// </summary>
        public long TotalTasks { get; private set; }

        /// <summary>
        /// Gets the number of tasks consumed by this task engine.
        /// </summary>
        public long CompletedTasks
        { 
            get { return this.SuccessfulResults + this.EmptyResults + this.FailedResults; }
        }

        /// <summary>
        /// Gets the number of successful task results.
        /// </summary>
        public long SuccessfulResults { get; private set; }

        /// <summary>
        /// Gets the number of empty task results.
        /// </summary>
        public long EmptyResults { get; private set; }

        /// <summary>
        /// Gets the number of failed task results.
        /// </summary>
        public long FailedResults { get; private set; }

        /// <summary>
        /// Gets the current progress of this process from 0 to 1.
        /// </summary>
        public double Progress { get; private set; }

        /// <summary>
        /// Gets the average time taken per task.
        /// </summary>
        public double TasksPerSecond { get; private set; }

        /// <summary>
        /// Detects changed properties for task engines.
        /// </summary>
        protected override void DetectChangedProperties()
        {
            base.DetectChangedProperties();

            if (0 != String.Compare(this.taskEngine.Name, this.Name, StringComparison.Ordinal))
            {
                this.Name = this.taskEngine.Name;
                this.AddChangedProperty(Constants.NameProperty);
            }

            if (this.taskEngine.ProcessTime != this.ProcessTime)
            {
                this.ProcessTime = this.taskEngine.ProcessTime;
                this.AddChangedProperty(Constants.ProcessTimeProperty);
            }

            if (this.taskEngine.TotalTasks != this.TotalTasks)
            {
                this.TotalTasks = this.taskEngine.TotalTasks;
                this.Progress = this.taskEngine.Progress;
                this.AddChangedProperty(Constants.TotalTasksProperty);
                this.AddChangedProperty(Constants.ProgressProperty);
            }

            if (this.taskEngine.SuccessfulResults != this.SuccessfulResults)
            {
                this.SuccessfulResults = this.taskEngine.SuccessfulResults;
                this.Progress = this.taskEngine.Progress;
                this.TasksPerSecond = this.taskEngine.TasksPerSecond;
                this.AddChangedProperty(Constants.CompletedTasksProperty);
                this.AddChangedProperty(Constants.SuccessfulResultsProperty);
                this.AddChangedProperty(Constants.ProgressProperty);
                this.AddChangedProperty(Constants.TasksPerSecondProperty);
            }

            if (this.taskEngine.EmptyResults != this.EmptyResults)
            {
                this.EmptyResults = this.taskEngine.EmptyResults;
                this.Progress = this.taskEngine.Progress;
                this.TasksPerSecond = this.taskEngine.TasksPerSecond;
                this.AddChangedProperty(Constants.CompletedTasksProperty);
                this.AddChangedProperty(Constants.EmptyResultsProperty);
                this.AddChangedProperty(Constants.ProgressProperty);
                this.AddChangedProperty(Constants.TasksPerSecondProperty);
            }

            if (this.taskEngine.FailedResults != this.FailedResults)
            {
                this.FailedResults = this.taskEngine.FailedResults;
                this.Progress = this.taskEngine.Progress;
                this.TasksPerSecond = this.taskEngine.TasksPerSecond;
                this.AddChangedProperty(Constants.CompletedTasksProperty);
                this.AddChangedProperty(Constants.FailedResultsProperty);
                this.AddChangedProperty(Constants.ProgressProperty);
                this.AddChangedProperty(Constants.TasksPerSecondProperty);
            }

            if (this.taskEngine.IsBusy != this.IsBusy)
            {
                this.IsBusy = this.taskEngine.IsBusy;
                this.AddChangedProperty(Constants.IsBusyProperty);
            }

            if (this.taskEngine.ProcessHealth != this.ProcessHealth)
            {
                this.ProcessHealth = this.taskEngine.ProcessHealth;
                this.AddChangedProperty(Constants.ProcessHealthProperty);
            }
        }
    }
}
