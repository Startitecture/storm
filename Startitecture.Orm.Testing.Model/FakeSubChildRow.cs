// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubChildRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The fake sub child row.
    /// </summary>
    public class FakeSubChildRow
    {
        /// <summary>
        /// Gets or sets the fake sub child entity id.
        /// </summary>
        public int FakeSubChildEntityId { get; set; }

        /// <summary>
        /// Gets or sets the parent fake child entity id.
        /// </summary>
        public int ParentFakeChildEntityId { get; set; }
    }
}