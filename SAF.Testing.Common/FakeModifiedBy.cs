// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeModifiedBy.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    /// <summary>
    /// The fake modified by.
    /// </summary>
    public class FakeModifiedBy : FakeMultiReferenceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeModifiedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public FakeModifiedBy(string uniqueName)
            : base(uniqueName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeModifiedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeMultiReferenceEntityId">
        /// The fake multi reference entity id.
        /// </param>
        public FakeModifiedBy(string uniqueName, int? fakeMultiReferenceEntityId)
            : base(uniqueName, fakeMultiReferenceEntityId)
        {
        }
    }
}