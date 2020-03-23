// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionComparer.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Compares two <see cref="IEnumerable&lt;T&gt;" /> collections for equality of elements and sequence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Compares two <see cref="System.Collections.Generic.IEnumerable{T}"/> collections for equality of elements and sequence.
    /// </summary>
    /// <typeparam name="T">The type of item contained in the collections</typeparam>
    /// <remarks>From http://stackoverflow.com/questions/50098/comparing-two-collections-for-equality. The author 
    /// claims he developed it from reflected .NET code (CollectionAssert in testing namespace).</remarks>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules", 
        "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
        Justification = "StackOverflow is the name of the site.")]
    public class CollectionComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// The default equality comparer for the type specified by the generic type argument.
        /// </summary>
        private static readonly CollectionComparer<T> DefaultComparer = new CollectionComparer<T>();

        /// <summary>
        /// Gets a default equality comparer for the type specified by the generic type argument.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "This approach mirrors the one used in EqualityComparer.")]
        public static CollectionComparer<T> Default
        {
            get
            {
                return DefaultComparer;
            }
        }

        /// <summary>
        /// Compares two collections for equality of elements and sequence.
        /// </summary>
        /// <param name="x">The first collection to compare.</param>
        /// <param name="y">The second collection to compare.</param>
        /// <returns>True if the collections contain equal elements in the same order; otherwise, false.</returns>
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if ((x == null) != (y == null))
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            var first = x as IList<T> ?? x.ToList();
            var second = y as IList<T> ?? y.ToList();

            if (first.Count != second.Count)
            {
                return false;
            }

            if (first.Any() && HaveMismatchedElement(first, second))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the hash code of an enumerable collection.
        /// </summary>
        /// <param name="obj">The collection to retrieve a hash code for.</param>
        /// <returns>The hash code of the collection.</returns>
        public int GetHashCode(IEnumerable<T> obj)
        {
            return Evaluate.GenerateHashCode(obj);
        }

        /// <summary>
        /// Determines if the collections contain mismatched elements.
        /// </summary>
        /// <param name="first">The first collection to compare.</param>
        /// <param name="second">The second collection to compare.</param>
        /// <returns>True if the collections contain mismatched elements; otherwise, false.</returns>
        private static bool HaveMismatchedElement(IEnumerable<T> first, IEnumerable<T> second)
        {
            int firstCount;
            int secondCount;

            var firstElementCounts = GetElementCounts(first, out firstCount);
            var secondElementCounts = GetElementCounts(second, out secondCount);

            if (firstCount != secondCount)
            {
                return true;
            }

            foreach (var kvp in firstElementCounts)
            {
                firstCount = kvp.Value;
                secondElementCounts.TryGetValue(kvp.Key, out secondCount);

                if (firstCount != secondCount)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a dictionary of elements of an <see cref="System.Collections.Generic.IEnumerable{T}"/> as keys and their placement in 
        /// the <see cref="System.Collections.Generic.IEnumerable{T}"/> as values.
        /// </summary>
        /// <param name="enumerable">The <see cref="System.Collections.Generic.IEnumerable{T}"/> to retrieve an ordered element dictionary for.</param>
        /// <param name="nullCount">The number of null values within the <see cref="System.Collections.Generic.IEnumerable{T}"/>.</param>
        /// <returns>
        /// A dictionary of elements of an <see cref="System.Collections.Generic.IEnumerable{T}"/> as keys and their placement in the 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> as values.
        /// </returns>
        private static Dictionary<T, int> GetElementCounts(IEnumerable<T> enumerable, out int nullCount)
        {
            var dictionary = new Dictionary<T, int>();
            nullCount = 0;

            foreach (T element in enumerable)
            {
                if (Evaluate.IsNull(element))
                {
                    nullCount++;
                }
                else
                {
                    int num;
                    dictionary.TryGetValue(element, out num);
                    num++;
                    dictionary[element] = num;
                }
            }

            return dictionary;
        }
    }
}
