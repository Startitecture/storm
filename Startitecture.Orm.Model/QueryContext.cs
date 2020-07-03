// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// The query context.
    /// </summary>
    public class QueryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        public QueryContext(ISelection selection, IEntityDefinition entityDefinition, StatementOutputType outputType)
            : this(selection, entityDefinition, outputType, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        /// <param name="parameterOffset">
        /// The parameter offset.
        /// </param>
        public QueryContext(ISelection selection, IEntityDefinition entityDefinition, StatementOutputType outputType, int parameterOffset)
        {
            this.Selection = selection;
            this.EntityDefinition = entityDefinition;
            this.OutputType = outputType;
            this.ParameterOffset = parameterOffset;
        }

        /// <summary>
        /// Gets the selection statement for the query.
        /// </summary>
        public ISelection Selection { get; }

        /// <summary>
        /// Gets the entity definition for the query.
        /// </summary>
        public IEntityDefinition EntityDefinition { get; }

        /// <summary>
        /// Gets the parameter offset of the query parameters.
        /// </summary>
        public int ParameterOffset { get; }

        /// <summary>
        /// Gets the output type for the query.
        /// </summary>
        public StatementOutputType OutputType { get; }
    }
}