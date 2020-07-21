// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultPageTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The result page tests.
    /// </summary>
    [TestClass]
    public class ResultPageTests
    {
        /// <summary>
        /// The set page test.
        /// </summary>
        [TestMethod]
        public void SetPage_PageNumber2_RowOffsetMatchesExpected()
        {
            var target = new ResultPage
                             {
                                 RowOffset = 3,
                                 Size = 14
                             };

            target.SetPage(2);
            Assert.AreEqual(14, target.RowOffset);
        }
    }
}