// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

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
        public PersonRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
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
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<ActionPrincipalRow> GetUniqueItemSelection(ActionPrincipalRow item)
        {
            return this.GetKeySelection(item, row => row.PersonId);
        }
    }
}