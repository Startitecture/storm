// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Mapper;

    /// <summary>
    /// The fake data context.
    /// </summary>
    public class FakeDataContext : Database
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDataContext"/> class.
        /// </summary>
        public FakeDataContext()
            : base("TestConnection")
        {
        }
    }
}
