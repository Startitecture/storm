// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperEntityMapperTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// This is a test class for AutoMapperEntityMapperTest and is intended
    /// to contain all AutoMapperEntityMapperTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AutoMapperEntityMapperTest
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            configuration =>
                {
                    configuration.CreateMap<FakeDto, FakeEntity>();
                    configuration.CreateMap<FakeEntity, FakeDataEntity>().ForMember(
                        dest => dest.FakeEntityId,
                        opt => opt.MapFrom(x => x.FakeEntityId.GetValueOrDefault()));

                    configuration.CreateMap<FakeDataEntity, FakeEntity>();
                    configuration.CreateMap<FakeEntity, FakeDto>().ForMember(dest => dest.TransactionProvider, expr => expr.Ignore());

                    configuration.CreateMap<FakeDto, BadMappingDto>();
                });

        #region Public Methods and Operators

        /// <summary>
        /// A test for Map
        /// </summary>
        [TestMethod]
        public void Map_WithBadMapping_ContainsErrorData()
        {
            var target = this.entityMapper;
            var item = new FakeDto { Name = "UnsuspectingDto", Description = "About to get borked", DateValue = DateTime.Now };

            try
            {
                target.Map<BadMappingDto>(item);
                Assert.Fail("No exception was thrown.");
            }
            catch (OperationException ex)
            {
                Assert.IsNotNull(ex.Data, "AdditionalData was null");
                Assert.AreEqual(ex.Data["{Item}"], item.ToString());

                foreach (var propertyInfo in item.GetType().GetNonIndexedProperties())
                {
                    Assert.AreEqual(item.GetPropertyValue(propertyInfo.Name), ex.Data[propertyInfo.Name]);
                }
            }
        }

        /// <summary>
        /// A test for MapTo
        /// </summary>
        [TestMethod]
        public void MapTo_WithBadMapping_ContainsErrorData()
        {
            var target = this.entityMapper;
            var item = new FakeDto { Name = "UnsuspectingDto", Description = "About to get borked", DateValue = DateTime.Now };

            try
            {
                var badMapping = new BadMappingDto();
                target.MapTo(item, badMapping);
                Assert.Fail("No exception was thrown.");
            }
            catch (OperationException ex)
            {
                Assert.IsNotNull(ex.Data, "AdditionalData was null");
                Assert.AreEqual(ex.Data["{Item}"], item.ToString());

                foreach (var propertyInfo in item.GetType().GetNonIndexedProperties())
                {
                    Assert.AreEqual(item.GetPropertyValue(propertyInfo.Name), ex.Data[propertyInfo.Name]);
                }
            }
        }

        /// <summary>
        /// A test for MapFrom
        /// </summary>
        [TestMethod]
        public void Map_AlreadyMappedEntity_MatchesExpected()
        {
            var target = this.entityMapper;
            var source = new FakeDto { FakeEntityId = 1, Name = "Name", Description = "Description", DateValue = DateTime.MinValue };

            var expected = new FakeEntity
                               {
                                   FakeEntityId = 1,
                                   Name = "Name",
                                   Description = "Description",
                                   DateValue = DateTime.MinValue
                               };

            var actual = target.Map<FakeEntity>(source);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for MapFrom
        /// </summary>
        [TestMethod]
        public void Map_NonMappedEntity_ThrowsException()
        {
            var target = this.entityMapper;
            const string Source = "This is obviously not going to work.";

            OperationException expected = null;

            try
            {
                target.Map<FakeEntity>(Source);
            }
            catch (OperationException ex)
            {
                expected = ex;
            }

            Assert.IsNotNull(expected);
        }

        /// <summary>
        /// A test for MapFrom
        /// </summary>
        [TestMethod]
        public void MapTo_AlreadyMappedEntity_MatchesExpected()
        {
            var target = this.entityMapper;
            var source = new FakeEntity { FakeEntityId = 1, Name = "Name", Description = "Description", DateValue = DateTime.MinValue };
            var output = new FakeDto { FakeEntityId = 1, Name = "Name", Description = "Description", DateValue = DateTime.MinValue };
            var expected = new FakeDto { FakeEntityId = 1, Name = "Name", Description = "Description", DateValue = DateTime.MinValue };

            var actual = target.MapTo(source, output);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for MapFrom
        /// </summary>
        [TestMethod]
        public void MapTo_NonMappedEntity_ThrowsException()
        {
            var target = this.entityMapper;
            var source = new FakeEntity { FakeEntityId = 0, Name = "Name", Description = "Description", DateValue = DateTime.MaxValue };

            OperationException expected = null;

            try
            {
                target.MapTo(5, source);
            }
            catch (OperationException ex)
            {
                expected = ex;
            }

            Assert.IsNotNull(expected);
        }

        #endregion
    }
}