// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappedQueryConverter.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Data.Providers
{
    using System;
    using System.Linq;

    using SAF.Core;

    /// <summary>
    /// The mapped query converter.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item in the query.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of data item in the resulting item selection.
    /// </typeparam>
    public class MappedQueryConverter<TItem, TDataItem>
        where TDataItem : class, new()
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper;

        /// <summary>
        /// The property name mapper.
        /// </summary>
        private readonly IPropertyNameMapper<TDataItem> propertyNameMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappedQueryConverter{TItem,TDataItem}"/> class.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public MappedQueryConverter(IEntityMapper entityMapper)
            : this(entityMapper, Singleton<DirectPropertyNameMapper<TDataItem>>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappedQueryConverter{TItem,TDataItem}"/> class.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="propertyNameMapper">
        /// The property name mapper.
        /// </param>
        public MappedQueryConverter(IEntityMapper entityMapper, IPropertyNameMapper<TDataItem> propertyNameMapper)
        {
            this.entityMapper = entityMapper;
            this.propertyNameMapper = propertyNameMapper;
        }

        /// <summary>
        /// Converts a query into an item selection.
        /// </summary>
        /// <param name="query">
        /// The query to convert.
        /// </param>
        /// <returns>
        /// An item selection instance for the specified query.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public ItemSelection<TDataItem> Convert(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var propertyNameSelection = this.propertyNameMapper.Map(query);
            TransactSqlSelection<TDataItem> itemSelection;

            if (Evaluate.IsNull(query.BoundaryExample))
            {
                var example = query.BaselineExample;

                if (Evaluate.IsNull(example))
                {
                    return new TransactSqlSelection<TDataItem>(new TDataItem());
                }

                var dataItemExample = example as TDataItem ?? this.entityMapper.Map<TDataItem>(example);

                if (query.PropertiesToInclude.Any())
                {
                    itemSelection = new TransactSqlSelection<TDataItem>(dataItemExample, propertyNameSelection);
                }
                else
                {
                    // Map from example selections.
                    itemSelection = new TransactSqlSelection<TDataItem>(dataItemExample);
                }
            }
            else
            {
                var baseline = query.BaselineExample;
                var boundary = query.BoundaryExample;
                var dataItemBaseline = baseline as TDataItem ?? this.entityMapper.Map<TDataItem>(baseline);
                var dataItemBoundary = baseline as TDataItem ?? this.entityMapper.Map<TDataItem>(boundary);

                if (query.PropertiesToInclude.Any())
                {
                    itemSelection = new TransactSqlSelection<TDataItem>(dataItemBaseline, dataItemBoundary, propertyNameSelection);
                }
                else
                {
                    itemSelection = new TransactSqlSelection<TDataItem>(dataItemBaseline, dataItemBoundary);
                }
            }

            return itemSelection;
        }
    }
}
