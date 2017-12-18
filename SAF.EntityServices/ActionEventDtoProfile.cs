// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionEventDtoProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains the AutoMapper configuration for mapping action event contracts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using AutoMapper;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.Security;

    /// <summary>
    /// Contains the AutoMapper configuration for mapping action event contracts. 
    /// </summary>
    [CLSCompliant(false)]
    [Obsolete("Create this explicitly.")]
    public class ActionEventDtoProfile : ContractMappingProfile<ActionEventDto, ActionEvent>
    {
        /// <summary>
        /// The account info provider.
        /// </summary>
        private readonly IAccountInfoProvider accountInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventDtoProfile"/> class.
        /// </summary>
        /// <param name="accountInfoProvider">
        /// The account info provider.
        /// </param>
        public ActionEventDtoProfile(IAccountInfoProvider accountInfoProvider)
        {
            this.accountInfoProvider = accountInfoProvider;
        }

        /// <summary>
        /// Gets the entity properties to ignore when creating an entity from a DTO.
        /// </summary>
        /// <value>
        /// A collection of property selectors that indicate properties to ignore when constructing an entity from a DTO.
        /// </value>
        protected override IEnumerable<Expression<Func<ActionEvent, object>>> ContractToEntityPropertiesToIgnore
        {
            get
            {
                return new Expression<Func<ActionEvent, object>>[]
                           {
                               item => item.Request, 
                               item => item.ActionEventId, 
                               item => item.UserAccount, 
                               item => item.ErrorInfo
                           };
            }
        }

        /// <summary>
        /// Constructs an entity from a DTO item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// An entity representing the <paramref name="item"/>.
        /// </returns>
        protected override ActionEvent ConstructFrom(ActionEventDto item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var actionRequest = ActionRequest.Create(
                item.GlobalIdentifier,
                item.ActionName,
                item.ActionSource,
                item.ItemType,
                item.Item,
                item.Description,
                item.AdditionalData);

            var accountInfo = this.accountInfoProvider.GetAccountInfo(item.UserAccount);
            var actionEvent = new ActionEvent(actionRequest, accountInfo, item.InitiationTime);

            if (item.ErrorCode == null)
            {
                actionEvent.CompleteAction(item.CompletionTime);
            }
            else
            {
                var errorInfo = new ErrorInfo(item.ErrorCode, item.ErrorType, item.ErrorMessage, item.ErrorData, item.FullErrorOutput);
                actionEvent.CompleteAction(errorInfo, item.CompletionTime);
            }

            return actionEvent;
        }

        /// <summary>
        /// Creates the entity to DTO mapping.
        /// </summary>
        /// <param name="expression">
        /// The expression to apply the mapping to.
        /// </param>
        protected override void CreateEntityToContractMapping(IMappingExpression<ActionEvent, ActionEventDto> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            expression
                .ForMember(dest => dest.UserAccount, expr => expr.MapFrom(source => source.UserAccount.AccountName))
                .ForMember(dest => dest.UserDisplayName, expr => expr.MapFrom(source => source.UserAccount.FullName))
                .ForMember(dest => dest.ActionName, expr => expr.MapFrom(source => source.Request.ActionName))
                .ForMember(dest => dest.ActionSource, expr => expr.MapFrom(source => source.Request.ActionSource))
                .ForMember(dest => dest.AdditionalData, expr => expr.MapFrom(source => source.Request.AdditionalData))
                .ForMember(dest => dest.Description, expr => expr.MapFrom(source => source.Request.Description))
                .ForMember(dest => dest.GlobalIdentifier, expr => expr.MapFrom(source => source.Request.GlobalIdentifier))
                .ForMember(dest => dest.Item, expr => expr.MapFrom(source => source.Request.Item))
                .ForMember(dest => dest.ItemType, expr => expr.MapFrom(source => source.Request.ItemType))
                .ForMember(dest => dest.ErrorCode, expr => expr.MapFrom(source => source.ErrorInfo.ErrorCode))
                .ForMember(dest => dest.ErrorData, expr => expr.MapFrom(source => source.ErrorInfo.ErrorData))
                .ForMember(dest => dest.ErrorMessage, expr => expr.MapFrom(source => source.ErrorInfo.ErrorMessage))
                .ForMember(dest => dest.ErrorType, expr => expr.MapFrom(source => source.ErrorInfo.ErrorType))
                .ForMember(dest => dest.FullErrorOutput, expr => expr.MapFrom(source => source.ErrorInfo.FullErrorOutput));
        }
    }
}
