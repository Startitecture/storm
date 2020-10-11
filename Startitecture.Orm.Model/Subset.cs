// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subset.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// Defines a subset of a result set.
    /// </summary>
    public class Subset
    {
        /// <summary>
        /// Gets the page specification for the filter set.
        /// </summary>
        public ResultPage Page { get; } = new ResultPage();

        /// <summary>
        /// Skips the specified number of rows in the result set.
        /// </summary>
        /// <param name="rows">
        /// The rows to skip.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public Subset Skip(int rows)
        {
            this.Page.RowOffset = rows < 0 ? 0 : rows;
            return this;
        }

        /// <summary>
        /// Limits the number of rows returned to the number specified.
        /// </summary>
        /// <param name="rows">
        /// The number of rows to take.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public Subset Take(int rows)
        {
            this.Page.Size = rows < 0 ? 0 : rows;
            return this;
        }
    }
}