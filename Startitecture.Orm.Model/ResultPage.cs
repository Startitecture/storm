// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultPage.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Defines a subset of results to return within a result set.
    /// </summary>
    public class ResultPage : IEquatable<ResultPage>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ResultPage, object>[] ComparisonProperties =
            {
                item => item.RowOffset,
                item => item.Size
            };

        /// <summary>
        /// Gets or sets the number of rows to offset the beginning of the page.
        /// </summary>
        public int RowOffset { get; set; }

        /// <summary>
        /// Gets or sets the size of the result set to return..
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Sets the page number for the current page result.
        /// </summary>
        /// <param name="pageNumber">
        /// The page number to set.
        /// </param>
        public void SetPage(int pageNumber)
        {
            // Pages are 1-based, so 1 - 1 = 0 or zero offset for the first page.
            this.RowOffset = this.Size * (pageNumber - 1);
        }

        #region Equality and Comparison Methods and Operators

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ResultPage valueA, ResultPage valueB)
        {
            return EqualityComparer<ResultPage>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ResultPage valueA, ResultPage valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{this.RowOffset}->{this.Size}";
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(ResultPage other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}