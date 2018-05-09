namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    public class TransactSqlJoin
    {
        /// <summary>
        /// The aliased relation statement format.
        /// </summary>
        private const string AliasedRelationStatementFormat = "{0} {1} AS [{2}] ON {3} = {4}";

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

        private static readonly TransactSqlQualifier TransactSqlQualifier = Singleton<TransactSqlQualifier>.Instance;

        private readonly IEntityDefinitionProvider definitionProvider;

        public TransactSqlJoin([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
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
        public string Create<TItem>(ItemSelection<TItem> selection)
        {
            return string.Join(Environment.NewLine, selection.Relations.Select(GenerateRelationStatement));
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

            var sourceAttribute =
                this.definitionProvider.Resolve(sourceReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.SourceExpression.GetPropertyName());

            var sourceName = TransactSqlQualifier.Qualify(sourceAttribute);

            var relationReference = this.definitionProvider.GetEntityReference(entityRelation.RelationExpression);
            relationReference.EntityAlias = entityRelation.RelationEntityAlias ?? relationReference.EntityAlias;

            var relationLocation = this.definitionProvider.GetEntityLocation(relationReference);
            var relationAttribute =
                this.definitionProvider.Resolve(relationReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == entityRelation.RelationExpression.GetPropertyName());

            var relationEntity = TransactSqlQualifier.GetCanonicalName(relationAttribute.Entity); 
            var relationName = TransactSqlQualifier.Qualify(relationAttribute); 

            if (string.IsNullOrWhiteSpace(relationLocation.Alias))
            {
                // Use the entity names for the inner join if no alias has been requested.
                return string.Format(RelationStatementFormat, joinType, relationEntity, sourceName, relationName);
            }

            // Use the entity names names for the inner join and alias the table.
            return string.Format(
                AliasedRelationStatementFormat,
                joinType,
                relationEntity,
                relationLocation.Alias,
                sourceName,
                relationName);
        }
    }
}
