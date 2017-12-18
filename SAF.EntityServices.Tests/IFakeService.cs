// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFakeService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices.Tests
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using SAF.Testing.Common;

    /// <summary>
    /// The FakeService interface.
    /// </summary>
    [ServiceContract]
    public interface IFakeService
    {
        /// <summary>
        /// The throw unhandled exception.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        void ThrowUnhandledException(FakeDto item);

        /// <summary>
        /// The throw domain exception.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        void ThrowDomainException(FakeDto item);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDto"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        FakeDto Save(FakeDto item);

        /// <summary>
        /// The select item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDto"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        FakeDto SelectItem(FakeDto key);

        /// <summary>
        /// The select.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        List<FakeDto> Select(FakeQuery query);

        /// <summary>
        /// The remove item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        void RemoveItem(FakeDto key);

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(InternalOperationFault))]
        int Remove(FakeQuery query);
    }
}