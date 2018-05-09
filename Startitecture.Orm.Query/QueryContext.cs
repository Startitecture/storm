// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    /// <summary>
    /// The query context.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to query.
    /// </typeparam>
    public class QueryContext<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext{TItem}"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        public QueryContext(ItemSelection<TItem> selection, StatementOutputType outputType)
            : this(selection, outputType, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext{TItem}"/> class.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        /// <param name="parameterOffset">
        /// The parameter offset.
        /// </param>
        public QueryContext(ItemSelection<TItem> selection, StatementOutputType outputType, int parameterOffset)
        {
            this.Selection = selection;
            this.OutputType = outputType;
            this.ParameterOffset = parameterOffset;
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        public ItemSelection<TItem> Selection { get; }

        /// <summary>
        /// Gets the parameter offset.
        /// </summary>
        public int ParameterOffset { get; }

        /// <summary>
        /// Gets the output type.
        /// </summary>
        public StatementOutputType OutputType { get; }
    }
}