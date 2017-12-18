// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestResult.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;

    /// <summary>
    /// The test result.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class TestResult<T> : TaskResult<T, bool>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult{T}"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public TestResult(T directive, bool result, Exception error)
            : base(directive, result, error)
        {
        }

        #endregion
    }
}