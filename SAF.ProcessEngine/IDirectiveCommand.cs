namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to a directive command.
    /// </summary>
    /// <remarks>
    /// This interface has all the members needed to implement <see cref="T:System.Windows.Input.ICommand"/> but does not 
    /// to avoid requiring the PresentationCore.dll just to use the ProcessEngine namespace. Implementors can manually 
    /// add the interface implementation and override the <see cref="CanExecute"/> method, and must provide change 
    /// detection for the <see cref="CanExecuteChanged"/> event.
    /// </remarks>
    public interface IExecutable : ICommandDispatcher
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets the name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the command will execute asynchronously (true) or synchronously 
        /// (false).
        /// </summary>
        bool ExecuteAsync { get; set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        void Execute(object parameter);

        /// <summary>
        /// Indicates whether the command can execute.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        /// <returns>True if this command can be executed; otherwise, false</returns>
        bool CanExecute(object parameter);
    }

    /////// <summary>
    /////// Provides an interface to a command directive.
    /////// </summary>
    /////// <typeparam name="T">The type of directive that the command executes.</typeparam>
    ////public interface IDirectiveCommand<T> : IDirectiveCommand
    ////{
    ////    /// <summary>
    ////    /// Loads the directive for this command.
    ////    /// </summary>
    ////    /// <param name="directive">The directive for this command.</param>
    ////    void LoadDirective(T directive);
    ////}
}
