// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.StringResources;

    /// <summary>
    /// A service proxy for entity repositories. Handles common methods and throws <see cref="FaultException"/>s.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public class EntityProxy<TEntity> : IEntityProxy<TEntity>
        where TEntity : IValidatingEntity
    {
        #region Constants

        /// <summary>
        /// The example format.
        /// </summary>
        private const string ExampleFormat = "values like '{0}'";

        /// <summary>
        /// The range description format.
        /// </summary>
        private const string RangeDescriptionFormat = "values between '{0}' and '{1}'";

        /// <summary>
        /// The updated values format.
        /// </summary>
        private const string SavedValuesFormat = "Saved values: '{0}'";

        /// <summary>
        /// The updated values format.
        /// </summary>
        private const string UpdatedValuesFormat = "Updated values: '{0}'";

        #endregion

        #region Fields

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper;

        /// <summary>
        /// The provider factory.
        /// </summary>
        private readonly IRepositoryProviderFactory providerFactory;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly IRepositoryFactory<TEntity> repositoryFactory;

        /// <summary>
        /// A list of action types that do not involve a specific target item.
        /// </summary>
        private readonly List<EntityActionType> targetlessActions = new List<EntityActionType>
                                                                        {
                                                                            EntityActionType.RemoveAll, 
                                                                            EntityActionType.SelectAll, 
                                                                            EntityActionType.UpdateAll
                                                                        };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.EntityServices.EntityProxy`1"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory for the proxy.
        /// </param>
        /// <param name="providerFactory">
        /// The repository provider factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        public EntityProxy(
            IRepositoryFactory<TEntity> repositoryFactory,
            IRepositoryProviderFactory providerFactory,
            IEntityMapper entityMapper,
            IActionEventProxy actionEventProxy)
        {
            this.actionEventProxy = actionEventProxy;
            this.repositoryFactory = repositoryFactory;
            this.providerFactory = providerFactory;
            this.entityMapper = entityMapper;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the entity mapper for the current entity proxy.
        /// </summary>
        public IEntityMapper EntityMapper
        {
            get
            {
                return this.entityMapper;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(TItem item, Action<TItem> action)
        {
            this.actionEventProxy.PerformAction(item, action);
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="request">
        /// The action request.
        /// </param>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(ActionRequest request, TItem item, Action<TItem> action)
        {
            this.actionEventProxy.PerformAction(request, item, action);
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, TResult> action)
        {
            return this.actionEventProxy.PerformActionWithResult(item, action);
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, Guid, TResult> action)
        {
            return this.actionEventProxy.PerformActionWithResult(item, action);
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="request">
        /// The action request.
        /// </param>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(ActionRequest request, TItem item, Func<TItem, TResult> action)
        {
            return this.actionEventProxy.PerformActionWithResult(request, item, action);
        }

        /// <summary>
        /// Records an action event.
        /// </summary>
        /// <param name="actionEvent">
        /// The action request.
        /// </param>
        public void RecordAction(ActionEvent actionEvent)
        {
            this.actionEventProxy.RecordAction(actionEvent);
        }

        /// <summary>
        /// Performs an action on the specified item with the action identifier, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(TItem item, Action<TItem, Guid> action)
        {
            this.actionEventProxy.PerformAction(item, action);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item)
        {
            this.actionEventProxy.RecordAction(item);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action that was taken.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Delegate action)
        {
            this.actionEventProxy.RecordAction(item, action);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action that was taken.
        /// </param>
        /// <param name="actionError">
        /// The action error.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Delegate action, Exception actionError)
        {
            this.actionEventProxy.RecordAction(item, action);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="actionName">
        /// The name of the action that was taken.
        /// </param>
        /// <param name="description">
        /// The description of the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, string actionName, string description)
        {
            this.actionEventProxy.RecordAction(item, actionName, description);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="actionError">
        /// The error associated with the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Exception actionError)
        {
            this.actionEventProxy.RecordAction(item, actionError);
        }

        /// <summary>
        /// Determines whether the specified item is contained in the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties of the entity.
        /// </typeparam>
        /// <param name="item">
        /// The item containing the properties to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.SelectOne, item);

            return this.actionEventProxy.PerformActionWithResult(request, item, this.ContainsItem);
        }

        /// <summary>
        /// Removes a selection of items from the repository.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <see cref="TEntity"/> remove.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool RemoveItem<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.RemoveOne, item);

            return this.actionEventProxy.PerformActionWithResult(request, item, this.RemoveItemFromRepository);
        }

        /// <summary>
        /// Removes a selection of items from the repository.
        /// </summary>
        /// <param name="query">
        /// The selection query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <see cref="TEntity"/> remove.
        /// </typeparam>
        /// <returns>
        /// The number of items affected by the removal.
        /// </returns>
        public int RemoveSelection<TItem>(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            ActionRequest request = Evaluate.IsNull(query.BaselineExample)
                                        ? this.CreateActionRequest(EntityActionType.RemoveAll, query.BaselineExample)
                                        : this.CreateActionRequest(
                                            EntityActionType.RemoveAny, query.BaselineExample, query.BoundaryExample, query);

            return this.PerformActionWithResult(request, query, this.RemoveSelectedItems);
        }

        /// <summary>
        /// Saves an item into the repository and returns the repository entity.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> to save.
        /// </typeparam>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/>.
        /// </returns>
        public TEntity SaveEntity<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.Save, item);

            return this.PerformActionWithResult(request, item, this.SaveEntityToRepository);
        }

        /// <summary>
        /// Saves an item into the repository and returns the saved item.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> to save.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TItem"/> that represents the saved <typeparamref name="TEntity"/> item.
        /// </returns>
        public TItem SaveItem<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.Save, item);

            return this.PerformActionWithResult(request, item, this.SaveItemToRepository);
        }

        /// <summary>
        /// Selects a single item from the repository.
        /// </summary>
        /// <param name="item">
        /// An item that contains the unique ID or key of the item to retrieve.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <see cref="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/> that matches the unique properties of <paramref name="item"/>, or null if no match is found.
        /// </returns>
        public TEntity SelectEntity<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.SelectOne, item);
            return this.PerformActionWithResult(request, item, this.GetFirstOrDefault);
        }

        /// <summary>
        /// Selects items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A collection of items matching the query.
        /// </returns>
        public IEnumerable<TEntity> SelectEntities<TItem>(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            ActionRequest request = Evaluate.IsNull(query.BaselineExample)
                                        ? this.CreateActionRequest(EntityActionType.SelectAll, query.BaselineExample, query)
                                        : this.CreateActionRequest(
                                            EntityActionType.SelectAny, query.BaselineExample, query.BoundaryExample, query);

            return this.PerformActionWithResult(request, query, this.SelectEntityList);
        }

        /// <summary>
        /// Selects a page of items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A page of items matching the query.
        /// </returns>
        public Page<TEntity> SelectEntityPage<TItem>(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            ActionRequest request = Evaluate.IsNull(query.BaselineExample)
                                        ? this.CreateActionRequest(EntityActionType.SelectAll, query.BaselineExample, query)
                                        : this.CreateActionRequest(
                                            EntityActionType.SelectAny, query.BaselineExample, query.BoundaryExample, query);

            return this.PerformActionWithResult(request, query, this.SelectEntityPageQuery);
        }

        /// <summary>
        /// Selects a single item from the repository.
        /// </summary>
        /// <param name="item">
        /// An item that contains the unique ID or key of the item to retrieve.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A <see cref="TItem"/> that matches the unique properties of <paramref name="item"/>, or null if no match is found.
        /// </returns>
        public TItem SelectItem<TItem>(TItem item)
        {
            ActionRequest request = this.CreateActionRequest(EntityActionType.SelectOne, item);
            return this.PerformActionWithResult(request, item, this.GetFirstOrDefaultAs);
        }

        /// <summary>
        /// Selects items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A collection of items matching the query.
        /// </returns>
        public IEnumerable<TItem> SelectItems<TItem>(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            ActionRequest request = Evaluate.IsNull(query.BaselineExample)
                                        ? this.CreateActionRequest(EntityActionType.SelectAll, query.BaselineExample, query)
                                        : this.CreateActionRequest(
                                            EntityActionType.SelectAny, query.BaselineExample, query.BoundaryExample, query);

            return this.PerformActionWithResult(request, query, this.SelectItemList);
        }

        /// <summary>
        /// Selects a page of items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A page of items matching the query.
        /// </returns>
        public Page<TItem> SelectItemPage<TItem>(IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            ActionRequest request = Evaluate.IsNull(query.BaselineExample)
                                        ? this.CreateActionRequest(EntityActionType.SelectAll, query.BaselineExample, query)
                                        : this.CreateActionRequest(
                                            EntityActionType.SelectAny, query.BaselineExample, query.BoundaryExample, query);

            return this.PerformActionWithResult(request, query, this.SelectItemPageQuery);
        }

        #endregion

        #region Methods

        #region Action Request Creation

        /// <summary>
        /// Begins an action and returns the action event to the caller.
        /// </summary>
        /// <param name="actionType">
        /// The type of action to start.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="properties">
        /// The properties to include in the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of target object.
        /// </typeparam>
        /// <returns>
        /// An <see cref="SAF.ActionTracking.ActionEvent"/> for the specified action.
        /// </returns>
        private ActionRequest CreateActionRequest<TItem>(EntityActionType actionType, TItem targetItem, IPropertyNameSelection properties)
        {
            return this.CreateActionRequest(actionType, targetItem, default(TItem), properties);
        }

        /// <summary>
        /// Begins an action and returns the action event to the caller.
        /// </summary>
        /// <param name="actionType">
        /// The type of action to start.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of target object.
        /// </typeparam>
        /// <returns>
        /// An <see cref="SAF.ActionTracking.ActionEvent"/> for the specified action.
        /// </returns>
        private ActionRequest CreateActionRequest<TItem>(EntityActionType actionType, TItem targetItem)
        {
            return this.CreateActionRequest(actionType, targetItem, default(TItem), new PropertyNameSelection<TItem>());
        }

        /// <summary>
        /// Begins an action and returns the action event to the caller.
        /// </summary>
        /// <param name="actionType">
        /// The type of action to start.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="comparisonItem">
        /// The comparison item.
        /// </param>
        /// <param name="properties">
        /// The properties to include in the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of target object.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="properties"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <see cref="EntityActionType"/> requires a target item, but <paramref name="targetItem"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="SAF.ActionTracking.ActionRequest"/> for the specified action.
        /// </returns>
        private ActionRequest CreateActionRequest<TItem>(
            EntityActionType actionType, TItem targetItem, TItem comparisonItem, IPropertyNameSelection properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            if (Evaluate.IsNull(targetItem) && !this.targetlessActions.Contains(actionType))
            {
                throw new ArgumentException(String.Format(ValidationMessages.TargetRequiredForAction, actionType), "targetItem");
            }

            string[] propertiesToInclude = properties.PropertiesToInclude == null
                                               ? new string[0]
                                               : properties.PropertiesToInclude.ToArray();

            bool isComparison = Evaluate.IsSet(comparisonItem);

            string description = String.Format(actionType.DescriptionFormat, typeof(TItem).Name);
            string additionalData = null;

            switch (actionType.Value)
            {
                case EntityActionType.SaveValue:

                    if (isComparison)
                    {
                        additionalData = String.Format(
                            UpdatedValuesFormat, Evaluate.GetDifferences(targetItem, comparisonItem, propertiesToInclude));
                    }
                    else
                    {
                        additionalData = String.Format(
                            SavedValuesFormat, targetItem.ToPropertyValueString(propertiesToInclude));
                    }

                    break;

                case EntityActionType.SelectOneValue:
                case EntityActionType.RemoveOneValue:
                    additionalData = String.Format(ExampleFormat, targetItem.ToPropertyValueString(propertiesToInclude));
                    break;

                case EntityActionType.SelectAnyValue:
                case EntityActionType.RemoveAnyValue:
                case EntityActionType.UpdateAnyValue:

                    if (isComparison)
                    {
                        additionalData = String.Format(
                            RangeDescriptionFormat, 
                            targetItem.ToPropertyValueString(propertiesToInclude), 
                            comparisonItem.ToPropertyValueString(propertiesToInclude));
                    }
                    else
                    {
                        additionalData = String.Format(ExampleFormat, targetItem.ToPropertyValueString(propertiesToInclude));
                    }

                    break;

                case EntityActionType.SelectAllValue:
                case EntityActionType.RemoveAllValue:
                case EntityActionType.UpdateAllValue:
                    break;

                default:
                    throw new ActionNotSupportedException(String.Format(ValidationMessages.ActionIsNotSupportedForMethod, actionType));
            }

            return ActionRequest.Create(
                actionType.ActionType, EntityOperationContext.Current.CurrentActionSource, targetItem, description, additionalData);
        }

        #endregion

        #region Repository Methods

        /// <summary>
        /// Determines whether the specified item is contained in the repository.
        /// </summary>
        /// <param name="candidate">
        /// The item containing the properties to search for.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties of the entity.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsItem<TItem>(TItem candidate)
        {
            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.Contains(candidate);
            }
        }

        /// <summary>
        /// Gets the first entity matching the specified item, or null if no entity is found.
        /// </summary>
        /// <param name="item">
        /// The item containing the properties to match.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item containing the properties to match.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="TEntity"/>, or the default value if no matches are found.
        /// </returns>
        private TEntity GetFirstOrDefault<TItem>(TItem item)
        {
            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.FirstOrDefault(item);
            }
        }

        /// <summary>
        /// Gets the first entity matching the specified item, or null if no entity is found.
        /// </summary>
        /// <param name="item">
        /// The item containing the properties to match.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item containing the properties to match.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="TEntity"/>, or the default value if no matches are found.
        /// </returns>
        private TItem GetFirstOrDefaultAs<TItem>(TItem item)
        {
            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.FirstOrDefaultAs(item);
            }
        }

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to remove.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        private bool RemoveItemFromRepository<TItem>(TItem item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException("item");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.Delete(item) > 0;
            }
        }

        /// <summary>
        /// Selects an enumerable of <see cref="TItem"/> items mapped from <see cref="TEntity"/> elements in the repository using the
        /// specified baseline example and optional boundary example.
        /// </summary>
        /// <param name="exampleQuery">
        /// The example Query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type item that represents the <see cref="TEntity"/>.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="TItem"/> elements.
        /// </returns>
        private int RemoveSelectedItems<TItem>(IExampleQuery<TItem> exampleQuery)
        {
            if (exampleQuery == null)
            {
                throw new ArgumentNullException("exampleQuery");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.DeleteSelection(exampleQuery);
            }
        }

        /// <summary>
        /// Saves an item, returning the underlying entity to the caller.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to save.
        /// </typeparam>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/>.
        /// </returns>
        /// <exception cref="T:System.ServiceModel.FaultException`1">
        /// If the entity failed to validate, contains an <see cref="EntityValidationFault"/>.
        /// If the entity failed to map properly, contains a <see cref="ApplicationConfigurationFault"/>.
        /// If the action failed, contains a <see cref="EntityRepositoryFault"/>.
        /// </exception>
        private TEntity SaveEntityToRepository<TItem>(TItem item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException("item");
            }

            var validatingEntity = item as IValidatingEntity;

            if (validatingEntity != null)
            {
                validatingEntity.ThrowOnValidationFailure();
            }

            TEntity entity;

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                entity = repository.Save(item);
            }

            return entity;
        }

        /// <summary>
        /// Saves an item, returning the saved item to the caller.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to save.
        /// </typeparam>
        /// <returns>
        /// The saved <typeparamref name="TItem"/>.
        /// </returns>
        /// <exception cref="T:System.ServiceModel.FaultException`1">
        /// If the item failed to validate, contains an <see cref="EntityValidationFault"/>.
        /// If the item failed to map properly, contains a <see cref="ApplicationConfigurationFault"/>.
        /// If the action failed, contains a <see cref="EntityRepositoryFault"/>.
        /// </exception>
        private TItem SaveItemToRepository<TItem>(TItem item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException("item");
            }

            var validatingEntity = item as IValidatingEntity;

            if (validatingEntity != null)
            {
                validatingEntity.ThrowOnValidationFailure();
            }

            var entity = this.entityMapper.Map<TEntity>(item);
            entity.ThrowOnValidationFailure();

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.SaveAs<TItem>(entity);
            }
        }

        /// <summary>
        /// Selects an enumerable of <see cref="TItem"/> items mapped from <see cref="TEntity"/> elements in the repository using the
        /// specified baseline example and optional boundary example.
        /// </summary>
        /// <param name="exampleQuery">
        /// The example query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type item that represents the <see cref="TEntity"/>.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="TItem"/> elements.
        /// </returns>
        private IEnumerable<TEntity> SelectEntityList<TItem>(IExampleQuery<TItem> exampleQuery)
        {
            if (exampleQuery == null)
            {
                throw new ArgumentNullException("exampleQuery");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.SelectEntities(exampleQuery).ToList();
            }
        }

        /// <summary>
        /// Selects a page of items matching the query from the repository.
        /// </summary>
        /// <param name="exampleQuery">
        /// The example query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A page of items matching the query.
        /// </returns>
        private Page<TEntity> SelectEntityPageQuery<TItem>(IExampleQuery<TItem> exampleQuery)
        {
            if (exampleQuery == null)
            {
                throw new ArgumentNullException("exampleQuery");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                // TODO: Ensure usages of this enumerate the list.
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.SelectEntityPage(exampleQuery);
            }
        }

        /// <summary>
        /// Selects an enumerable of <see cref="TItem"/> items mapped from <see cref="TEntity"/> elements in the repository using the
        /// specified baseline example and optional boundary example.
        /// </summary>
        /// <param name="exampleQuery">
        /// The example query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type item that represents the <see cref="TEntity"/>.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="TItem"/> elements.
        /// </returns>
        private IEnumerable<TItem> SelectItemList<TItem>(IExampleQuery<TItem> exampleQuery)
        {
            if (exampleQuery == null)
            {
                throw new ArgumentNullException("exampleQuery");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.SelectAs(exampleQuery).ToList();
            }
        }

        /// <summary>
        /// Selects a page of <see cref="TItem"/> items mapped from <see cref="TEntity"/> elements in the repository using the 
        /// specified baseline example and optional boundary example.
        /// </summary>
        /// <param name="exampleQuery">
        /// The example query.
        /// </param>
        /// <typeparam name="TItem">
        /// The type item that represents the <see cref="TEntity"/>.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="TItem"/> elements.
        /// </returns>
        private Page<TItem> SelectItemPageQuery<TItem>(IExampleQuery<TItem> exampleQuery)
        {
            if (exampleQuery == null)
            {
                throw new ArgumentNullException("exampleQuery");
            }

            using (IRepositoryProvider provider = this.providerFactory.Create())
            {
                // TODO: Ensure usages of this enumerate the list.
                IEntityRepository<TEntity> repository = this.repositoryFactory.Create(provider);
                return repository.SelectPageAs(exampleQuery);
            }
        }

        #endregion

        #endregion
    }
}