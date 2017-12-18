// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTrigger.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test trigger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;

    /// <summary>
    /// The test trigger.
    /// </summary>
    /// <typeparam name="TEvent">
    /// </typeparam>
    internal class TestTrigger<TEvent> : EventTrigger<TEvent>
        where TEvent : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTrigger{TEvent}"/> class.
        /// </summary>
        public TestTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTrigger{TEvent}"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public TestTrigger(Func<object, TEvent, bool> condition)
            : base(condition)
        {
        }

        #endregion
    }
}