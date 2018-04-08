// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The unified field repository.
    /// </summary>
    public class UnifiedFieldRepository : EntityRepository<UnifiedField, UnifiedFieldRow>, IUnifiedFieldRepository
    {
        /// <summary>
        /// The command provider.
        /// </summary>
        private readonly IStructuredCommandProvider commandProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="commandProvider">
        /// The command provider.
        /// </param>
        public UnifiedFieldRepository([NotNull] IRepositoryProvider repositoryProvider, [NotNull] IStructuredCommandProvider commandProvider)
            : base(repositoryProvider)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (commandProvider == null)
            {
                throw new ArgumentNullException(nameof(commandProvider));
            }

            this.commandProvider = commandProvider;
        }

        /// <summary>
        /// Gets all of the unified fields in the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> of <see cref="UnifiedField" /> items.
        /// </returns>
        public IEnumerable<UnifiedField> SelectAllFields()
        {
            return this.SelectAll();
        }

        /// <summary>
        /// Gets all of the unified fields matching the specified <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="sourceType">
        /// The field source type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UnifiedField"/> items matching the <paramref name="sourceType"/>
        /// </returns>
        public IEnumerable<UnifiedField> GetFieldsBySourceType([NotNull] Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            var itemSelection = Select.From<UnifiedFieldRow>().WhereEqual(row => row.SourceType, sourceType.FullName);
            return this.SelectEntities(itemSelection);
        }

        /// <summary>
        /// Saves all of the specified fields into the repository.
        /// </summary>
        /// <param name="fields">
        /// The fields to save.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of saved <see cref="UnifiedField"/> items.
        /// </returns>
        public IEnumerable<UnifiedField> SaveFields([NotNull] IEnumerable<UnifiedField> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var fieldDictionary = fields.ToDictionary(x => x.Name, x => x);

            var transaction = this.RepositoryProvider.StartTransaction();
            var tableLoader = Singleton<DataTableLoader<UnifiedFieldRow>>.Instance;
            var dataTable = tableLoader.Load(fieldDictionary.Values, this.EntityMapper);

            var structuredMergeCommand =
                new StructuredMergeCommand<UnifiedFieldRow>(this.commandProvider, transaction)
                    .MergeInto<UnifiedFieldRow>(row => row.Name)
                    .SelectFromInserted(row => row.Name);

            using (var reader = structuredMergeCommand.ExecuteReader(dataTable))
            {
                var unifiedFieldIdOrdinal = reader.GetOrdinal(nameof(UnifiedFieldRow.UnifiedFieldId));
                var nameOrdinal = reader.GetOrdinal(nameof(UnifiedFieldRow.Name));

                while (reader.Read())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    var unifiedFieldId = (int)values[unifiedFieldIdOrdinal];
                    var name = (string)values[nameOrdinal];

                    this.EntityMapper.MapTo(unifiedFieldId, fieldDictionary[name]);
                }
            }

            this.RepositoryProvider.CompleteTransaction();

            return fieldDictionary.Values;
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<UnifiedFieldRow> GetUniqueItemSelection([NotNull] UnifiedFieldRow item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.GetKeySelection(item, row => row.UnifiedFieldId, row => row.Name);
        }
    }
}
