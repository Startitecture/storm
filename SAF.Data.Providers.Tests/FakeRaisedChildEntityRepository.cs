// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedChildEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System.Collections.Generic;

    using SAF.Testing.Common;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

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
        /// Selects the child rows for the complex entity with the specified ID.
        /// </summary>
        /// <param name="complexEntityId">
        /// The complex entity ID.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FakeChildEntity"/> items for the specified <paramref name="complexEntityId"/>.
        /// </returns>
        public IEnumerable<FakeChildEntity> SelectForComplexEntity(int complexEntityId)
        {
            return this.SelectEntities(Query.From<FakeRaisedChildRow>().Matching(row => row.FakeComplexEntityId, complexEntityId));
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