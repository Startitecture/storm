// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::AutoMapper;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for AutoMapper.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Instructs the AutoMapper profile to resolve the specified <paramref name="constructorParam"/> by checking the resolution context for an
        /// already created item matching the <paramref name="keyAttributes"/> of the source's reference property.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="resolutionSource">
        /// The resolution source.
        /// </param>
        /// <param name="resolutionTarget">
        /// The resolution target.
        /// </param>
        /// <param name="constructorParam">
        /// The constructor param.
        /// </param>
        /// <param name="keyAttributes">
        /// The key attributes.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item that is the source of the mapping expression.
        /// </typeparam>
        /// <typeparam name="TReference">
        /// The type of property on the source that will be used as the reference to resolve on the target.
        /// </typeparam>
        /// <typeparam name="TDest">
        /// The type of item that is the destination of the mapping expression.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of property on the destination that will be resolved.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IMappingExpression{TSource,TDest}"/>.
        /// </returns>
        public static IMappingExpression<TSource, TDest> ResolveByKey<TSource, TReference, TDest, TTarget>(
            [NotNull] this IMappingExpression<TSource, TDest> expression,
            [NotNull] Expression<Func<TSource, TReference>> resolutionSource,
            [NotNull] Expression<Func<TDest, TTarget>> resolutionTarget,
            [NotNull] string constructorParam,
            [NotNull] params Expression<Func<TReference, object>>[] keyAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (resolutionSource == null)
            {
                throw new ArgumentNullException(nameof(resolutionSource));
            }

            if (resolutionTarget == null)
            {
                throw new ArgumentNullException(nameof(resolutionTarget));
            }

            if (constructorParam == null)
            {
                throw new ArgumentNullException(nameof(constructorParam));
            }

            if (keyAttributes == null)
            {
                throw new ArgumentNullException(nameof(keyAttributes));
            }

            expression.ForCtorParam(
                constructorParam,
                configurationExpression => configurationExpression.MapFrom(
                    (source, context) => ResolveItem<TReference, TTarget>(context, resolutionSource.Compile().Invoke(source), keyAttributes.ToArray())));

            expression.ForMember(
                resolutionTarget,
                configurationExpression => configurationExpression.MapFrom(
                    (source, dest, target, context) => ResolveItem<TReference, TTarget>(
                        context,
                        resolutionSource.Compile().Invoke(source),
                        keyAttributes.ToArray())));

            return expression;
        }

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keyAttributes">
        /// The key attributes.
        /// </param>
        /// <typeparam name="TReference">
        /// The type of property on the source that will be used as the reference to resolve on the target.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of property on the destination that will be resolved.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TTarget"/> that was resolved for the <paramref name="context"/>.
        /// </returns>
        private static TTarget ResolveItem<TReference, TTarget>(
            ResolutionContext context,
            TReference source,
            IEnumerable<Expression<Func<TReference, object>>> keyAttributes)
        {
            var key = $"{typeof(TReference).FullName}:{string.Join(".", keyAttributes.Select(expression => expression.Compile().Invoke(source)))}";

            if (context.Options.Items.TryGetValue(key, out var existing))
            {
                return (TTarget)existing;
            }

            var newItem = context.Mapper.Map<TTarget>(source);
            context.Options.Items[key] = newItem;
            return newItem;
        }
    }
}