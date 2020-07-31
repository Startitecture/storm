// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// The query context.
    /// </summary>
    public class StatementContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatementContext"/> class.
        /// </summary>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        public StatementContext(StatementOutputType outputType)
        {
            this.OutputType = outputType;
        }

        /// <summary>
        /// Gets the output type for the query.
        /// </summary>
        public StatementOutputType OutputType { get; }
    }
}