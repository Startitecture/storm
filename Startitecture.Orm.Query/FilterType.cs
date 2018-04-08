// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    /// <summary>
    /// The filter type.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// The filter should match values that are equal to the predicate value.
        /// </summary>
        Equality = 0,

        /// <summary>
        /// The filter should match values that are not equal to the predicate value.
        /// </summary>
        Inequality = 1,

        /// <summary>
        /// The filter should match values less than the predicate value.
        /// </summary>
        LessThan = 2,

        /// <summary>
        /// The filter should match values less than or equal to the predicate value.
        /// </summary>
        LessThanOrEqualTo = 3,

        /// <summary>
        /// The filter should match values greater than the predicate value.
        /// </summary>
        GreaterThan = 4,

        /// <summary>
        /// The filter should match values greater than or equal to the predicate value.
        /// </summary>
        GreaterThanOrEqualTo = 5,

        /// <summary>
        /// The filter should match values between the two predicate values.
        /// </summary>
        Between = 6,

        /// <summary>
        /// The filter should match the set of predicate values.
        /// </summary>
        MatchesSet = 7,

        /// <summary>
        /// The filter should match values outside the set of predicate values.
        /// </summary>
        DoesNotMatchSet = 8,

        /// <summary>
        /// The filter should match values that are set.
        /// </summary>
        IsSet = 9,

        /// <summary>
        /// The filter should match values that are not set.
        /// </summary>
        IsNotSet = 10,
    }
}