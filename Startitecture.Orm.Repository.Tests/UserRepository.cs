﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The user repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The user repository.
    /// </summary>
    public class UserRepository : EntityRepository<User, UserRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public UserRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper, user => user.UserId)
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
        protected override ItemSelection<UserRow> GetUniqueItemSelection(UserRow item)
        {
            return this.GetKeySelection(item, row => row.UserId, row => row.AccountName);
        }

        /// <summary>
        /// Saves the dependencies of the specified entity prior to saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependencies and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected override void SaveDependencies(User entity, IRepositoryProvider provider, UserRow dataItem)
        {
            var personRepo = new PersonRepository(provider, this.EntityMapper);
            personRepo.Save(entity);
        }
    }
}