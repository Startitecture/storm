namespace SAF.ProcessEngine
{
    /// <summary>
    /// Delegate for asynchronous command execution.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
    /// object can be set to null.</param>
    public delegate void ExecuteCommand(object parameter);
}
