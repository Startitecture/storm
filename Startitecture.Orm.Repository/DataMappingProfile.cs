// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The data mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// The data mapping profile.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of data item to map.
    /// </typeparam>
    public class DataMappingProfile<TDataItem> : Profile
        where TDataItem : ITransactionContext
    {
        /// <summary>
        /// The row to row mapping expression.
        /// </summary>
        private readonly IMappingExpression<TDataItem, TDataItem> rowToRowMappingExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMappingProfile{TDataItem}"/> class.
        /// </summary>
        public DataMappingProfile()
        {
            this.rowToRowMappingExpression = this.CreateMap<TDataItem, TDataItem>();
        }

        /// <summary>
        /// Sets a key for the data item from one of the data item properties. There can only be one key per 
        /// <typeparamref name="TDataKey"/> type.
        /// </summary>
        /// <param name="dataItemKey">
        /// The data Item Key.
        /// </param>
        /// <typeparam name="TDataKey">
        /// The type of the data key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="DataMappingProfile{T}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public virtual DataMappingProfile<TDataItem> SetPrimaryKey<TDataKey>(
            Expression<Func<TDataItem, TDataKey>> dataItemKey)
        {
            if (dataItemKey == null)
            {
                throw new ArgumentNullException(nameof(dataItemKey));
            }

            // Associate a link between the entity's key type and the data item.
            this.CreateMap<TDataItem, TDataKey>().ConstructUsing(dataItemKey.Compile());

            // Create a mapping between the entity key type and the entity so that the key will be applied to the entity.
            this.SetUniqueKey(dataItemKey);
            this.rowToRowMappingExpression.ForMember(dataItemKey.GetPropertyName(), expr => expr.Ignore());
            return this;
        }

        /// <summary>
        /// Sets the data item properties that should only be written once, such as creation information.
        /// </summary>
        /// <param name="writeOnceColumns">
        /// The write once columns.
        /// </param>
        /// <returns>
        /// The current <see cref="DataMappingProfile{T}"/>.
        /// </returns>
        public virtual DataMappingProfile<TDataItem> WriteOnce([NotNull] params Expression<Func<TDataItem, object>>[] writeOnceColumns)
        {
            if (writeOnceColumns == null)
            {
                throw new ArgumentNullException(nameof(writeOnceColumns));
            }

            foreach (var expression in writeOnceColumns)
            {
                this.rowToRowMappingExpression.ForMember(expression.GetPropertyName(), expr => expr.Ignore());
            }

            return this;
        }

        /// <summary>
        /// Sets a key for the data item from one of the data item properties. There can only be one key per 
        /// <typeparamref name="TKey"/> type.
        /// </summary>
        /// <param name="key">
        /// The key to set.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="DataMappingProfile{T}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public DataMappingProfile<TDataItem> SetUniqueKey<TKey>(Expression<Func<TDataItem, TKey>> key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (typeof(TKey) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            this.ConfigureKey(key);
            return this;
        }

        /// <summary>
        /// Sets a compound key from the properties specified in the related entity.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type that contains the key property.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="DataMappingProfile{T}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public DataMappingProfile<TDataItem> SetDependencyKey<TDependency, TKey>(Expression<Func<TDependency, TKey>> key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (typeof(TDependency) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            this.CreateMap<TDependency, TDataItem>()
                .ForMember(key.GetPropertyName(), expr => expr.MapFrom(key))
                .ForAllOtherMembers(expr => expr.Ignore());

            return this;
        }

        /// <summary>
        /// Sets a compound key from the properties specified in the related entity.
        /// </summary>
        /// <param name="sourceKey">
        /// The source key in the dependency.
        /// </param>
        /// <param name="targetKey">
        /// The target key in the current data item.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type that contains the key property.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="DataMappingProfile{T}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public DataMappingProfile<TDataItem> SetDependencyKey<TDependency, TKey>(
            Expression<Func<TDependency, TKey>> sourceKey,
            Expression<Func<TDataItem, object>> targetKey)
        {
            if (sourceKey == null)
            {
                throw new ArgumentNullException(nameof(sourceKey));
            }

            if (targetKey == null)
            {
                throw new ArgumentNullException(nameof(targetKey));
            }

            if (typeof(TDependency) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            this.CreateMap<TDependency, TDataItem>()
                .ForMember(targetKey, expr => expr.MapFrom(sourceKey))
                .ForAllOtherMembers(expr => expr.Ignore());

            return this;
        }
    }
}
