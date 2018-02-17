﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubSubEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;
    using SAF.Data.Providers;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The fake sub sub entity repository.
    /// </summary>
    public class FakeSubSubEntityRepository : EntityRepository<FakeSubSubEntity, FakeSubSubRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubSubEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeSubSubEntityRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
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
        protected override ItemSelection<FakeSubSubRow> GetUniqueItemSelection(FakeSubSubRow item)
        {
            return this.GetKeySelection(item, row => row.FakeSubSubEntityId, row => row.UniqueName);
        }
    }
}
