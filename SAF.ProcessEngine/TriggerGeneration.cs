namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides static methods for generating triggers.
    /// </summary>
    public static class TriggerGeneration
    {
        /// <summary>
        /// Generates process stop state triggers based on a condition.
        /// </summary>
        /// <typeparam name="TState">The type that represents the posssible states.</typeparam>
        /// <param name="condition">The condition that can be true or false.</param>
        /// <param name="newState">The state to trigger if the condition is true.</param>
        /// <returns>An array of state triggers for each condition.</returns>
        public static IStateTrigger<TState>[] CreateProcessStopTriggers<TState>(
            Func<object, ProcessStoppedEventArgs, bool> condition, 
            TState newState)
        {
            return 
                new IStateTrigger<TState>[] 
                {
                    new ProcessStopStateTrigger<TState>(newState, condition)
                };
        }

        /// <summary>
        /// Generates state triggers based on the specified command's execution events.
        /// </summary>
        /// <typeparam name="TState">The type that represents the posssible states.</typeparam>
        /// <param name="command">The command to register.</param>
        /// <param name="executingState">The state that command execution should trigger.</param>
        /// <param name="failedState">The state that command failure should trigger.</param>
        /// <param name="completeState">The state that command completion should trigger.</param>
        /// <returns>An array of state triggers for each condition.</returns>
        [System.Diagnostics.DebuggerHidden]
        public static IStateTrigger<TState>[] CreateCommandStateTriggers<TState>(
            IExecutable command, 
            TState executingState, 
            TState failedState, 
            TState completeState)
        {
            return
                new IStateTrigger<TState>[]
                {
                    new CommandExecutingStateTrigger<TState>(executingState, command),
                    new CommandFailedStateTrigger<TState>(failedState, command),
                    new CommandCompletedStateTrigger<TState>(completeState, command)
                };
        }
    }
}
