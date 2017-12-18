// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
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