﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The person repository.
    /// </summary>
    public class PersonRepository : EntityRepository<Person, ActionPrincipalRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity Mapper.
        /// </param>
        public PersonRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper, person => person.PersonId)
        {
        }

        /// <summary>
        /// Selects people from the repository matching the specified <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection to make.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Person"/> items matching the <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public IEnumerable<Person> SelectPeople(ItemSelection<ActionPrincipalRow> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.SelectEntities(selection);
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
        protected override ItemSelection<ActionPrincipalRow> GetUniqueItemSelection(ActionPrincipalRow item)
        {
            return this.GetKeySelection(item, row => row.PersonId);
        }
    }
}