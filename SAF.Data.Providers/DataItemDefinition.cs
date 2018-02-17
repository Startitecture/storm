// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemDefinition.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines a specific type of data entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Defines a specific type of data entity.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of data entity to define.
    /// </typeparam>
    public class DataItemDefinition<TDataItem> : EntityDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemDefinition{TDataItem}"/> class.
        /// </summary>
        public DataItemDefinition()
            : base(Singleton<DataItemDefinitionProvider>.Instance, typeof(TDataItem))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemDefinition{TDataItem}"/> class.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        public DataItemDefinition([NotNull] EntityReference entityReference)
            : base(Singleton<DataItemDefinitionProvider>.Instance, entityReference)
        {
        }
    }
}