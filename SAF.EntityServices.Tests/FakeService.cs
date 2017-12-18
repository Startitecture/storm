// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The fake service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ExcludeFromCodeCoverage]
    public class FakeService : EntityServiceBase<FakeEntity>, IFakeService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeService"/> class.
        /// </summary>
        /// <param name="proxyFactory">
        /// The proxy factory.
        /// </param>
        public FakeService(IEntityProxyFactory proxyFactory)
            : base(proxyFactory)
        {
        }

        /// <summary>
        /// The throw unhandled exception.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void ThrowUnhandledException(FakeDto item)
        {
            this.EntityProxy.PerformAction(item, ThrowUnhandledExceptionForDto);
        }

        /// <summary>
        /// The throw domain exception.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void ThrowDomainException(FakeDto item)
        {
            this.EntityProxy.PerformAction(item, ThrowDomainExceptionForDto);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDto"/>.
        /// </returns>
        public FakeDto Save(FakeDto item)
        {
            return this.EntityProxy.SaveItem(item);
        }

        /// <summary>
        /// The select item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDto"/>.
        /// </returns>
        public FakeDto SelectItem(FakeDto key)
        {
            var selectEntity = this.EntityProxy.SelectEntity(key);

            return selectEntity == null ? null : this.EntityProxy.EntityMapper.Map<FakeDto>(selectEntity);
        }

        /// <summary>
        /// The select.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<FakeDto> Select(FakeQuery query)
        {
            return this.EntityProxy.SelectItems(query).ToList();
        }

        /// <summary>
        /// The remove item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void RemoveItem(FakeDto key)
        {
            this.EntityProxy.RemoveItem(key);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Remove(FakeQuery query)
        {
            return this.EntityProxy.RemoveSelection(query);
        }

        /// <summary>
        /// The throw unhandled exception for dto.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private static void ThrowUnhandledExceptionForDto(FakeDto dto)
        {
            throw new InvalidOperationException("Unhandled Exception");
        }

        /// <summary>
        /// The throw domain exception for dto.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <exception cref="OperationException">
        /// </exception>
        private static void ThrowDomainExceptionForDto(FakeDto dto)
        {
            throw new OperationException("Domain Exception");
        }
    }
}
