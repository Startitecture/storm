// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedChildEntityRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System.Collections.Generic;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The fake raised child entity repository.
    /// </summary>
    public class FakeRaisedChildEntityRepository : EntityRepository<FakeChildEntity, FakeRaisedChildRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedChildEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeRaisedChildEntityRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider, SelectionComparer.SomeValue)
        {
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<FakeRaisedChildRow> GetUniqueItemSelection(FakeRaisedChildRow item)
        {
            return this.GetKeySelection(item, row => row.FakeChildEntityId, row => row.FakeComplexEntityId, row => row.Name);
        }

        /// <summary>
        /// The selection comparer.
        /// </summary>
        private class SelectionComparer : Comparer<FakeRaisedChildRow>
        {
            /// <summary>
            /// The some value comparer.
            /// </summary>
            private static readonly SelectionComparer SomeValueComparer = new SelectionComparer();

            /// <summary>
            /// Gets a comparer that orders by some value.
            /// </summary>
            public static SelectionComparer SomeValue
            {
                get
                {
                    return SomeValueComparer;
                }
            }

            /// <summary>
            /// When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
            /// indicating whether one object is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the
            /// following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero
            /// <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than
            /// <paramref name="y"/>.
            /// </returns>
            /// <param name="x">
            /// The first object to compare.
            /// </param>
            /// <param name="y">
            /// The second object to compare.
            /// </param>
            public override int Compare(FakeRaisedChildRow x, FakeRaisedChildRow y)
            {
                return Evaluate.Compare(x, y, entity => entity.FakeComplexEntityId, entity => entity.SomeValue);
            }
        }
    }
}