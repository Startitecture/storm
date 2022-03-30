// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinClause.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a Transact-SQL join.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

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
        /// Gets or sets the indent for the statement output.
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Gets or sets the alias suffix to append to an alias. If set, an alias will be used for every relation.
        /// </summary>
        public string AliasSuffix { get; set; }

        /// <summary>
        /// Creates a JOIN clause for the specified <paramref name="relations"/>.
        /// </summary>
        /// <param name="relations">
        /// The relations to create the JOIN clause for.
        /// </param>
        /// <returns>
        /// The JOIN clause as a <see cref="string"/>.
        /// </returns>
        public string Create([NotNull] IEnumerable<IEntityRelation> relations)
        {
            if (relations == null)
            {
                throw new ArgumentNullException(nameof(relations));
            }

            var indent = this.Indent > 0 ? new string(' ', this.Indent) : string.Empty;
            return string.Join(Environment.NewLine, relations.Select(relation => $"{indent}{this.GenerateRelationStatement(relation)}"));
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
            var useAliasSuffix = string.IsNullOrWhiteSpace(this.AliasSuffix) == false;

            var sourceReference = this.definitionProvider.GetEntityReference(entityRelation.SourceExpression);
            var sourceDefinition = this.definitionProvider.Resolve(sourceReference.EntityType);
            var sourceAlias = entityRelation.SourceEntityAlias ?? sourceReference.EntityAlias;
            sourceReference.EntityAlias = useAliasSuffix ? $"{sourceAlias ?? sourceDefinition.EntityName}{this.AliasSuffix}" : sourceAlias;

            var sourceLocation = this.definitionProvider.GetEntityLocation(sourceReference);

            var sourceAttribute =
                this.definitionProvider.Resolve(sourceReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.SourceExpression.GetPropertyName());

            var sourceName = this.nameQualifier.Qualify(sourceAttribute, sourceLocation);

            var relationReference = this.definitionProvider.GetEntityReference(entityRelation.RelationExpression);
            var relationDefinition = this.definitionProvider.Resolve(relationReference.EntityType);
            var relationAlias = entityRelation.RelationEntityAlias ?? relationReference.EntityAlias;
            relationReference.EntityAlias = useAliasSuffix ? $"{relationAlias ?? relationDefinition.EntityName}{this.AliasSuffix}" : relationAlias;

            var relationLocation = this.definitionProvider.GetEntityLocation(relationReference);
            var relationAttribute =
                this.definitionProvider.Resolve(relationLocation.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.RelationExpression.GetPropertyName());

            var relationEntity = this.nameQualifier.GetPhysicalName(relationAttribute.Entity);
            var relationName = this.nameQualifier.Qualify(relationAttribute, relationLocation);

            if (string.IsNullOrWhiteSpace(relationLocation.Alias) && useAliasSuffix == false)
            {
                // Use the entity names for the inner join if no alias has been requested.
                return string.Format(
                    CultureInfo.InvariantCulture,
                    RelationStatementFormat,
                    joinType,
                    relationEntity,
                    sourceName,
                    relationName);
            }

            // Use the entity names names for the inner join and alias the table.
            return string.Format(
                CultureInfo.InvariantCulture,
                AliasedRelationStatementFormat,
                joinType,
                relationEntity,
                this.nameQualifier.Escape(relationLocation.Alias), // TODO: could be null, need to verify
                sourceName,
                relationName);
        }
    }
}
