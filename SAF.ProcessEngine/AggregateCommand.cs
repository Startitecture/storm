// -----------------------------------------------------------------------
// <copyright file="AggregateCommand.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Executes multiple commands as a single command.
    /// </summary>
    public class AggregateCommand : IExecutable
    {
        /// <summary>
        /// Represents the empty <see cref="AggregateCommand"/>. This field is read-only.
        /// </summary>
        public static readonly AggregateCommand Empty = new AggregateCommand(String.Empty, new IExecutable[] { });

        /// <summary>
        /// Contains all the commands to be executed by this aggregate command.
        /// </summary>
        private readonly CommandCollection commandCollection = new CommandCollection();

        /// <summary>
        /// Contains all command failures related to the current execution.
        /// </summary>
        private readonly List<IErrorEvent> commandFailures = new List<IErrorEvent>();

        /// <summary>
        /// An object used to synchronize execution of commands.
        /// </summary>
        private readonly object executeLock = new object();

        /// <summary>
        /// The number of currently executing commands.
        /// </summary>
        private int pendingExecutions = 0;

        /// <summary>
        /// Indicates whether the commands will execute asynchronously or not.
        /// </summary>
        private bool executeAsync = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCommand"/> class with the specified name and commands to execute.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        public AggregateCommand(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCommand"/> class with the specified name and commands to execute.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="commands">The commands to execute.</param>
        public AggregateCommand(string name, IEnumerable<IExecutable> commands)
            : this(name)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            foreach (IExecutable command in commands)
            {
                this.Add(command);
            }
        }

        /// <summary>
        /// Occurs when any of the underlying commands encounters a change in execution context.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when the first command in the collection has started execution.
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when all commands have executed successfully.
        /// </summary>
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when one of the underlying commands fails to execute. In synchronous mode, no more commands will be invoked. In 
        /// asychronous mode, other commands already invoked may still complete but will not trigger events.
        /// </summary>
        public event EventHandler<CommandFailedEventArgs> CommandFailed;

        /// <summary>
        /// Gets the name of the command collection.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the commands will execute asynchronously (true) or in sequence (false).
        /// </summary>
        public bool ExecuteAsync
        {
            get
            {
                return this.executeAsync;
            }

            set
            {
                lock (this.executeLock)
                {
                    this.executeAsync = value;
                }
            }
        }

        /// <summary>
        /// Adds a command to the command collection.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void Add(IExecutable command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            lock (this.executeLock)
            {
                command.CommandCompleted += this.HandleCompletedCommand;
                command.CommandFailed += this.HandleFailedCommand;
                command.CanExecuteChanged += this.HandleExecutionContextChange;
                this.commandCollection.Add(command);
            }
        }

        /// <summary>
        /// Removes a command from the command collection.
        /// </summary>
        /// <param name="command">The command to remove.</param>
        /// <returns>
        /// <c>true</c> if the command was removed; otherwise <c>false</c>. This method also returns false if item was not found in 
        /// the collection.
        /// </returns>
        public bool Remove(IExecutable command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            lock (this.executeLock)
            {
                bool removed = this.commandCollection.Remove(command);

                if (removed)
                {
                    command.CommandFailed -= this.HandleFailedCommand;
                    command.CommandCompleted -= this.HandleCompletedCommand;
                    command.CanExecuteChanged -= this.HandleExecutionContextChange;
                }

                return removed;
            }
        }

        /// <summary>
        /// Clears all commands from the command collection.
        /// </summary>
        public void Clear()
        {
            List<IExecutable> commandsToRemove = new List<IExecutable>(this.commandCollection);

            foreach (IExecutable command in commandsToRemove)
            {
                this.Remove(command);
            }
        }

        /// <summary>
        /// Executes the commands in the command collection.
        /// </summary>
        /// <param name="parameter">The optional parameter to pass to the commands.</param>
        public void Execute(object parameter)
        {
            foreach (var command in this.commandCollection)
            {
                if (!command.CanExecute(parameter))
                {
                    throw new InvalidOperationException(
                        String.Format("Execution pre-conditions for '{0}' were not met.", command.Name));
                }
            }

            this.TriggerExecution(new CommandExecutingEventArgs(this));

            if (this.commandCollection.Count > 0)
            {
                foreach (var command in this.commandCollection)
                {
                    if (this.commandFailures.Count == 0)
                    {
                        lock (this.executeLock)
                        {
                            this.pendingExecutions++;
                        }

                        command.ExecuteAsync = this.ExecuteAsync;
                        command.Execute(parameter);
                    }
                }
            }
            else
            {
                // Trigger even if there are no commands to execute. This allows for consistent behavior.
                this.TriggerCompletion(new CommandCompletedEventArgs(this));
            }
        }

        /// <summary>
        /// Indicates whether the command can execute in the current context.
        /// </summary>
        /// <param name="parameter">An optional command parameter.</param>
        /// <returns><c>true</c> if the command can execute in the current context; otherwise <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            foreach (IExecutable command in this.commandCollection)
            {
                if (!command.CanExecute(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        #region Event Handlers

        /// <summary>
        /// Handles events that affect whether the command can execute or not.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data associated with the event.</param>
        protected void HandleExecutionContextChange(object sender, EventArgs e)
        {
            this.TriggerCanExecuteChanged(e);
        }

        /// <summary>
        /// Handles command failure events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void HandleFailedCommand(object sender, CommandFailedEventArgs e)
        {
            this.ProcessCommandCompletion(e);
        }

        /// <summary>
        /// Handles command completion events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void HandleCompletedCommand(object sender, CommandCompletedEventArgs e)
        {
            this.ProcessCommandCompletion(e);
        }

        /// <summary>
        /// Processes command completion and failures.
        /// </summary>
        /// <typeparam name="TArgs">The type of event to process.</typeparam>
        /// <param name="eventArgs">The event data to process.</param>
        private void ProcessCommandCompletion<TArgs>(TArgs eventArgs)
            where TArgs : EventArgs
        {
            IErrorEvent errorEvent = null;
            bool complete = false;

            lock (this.executeLock)
            {
                if (eventArgs is IErrorEvent)
                {
                    this.commandFailures.Add(eventArgs as IErrorEvent);
                    this.pendingExecutions = 0;
                }
                else
                {
                    this.pendingExecutions--;
                }

                // Setting the flag outside the lock keeps us from locking events over which we have no control.
                if (this.pendingExecutions == 0)
                {
                    complete = true;
                    errorEvent = this.commandFailures.FirstOrDefault();
                    this.commandFailures.Clear();
                }
            }

            if (complete)
            {
                if (errorEvent == null)
                {
                    this.TriggerCompletion(new CommandCompletedEventArgs(this));
                }
                else
                {
                    this.TriggerFailure(new CommandFailedEventArgs(this, errorEvent.EventError));
                }
            }
        }

        #endregion

        #region Event Triggers

        /// <summary>
        /// Triggers the <see cref="M:CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerCanExecuteChanged(EventArgs e)
        {
            EventHandler temp = this.CanExecuteChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="M:CommandExecuting"/> event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerExecution(CommandExecutingEventArgs e)
        {
            EventHandler<CommandExecutingEventArgs> temp = this.CommandExecuting;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="M:CommandCompleted"/> event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerCompletion(CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> temp = this.CommandCompleted;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="M:CommandFailed"/> event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerFailure(CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> temp = this.CommandFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion
    }
}
