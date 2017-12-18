// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCommand.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;
    using System.Threading;

    /// <summary>
    /// The test command.
    /// </summary>
    public class TestCommand : CommandBase<TimeSpan>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCommand"/> class.
        /// </summary>
        public TestCommand()
            : base("Test")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool CanExecute(TimeSpan directive, object parameter)
        {
            return true;
        }

        /// <summary>
        /// The invoke command.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        protected override void InvokeCommand(TimeSpan parameters, object parameter)
        {
            Thread.Sleep(parameters.Duration());
        }

        #endregion
    }
}