// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiedBy.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The fake modified by.
    /// </summary>
    public class ModifiedBy : MultiReferenceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public ModifiedBy(string uniqueName)
            : base(uniqueName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeMultiReferenceEntityId">
        /// The fake multi reference entity id.
        /// </param>
        public ModifiedBy(string uniqueName, int? fakeMultiReferenceEntityId)
            : base(uniqueName, fakeMultiReferenceEntityId)
        {
        }
    }
}