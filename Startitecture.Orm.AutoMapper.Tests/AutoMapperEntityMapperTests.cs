// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperEntityMapperTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The auto mapper entity mapper tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.AutoMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using global::AutoMapper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The auto mapper entity mapper tests.
    /// </summary>
    [TestClass]
    public class AutoMapperEntityMapperTests
    {
        /// <summary>
        /// The configuration provider.
        /// </summary>
        private static readonly IConfigurationProvider MappingConfigurationProvider =
            new MapperConfiguration(expression => expression.AddProfile<FieldValueMappingProfile>());

        /// <summary>
        /// The map test.
        /// </summary>
        [TestMethod]
        public void Map_FieldValueListWithSameReferenceParent_ReferenceMaintainedInMapping()
        {
            var target = new AutoMapperEntityMapper(new Mapper(MappingConfigurationProvider));

            var fieldRow = new FieldRow { FieldId = 234, Name = "Field1", Description = "The first field" };
            var domainIdentity1Row = new DomainIdentityRow { DomainIdentityId = 134, FirstName = "Tom", LastName = "Bomb", UniqueIdentifier = "tom@google.com" };
            var domainIdentity2Row = new DomainIdentityRow { DomainIdentityId = 765, FirstName = "Tim", LastName = "Bo", UniqueIdentifier = "tim@google.com" };

            var fieldValueRows = new List<FieldValueRow>
                                     {
                                         new FieldValueRow
                                             {
                                                 Field = fieldRow,
                                                 FieldId = fieldRow.FieldId,
                                                 FieldValueId = 93453,
                                                 LastModifiedTime = DateTimeOffset.Now,
                                                 LastModifiedBy = domainIdentity1Row,
                                                 LastModifiedByDomainIdentifierId = domainIdentity1Row.DomainIdentityId
                                             },
                                         new FieldValueRow
                                             {
                                                 Field = fieldRow,
                                                 FieldId = fieldRow.FieldId,
                                                 FieldValueId = 93454,
                                                 LastModifiedTime = DateTimeOffset.Now,
                                                 LastModifiedBy = domainIdentity1Row,
                                                 LastModifiedByDomainIdentifierId = domainIdentity1Row.DomainIdentityId
                                             },
                                         new FieldValueRow
                                             {
                                                 Field = fieldRow,
                                                 FieldId = fieldRow.FieldId,
                                                 FieldValueId = 93455,
                                                 LastModifiedTime = DateTimeOffset.Now,
                                                 LastModifiedBy = domainIdentity2Row,
                                                 LastModifiedByDomainIdentifierId = domainIdentity2Row.DomainIdentityId
                                             },
                                         new FieldValueRow
                                             {
                                                 Field = fieldRow,
                                                 FieldId = fieldRow.FieldId,
                                                 FieldValueId = 93456,
                                                 LastModifiedTime = DateTimeOffset.Now,
                                                 LastModifiedBy = domainIdentity2Row,
                                                 LastModifiedByDomainIdentifierId = domainIdentity2Row.DomainIdentityId
                                             },
                                         new FieldValueRow
                                             {
                                                 Field = fieldRow,
                                                 FieldId = fieldRow.FieldId,
                                                 FieldValueId = 93457,
                                                 LastModifiedTime = DateTimeOffset.Now,
                                                 LastModifiedBy = domainIdentity2Row,
                                                 LastModifiedByDomainIdentifierId = domainIdentity2Row.DomainIdentityId
                                             },
                                     };

            var values = new List<FieldValue>();

            Trace.WriteLine("test");

            foreach (var row in fieldValueRows)
            {
                values.Add(target.Map<FieldValue>(row));
            }

            var firstField = values.First().Field;

            foreach (var value in values)
            {
                Assert.AreSame(firstField, value.Field);
            }
        }
    }
}