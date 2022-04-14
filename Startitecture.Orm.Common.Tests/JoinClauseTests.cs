// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinClauseTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="JoinClause"/> class.
    /// </summary>
    [TestClass]
    public class JoinClauseTests
    {
        /// <summary>
        /// Tests the Create method.
        /// </summary>
        [TestMethod]
        public void Create_RelationsWithInnerAndLeftImplicitJoins_TextMatchesExpected()
        {
            var target = new JoinClause(new DataAnnotationsDefinitionProvider(), new TransactSqlQualifier());
            var actual = target.Create(
                new EntityRelationSet<DomainAggregateRow>().InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                    .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                    .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                    .Relations);

            const string Expected = @"INNER JOIN [dbo].[DomainIdentity] AS [CreatedBy] ON [dbo].[DomainAggregate].[CreatedByDomainIdentityId] = [CreatedBy].[DomainIdentityId]
INNER JOIN [dbo].[SubContainer] ON [dbo].[DomainAggregate].[SubContainerId] = [dbo].[SubContainer].[SubContainerId]
LEFT JOIN [dbo].[AggregateOption] ON [dbo].[DomainAggregate].[DomainAggregateId] = [dbo].[AggregateOption].[AggregateOptionId]";

            Assert.AreEqual(Expected, actual);
        }
    }
}