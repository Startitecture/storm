﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterType.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An enumeration of supported SQL filters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// An enumeration of supported SQL filters.
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
        IsNotNull = 9,

        /// <summary>
        /// The filter should match values that are not set.
        /// </summary>
        IsNull = 10,

        /// <summary>
        /// The filter should match the associated sub query.
        /// </summary>
        MatchesSubQuery = 11
    }
}