﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake dependent entity repository.
    /// </summary>
    public class FakeDependentEntityRepository : EntityRepository<FakeDependentEntity, FakeDependentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDependentEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public FakeDependentEntityRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper, entity => entity.FakeDependentEntityId)
        {
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="ItemSelection{T}"/> for the specified item.
        /// </returns>
        protected override ItemSelection<FakeDependentRow> GetUniqueItemSelection(FakeDependentRow item)
        {
            return this.GetKeySelection(item, row => row.FakeDependentEntityId);
        }
    }
}