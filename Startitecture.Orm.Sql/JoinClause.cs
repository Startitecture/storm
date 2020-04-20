﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinClause.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a Transact-SQL join.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Represents a Transact-SQL join.
    /// </summary>
    public class JoinClause
    {
        /// <summary>
        /// The aliased relation statement format.
        /// </summary>
        private const string AliasedRelationStatementFormat = "{0} {1} AS {2} ON {3} = {4}";

        /// <summary>
        /// The relation statement format.
        /// </summary>
        private const string RelationStatementFormat = "{0} {1} ON {2} = {3}";

        /// <summary>
        /// The inner join clause.
        /// </summary>
        private const string InnerJoinClause = "INNER JOIN";

        /// <summary>
        /// The left join clause.
        /// </summary>
        private const string LeftJoinClause = "LEFT JOIN";

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The name qualifier.
        /// </summary>
        private readonly INameQualifier nameQualifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinClause"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="nameQualifier">
        /// The SQL name qualifier.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> or <paramref name="nameQualifier"/> is null.
        /// </exception>
        public JoinClause([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] INameQualifier nameQualifier)
        {
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.nameQualifier = nameQualifier ?? throw new ArgumentNullException(nameof(nameQualifier));
        }

        /// <summary>
        /// Creates a JOIN clause for the specified <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection to evaluate.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being selected.
        /// </typeparam>
        /// <returns>
        /// The JOIN clause as a <see cref="string"/>.
        /// </returns>
        public string Create<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return string.Join(Environment.NewLine, selection.Relations.Select(this.GenerateRelationStatement));
        }

        /// <summary>
        /// Gets the JOIN clause for the specified relation type.
        /// </summary>
        /// <param name="relationType">
        /// The relation type.
        /// </param>
        /// <returns>
        /// The JOIN clause as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relationType"/> is not one of the named enumerations.
        /// </exception>
        private static string GetJoinClause(EntityRelationType relationType)
        {
            string joinType;

            switch (relationType)
            {
                case EntityRelationType.InnerJoin:
                    joinType = InnerJoinClause;
                    break;
                case EntityRelationType.LeftJoin:
                    joinType = LeftJoinClause;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationType));
            }

            return joinType;
        }

        /// <summary>
        /// Generates a relation statement.
        /// </summary>
        /// <param name="entityRelation">
        /// The entity relation to generate a statement for.
        /// </param>
        /// <returns>
        /// The relation statement as a <see cref="string"/>.
        /// </returns>
        private string GenerateRelationStatement(IEntityRelation entityRelation)
        {
            var joinType = GetJoinClause(entityRelation.RelationType);

            var sourceReference = this.definitionProvider.GetEntityReference(entityRelation.SourceExpression);
            sourceReference.EntityAlias = entityRelation.SourceEntityAlias ?? sourceReference.EntityAlias;

            var sourceLocation = this.definitionProvider.GetEntityLocation(sourceReference);

            var sourceAttribute =
                this.definitionProvider.Resolve(sourceReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.SourceExpression.GetPropertyName());

            var sourceName = this.nameQualifier.Qualify(sourceAttribute, sourceLocation);

            var relationReference = this.definitionProvider.GetEntityReference(entityRelation.RelationExpression);
            relationReference.EntityAlias = entityRelation.RelationEntityAlias ?? relationReference.EntityAlias;

            var relationLocation = this.definitionProvider.GetEntityLocation(relationReference);
            var relationAttribute =
                this.definitionProvider.Resolve(relationLocation.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.RelationExpression.GetPropertyName());

            var relationEntity = this.nameQualifier.GetCanonicalName(relationAttribute.Entity); 
            var relationName = this.nameQualifier.Qualify(relationAttribute, relationLocation); 

            if (string.IsNullOrWhiteSpace(relationLocation.Alias))
            {
                // Use the entity names for the inner join if no alias has been requested.
                return string.Format(CultureInfo.InvariantCulture, RelationStatementFormat, joinType, relationEntity, sourceName, relationName);
            }

            // Use the entity names names for the inner join and alias the table.
            return string.Format(
                CultureInfo.InvariantCulture, 
                AliasedRelationStatementFormat,
                joinType,
                relationEntity,
                this.nameQualifier.Escape(relationLocation.Alias),
                sourceName,
                relationName);
        }
    }
}