// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Core;

    using JetBrains.Annotations;

    using Startitecture.Resources;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets an entity reference from the specified member expression.
        /// </summary>
        /// <param name="memberExpression">
        /// The member expression.
        /// </param>
        /// <returns>
        /// An <see cref="EntityReference"/> for the specified expression.
        /// </returns>
        /// <exception cref="Startitecture.Core.OperationException">
        /// <paramref name="memberExpression"/> does not have a declaring type.
        /// </exception>
        public static EntityReference GetEntityReference([NotNull] this LambdaExpression memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var attributeMember = memberExpression.GetMember();

            if (attributeMember == null)
            {
                throw new OperationException(memberExpression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            // Check whether there's a RelatedEntityAttribute that will override the natural type.
            // Do not use .Member.DeclaringType in place of .Expression.Type because this could be the base type of an inherited type.
            var relatedEntityAttribute = attributeMember.Member.GetCustomAttribute<RelatedEntityAttribute>();
            var declaringType = relatedEntityAttribute?.EntityType ?? attributeMember.Expression.Type;

            if (declaringType == null)
            {
                throw new OperationException(memberExpression, ValidationMessages.PropertyMustHaveDeclaringType);
            }

            // This will be null for RelatedEntityAttributes, so we check from the attribute directly (below).
            var entityMember = attributeMember.Expression as MemberExpression;

            // The definition provider will handle identical alias/entity names.
            return new EntityReference { EntityType = declaringType, EntityAlias = relatedEntityAttribute?.EntityAlias ?? entityMember?.Member.Name };
        }
    }
}