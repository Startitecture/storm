// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestEngine.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The test engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    /// <summary>
    /// The test engine.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item being processed.
    /// </typeparam>
    public class TestEngine<T> : TaskEngine<T, TestResult<T>>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(T item)
        {
            this.QueueTask(item);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The consume item.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <returns>
        /// A test result.
        /// </returns>
        protected override TestResult<T> ConsumeItem(T directive)
        {
            return new TestResult<T>(directive, true, null);
        }

        #endregion
    }
}