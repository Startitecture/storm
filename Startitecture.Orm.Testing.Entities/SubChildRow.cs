// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubChildRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    /// <summary>
    /// The fake sub child row.
    /// </summary>
    public class SubChildRow
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