// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSender.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test sender.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    /// <summary>
    /// The test sender.
    /// </summary>
    internal class TestSender
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSender"/> class.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public TestSender(bool parameter)
        {
            this.TestParameter = parameter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether test parameter.
        /// </summary>
        public bool TestParameter { get; private set; }

        #endregion
    }
}