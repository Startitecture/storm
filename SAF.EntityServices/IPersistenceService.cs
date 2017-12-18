// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistenceService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to services that persist objects using a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System.ServiceModel;

    /// <summary>
    /// Provides an interface to services that persist objects using a repository.
    /// </summary>
    /// <typeparam name="TDto">
    /// The type of data transfer object (DTO) used to transfer data across the service boundary.
    /// </typeparam>
    [ServiceContract]
    public interface IPersistenceService<TDto>
    {
        /// <summary>
        /// Saves an item into the repository.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <returns>
        /// A data transfer object (DTO) representing the new item.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        TDto Save(TDto item);

        /// <summary>
        /// Determines whether the specified item is contained in the repository.
        /// </summary>
        /// <param name="item">
        /// The item to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists; otherwise, <c>false</c>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        bool Contains(TDto item);

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ApplicationConfigurationFault))]
        [FaultContract(typeof(EntityValidationFault))]
        [FaultContract(typeof(EntityRepositoryFault))]
        [FaultContract(typeof(InternalOperationFault))]
        bool Remove(TDto item);
    }
}