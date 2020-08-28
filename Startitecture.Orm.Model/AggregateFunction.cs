// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateFunction.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An enumeration of SQL aggregate functions that can be applied to a column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// An enumeration of SQL aggregate functions that can be applied to a column.
    /// </summary>
    public enum AggregateFunction
    {
        /// <summary>
        /// Does not apply any aggregate function.
        /// </summary>
        None = 0,

        /// <summary>
        /// Gets the count of all returned values.
        /// </summary>
        Count = 1,

        /// <summary>
        /// Gets the sum of all returned values.
        /// </summary>
        Sum = 2,

        /// <summary>
        /// Gets the minimum value of all the returned values.
        /// </summary>
        Min = 3,

        /// <summary>
        /// Gets the maximum value of all the returned values.
        /// </summary>
        Max = 4,

        /// <summary>
        /// Gets the average of all the returned values.
        /// </summary>
        Avg = 5
    }
}