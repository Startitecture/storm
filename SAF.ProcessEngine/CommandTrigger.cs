// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandTrigger.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Encapsulates an event that should trigger execution of a specific command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Encapsulates an event that should trigger execution of a specific command.
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="EventArgs"/> that represents the sender and event of the 
    /// trigger.</typeparam>
    public class CommandTrigger<TEvent> : EventTrigger<TEvent>, ICommandTrigger
        where TEvent : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTrigger&lt;TEvent&gt;"/> class with the specified command
        /// and trigger condition.
        /// </summary>
        /// <param name="command">The command that should be executed when the event occurs.</param>
        /// <param name="condition">The condition of the event trigger.</param>
        public CommandTrigger(IExecutable command, Func<object, TEvent, bool> condition)
            : base(condition)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command that the event should trigger.
        /// </summary>
        public IExecutable Command
        {
            get;
            private set;
        }
    }
}
