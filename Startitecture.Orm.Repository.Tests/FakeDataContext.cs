// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake data context.
    /// </summary>
    public class FakeDataContext : Database
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDataContext"/> class.
        /// </summary>
        public FakeDataContext()
            : this(new PetaPocoDefinitionProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDataContext"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public FakeDataContext(IEntityDefinitionProvider definitionProvider)
            : base("TestConnection", definitionProvider)
        {
        }
    }
}
