// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The fake data entity.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeDataEntity
    {
        /// <summary>
        /// Gets or sets the fake entity id.
        /// </summary>
        public int FakeEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        public DateTime DateValue { get; set; }
    }
}