namespace SAF.ProcessEngine
{
    using System;
    using System.Threading;

    using SAF.Core;

    /// <summary>
    /// Executes a directive as a command.
    /// </summary>
    /// <typeparam name="T">The type of directive to execute.</typeparam>
    /// <remarks>
    /// This class has all the members needed to implement <see cref="T:System.Windows.Input.ICommand"/> but does not 
    /// to avoid requiring the PresentationCore.dll just to use the ProcessEngine namespace. Implementors can manually 
    /// add the interface implementation and override the <see cref="CanExecute(object)"/> method, and must provide change 
    /// detection for the <see cref="CanExecuteChanged"/> event.
    /// </remarks>
    public abstract class CommandBase<T> : IExecutable
    {
        #region Fields

        ////private readonly Dispatcher eventDispatcher;

        /// <summary>
        /// The parameters to use when executing the command.
        /// </summary>
        private T commandDirective = default(T);

        /// <summary>
        /// The message to return to the caller if the condition is not satisfied.
        /// </summary>
        private string conditionMessage;

        /// <summary>
        /// An object that provides synchronous access to the parameters.
        /// </summary>
        /// <remarks>
        /// The purpose of this object is to prevent the caller from changing the parameter reference during 
        /// execution. The parameters themselves may change and should either be synchronized by the caller
        /// or cloned in the ExecuteWithParameters method if required for thread safety.
        /// </remarks>
        private object directiveControl = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase&lt;T&gt;"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of this command.</param>
        protected CommandBase(string name)
            : this(name, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase&lt;T&gt;"/> class with the specified name, 
        /// condition and condition message.
        /// </summary>
        /// <param name="name">The name of this command.</param>
        /// <param name="conditionMessage">The message to return to the caller if the condition is not 
        /// satisfied.</param>
        protected CommandBase(string name, string conditionMessage)
            ////: this(name, conditionMessage, Dispatcher.CurrentDispatcher)
        {
            this.Name = name;
            this.conditionMessage =
                String.IsNullOrEmpty(conditionMessage) ?
                String.Format("{0} failed the pre-condition for execution", this.GetType().Name) : conditionMessage;

            this.TriggerCanExecuteChanged();
        }

        ////protected CommandBase(string name, string conditionMessage, Dispatcher eventDispatcher)
        ////{
        ////    this.eventDispatcher = eventDispatcher;

        ////}

        #endregion

        #region Delegates and Events

        /// <summary>
        /// Occurs immediately before the command executes. The event is fired in the <see cref="SynchronizationContext"/>
        /// of the thread that this command was created in.
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs immediately after the command executes.  The event is fired in the <see cref="SynchronizationContext"/>
        /// of the thread that this command was created in.
        /// </summary>
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when a command issued by the process has failed during execution. The event is fired in the 
        /// <see cref="SynchronizationContext"/> of the thread that this command was created in.
        /// </summary>
        public event EventHandler<CommandFailedEventArgs> CommandFailed;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute. The event is fired in the 
        /// <see cref="SynchronizationContext"/> of the thread that this command was created in.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the command will execute asynchronously (true) or synchronously 
        /// (false).
        /// </summary>
        public bool ExecuteAsync { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates whether the command can execute.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        /// <returns>True if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.CanExecute(this.commandDirective, parameter);
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        /// <remarks>
        /// The <see cref="CommandCompleted"/> and <see cref="CommandFailed"/> events signal command completion in both
        /// synchronous and asynchronous execution.
        /// </remarks>
        public void Execute(object parameter)
        {
            if (this.ExecuteAsync)
            {
                ExecuteCommand ced = new ExecuteCommand(this.InvokeCommand);
                ced.BeginInvoke(parameter, this.CommandExecutionCallback, ced);
            }
            else
            {
                this.InvokeCommand(parameter);
            }
        }

        /// <summary>
        /// Loads the directive for this command.
        /// </summary>
        /// <param name="directive">The directive for this command.</param>
        public void LoadDirective(T directive)
        {
            lock (this.directiveControl)
            {
                this.commandDirective = directive;
                this.TriggerCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the string representation of this command.
        /// </summary>
        /// <returns>The name of this command.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Triggers the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected void TriggerCanExecuteChanged()
        {
            ////if (this.eventDispatcher == null)
            ////{
                this.OnCanExecuteChanged(EventArgs.Empty);
            ////}
            ////else
            ////{
            ////    this.eventDispatcher.BeginInvoke(new Action(() => 
            ////    {
            ////        this.OnCanExecuteChanged(EventArgs.Empty);
            ////    }));
            ////}
        }

        /// <summary>
        /// Indicates whether the command can execute.
        /// </summary>
        /// <param name="directive">The directive for this command.</param>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        /// <returns>True if this command can be executed; otherwise, false.</returns>
        protected abstract bool CanExecute(T directive, object parameter);

        /// <summary>
        /// Executes the command with the provided parameters.
        /// </summary>
        /// <param name="directive">The parameters for this command.</param>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        protected abstract void InvokeCommand(T directive, object parameter);

        /// <summary>
        /// Invokes the execution of the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        private void InvokeCommand(object parameter)
        {
            this.OnCommandExecuting(new CommandExecutingEventArgs(this));
            Exception error = null;

            try
            {
                if (this.CanExecute(parameter))
                {
                    lock (this.directiveControl)
                    {
                        this.InvokeCommand(this.commandDirective, parameter);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        String.Format("The command execution pre-condition was not met: {0}", this.conditionMessage));
                }
            }
            catch (ArgumentException ex)
            {
                error = ex;
            }
            catch (InvalidOperationException ex)
            {
                error = ex;
            }
            catch (BusinessException ex)
            {
                error = ex;
            }
            catch (OperationException ex)
            {
                error = ex;
            }
            catch (AccessException ex)
            {
                error = ex;
            }
            catch (Exception ex)
            {
                error = ex;
                throw;
            }
            finally
            {
                if (error == null)
                {
                    this.OnCommandCompleted(new CommandCompletedEventArgs(this));
                }
                else
                {
                    this.OnCommandFailed(new CommandFailedEventArgs(this, error));
                    string message = String.Format(
                        "{0}{1} failed.",
                        this,
                        this.commandDirective == null ? String.Empty : String.Format(" '{0}'", this.commandDirective));

                    System.Diagnostics.Trace.TraceError("{0}: {1}", message, error);
                }
            }
        }

        /// <summary>
        /// Callback method for command execution.
        /// </summary>
        /// <param name="ar">Asynchronous result state.</param>
        [System.Diagnostics.DebuggerHidden]
        private void CommandExecutionCallback(IAsyncResult ar)
        {
            ExecuteCommand cd = (ExecuteCommand)ar.AsyncState;
            cd.EndInvoke(ar);
        }

        #region Event Methods

        /// <summary>
        /// Delegate for triggering events in the caller's event context.
        /// </summary>
        /// <param name="state">Event arguments to send to the event.</param>
        private void TriggerEvent(object state)
        {
            if (state is CommandExecutingEventArgs)
            {
                this.OnCommandExecuting(state as CommandExecutingEventArgs);
            }
            else if (state is CommandCompletedEventArgs)
            {
                this.OnCommandCompleted(state as CommandCompletedEventArgs);
            }
            else if (state is CommandFailedEventArgs)
            {
                this.OnCommandFailed(state as CommandFailedEventArgs);
            }
            else if (EventArgs.Empty.Equals(state))
            {
                this.OnCanExecuteChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers the CommandExecuting event.
        /// </summary>
        /// <param name="e">The <see cref="CommandExecutingEventArgs"/> associated with the event.</param>
        private void OnCommandExecuting(CommandExecutingEventArgs e)
        {
            EventHandler<CommandExecutingEventArgs> temp = this.CommandExecuting;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the CommandExecuted event.
        /// </summary>
        /// <param name="e">The <see cref="CommandCompletedEventArgs"/> associated with the event.</param>
        private void OnCommandCompleted(CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> temp = this.CommandCompleted;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="CommandFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CommandFailedEventArgs"/> associated with the event.</param>
        private void OnCommandFailed(CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> temp = this.CommandFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> associated with the event.</param>
        private void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler temp = this.CanExecuteChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #endregion
    }
}
