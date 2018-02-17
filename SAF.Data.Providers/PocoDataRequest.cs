// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PocoDataRequest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Data;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Defines a POCO data request for a POCO factory.
    /// </summary>
    public class PocoDataRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PocoDataRequest"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data record.
        /// </param>
        /// <param name="pocoType">
        /// The POCO data type for the data record.
        /// </param>
        public PocoDataRequest([NotNull] IDataReader dataReader, [NotNull] Type pocoType)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException(nameof(dataReader));
            }

            if (pocoType == null)
            {
                throw new ArgumentNullException(nameof(pocoType));
            }

            this.DataReader = dataReader;
            this.EntityDefinition = Singleton<DataItemDefinitionProvider>.Instance.Resolve(pocoType);
        }

        /// <summary>
        /// Gets the data record.
        /// </summary>
        public IDataReader DataReader { get; private set; }

        /// <summary>
        /// Gets the entity definition.
        /// </summary>
        public IEntityDefinition EntityDefinition { get; }

        /// <summary>
        /// Gets or sets the field count.
        /// </summary>
        public int FieldCount { get; set; }

        /// <summary>
        /// Gets or sets the first column to read.
        /// </summary>
        public int FirstColumn { get; set; }
    }
}