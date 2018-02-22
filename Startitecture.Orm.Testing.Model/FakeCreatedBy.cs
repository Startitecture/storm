// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeCreatedBy.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The fake created by.
    /// </summary>
    public class FakeCreatedBy : FakeMultiReferenceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreatedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public FakeCreatedBy(string uniqueName)
            : base(uniqueName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreatedBy"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeMultiReferenceEntityId">
        /// The fake multi reference entity id.
        /// </param>
        public FakeCreatedBy(string uniqueName, int? fakeMultiReferenceEntityId)
            : base(uniqueName, fakeMultiReferenceEntityId)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FakeCreatedBy"/> class from being created.
        /// </summary>
        private FakeCreatedBy()
            : base(null)
        {
        }
    }
}