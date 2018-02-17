// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedElementComparer.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The i ordered element comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The i ordered element comparer.
    /// </summary>
    public class OrderedElementComparer : Comparer<IOrderedElement>
    {
        /// <summary>
        /// Performs a comparison of two <see cref="IOrderedElement"/> objects and returns a value indicating whether one object is 
        /// less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as
        /// shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than
        /// <paramref name="y"/>.
        /// </returns>
        public override int Compare(IOrderedElement x, IOrderedElement y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.Order.CompareTo(y.Order);
        }
    }
}