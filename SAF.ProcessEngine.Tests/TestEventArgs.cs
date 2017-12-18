// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestEventArgs.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;

    /// <summary>
    /// The test event args.
    /// </summary>
    public class TestEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEventArgs"/> class.
        /// </summary>
        /// <param name="testNumber">
        /// The test number.
        /// </param>
        public TestEventArgs(int testNumber)
        {
            this.TestNumber = testNumber;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the test number.
        /// </summary>
        public int TestNumber { get; private set; }

        #endregion
    }
}