// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data.Providers;

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
