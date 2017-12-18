// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelectionService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to services that select items from a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using SAF.Data;

    /// <summary>
    /// Provides an interface to services that select items from a repository.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The type of query that defines the selection.
    /// </typeparam>
    /// <typeparam name="TDto">
    /// The type of item to select.
    /// </typeparam>
    [ServiceContract]
    public interface ISelectionService<in TQuery, TDto>
        where TQuery : IExampleQuery<TDto>
    {
        /// <summary>
        /// Selects items from the repository.
        /// </summary>
        /// <param name="query">
        /// The selection query.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.List`1"/> of items matching the query.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        List<TDto> GetSelection(TQuery query);
    }
}