// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified field service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.StringResources;
    using SAF.Testing.Common;

    /// <summary>
    /// The unified field service.
    /// </summary>
    public class UnifiedFieldService
    {
        /// <summary>
        /// The field repository.
        /// </summary>
        private readonly IUnifiedFieldRepository fieldRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldService"/> class.
        /// </summary>
        /// <param name="fieldRepository">
        /// The field repository.
        /// </param>
        public UnifiedFieldService(IUnifiedFieldRepository fieldRepository)
        {
            this.fieldRepository = fieldRepository;
        }

        /// <summary>
        /// Selects all unified fields from the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> of <see cref="UnifiedField" /> items.
        /// </returns>
        public IEnumerable<UnifiedField> SelectAllFields()
        {
            return this.fieldRepository.SelectAllFields();
        }

        /// <summary>
        /// Selects fields by their source type.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UnifiedField"/> items matching the <paramref name="sourceType"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceType"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// <paramref name="sourceType"/> has a namespace that is not supported for Unified Fields.
        /// </exception>
        public IEnumerable<UnifiedField> SelectFields([NotNull] Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            var commonNamespace = typeof(Person).Namespace;
            var pocNamespace = typeof(Contract).Namespace;

            var supportedNamespaces = new List<string> { pocNamespace, commonNamespace };

            if (supportedNamespaces.Contains(sourceType.Namespace) == false)
            {
                throw new BusinessException(sourceType, FieldsMessages.UnifiedFieldSourceTypeNotSupported);
            }

            return this.fieldRepository.GetFieldsBySourceType(sourceType);
        }

        /// <summary>
        /// Saves the specified fields to the database.
        /// </summary>
        /// <param name="fields">
        /// The fields to save.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fields"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UnifiedField"/> items saved in the database.
        /// </returns>
        public IEnumerable<UnifiedField> SaveFields([NotNull] IEnumerable<UnifiedField> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var unifiedFields = fields as IList<UnifiedField> ?? fields.ToList();

            foreach (var field in unifiedFields)
            {
                Singleton<UnifiedFieldValidator>.Instance.ThrowOnValidationFailure(field);
            }

            return this.fieldRepository.SaveFields(unifiedFields);
        }
    }
}