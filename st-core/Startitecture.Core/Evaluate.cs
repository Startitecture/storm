// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Evaluate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Startitecture.Resources;

    /// <summary>
    /// Static methods for evaluating values.
    /// </summary>
    public static class Evaluate
    {
        #region Null and Default Values

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <typeparam name="T">
        /// The type of value to check.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the value is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull<T>(T value)
        {
            return !IsSet(value);
        }

        /// <summary>
        /// Determines whether the specified value is set (not equal to null).
        /// </summary>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <typeparam name="T">
        /// The type of value to check.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the value is not null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSet<T>(T value)
        {
            var type = typeof(T);
            return (type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))) || !IsDefaultValue(value);
        }

        /// <summary>
        /// Checks whether a value is in its default state.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value to check.
        /// </typeparam>
        /// <param name="value">
        /// The value to check.
        /// </param>
        /// <returns>
        /// True if the value is in its default state, otherwise false.
        /// </returns>
        public static bool IsDefaultValue<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }

        #endregion

        #region Equality and Comparison

        /// <summary>
        /// Tests the equality of two items, using recursion if non-string enumerable properties are encountered.
        /// </summary>
        /// <param name="itemA">
        /// The base item.
        /// </param>
        /// <param name="itemB">
        /// The comparison item.
        /// </param>
        /// <returns>
        /// <c>true</c> if the items are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool RecursiveEquals(object itemA, object itemB)
        {
            switch (itemA)
            {
                // Don't allow strings to get evaluated as collections.
                case string stringValueA when itemB is string stringValueB && Equals(stringValueA, stringValueB) == false:
                // Validate buffers are the same length.
                // This also ensures that the count does not exceed the length of either buffer.  
                case byte[] bytesA when itemB is byte[] bytesB
                                        && (bytesA.Length == bytesB.Length && NativeMethods.memcmp(bytesA, bytesB, bytesA.Length) == 0) == false:
                    return false;
                case IEnumerable enumerableA when itemB is IEnumerable enumerableB:
                    {
                        if (enumerableA is ICollection collectionA && enumerableB is ICollection collectionB)
                        {
                            if (collectionA.Count != collectionB.Count)
                            {
                                return false;
                            }
                        }

                        var iteratorA = enumerableA.GetEnumerator();
                        var iteratorB = enumerableB.GetEnumerator();

                        while (iteratorA.MoveNext() && iteratorB.MoveNext())
                        {
                            if (RecursiveEquals(iteratorA.Current, iteratorB.Current) == false)
                            {
                                return false;
                            }
                        }

                        // One collection is longer than the other one, but we did not capture this initially because the enumerable objects were not
                        // collections.
                        if (iteratorA.MoveNext() || iteratorB.MoveNext())
                        {
                            return false;
                        }

                        break;
                    }

                default:
                    {
                        if (Equals(itemA, itemB) == false)
                        {
                            return false;
                        }

                        break;
                    }
            }

            return true;
        }

        /// <summary>
        /// Tests the equality of two values.
        /// </summary>
        /// <typeparam name="T">
        /// The type of values to compare.
        /// </typeparam>
        /// <param name="valueA">
        /// The base value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool Equals<T>(T valueA, object valueB)
        {
            if (IsNull(valueA) && IsNull(valueB))
            {
                return true;
            }

            if (ReferenceEquals(valueA, valueB))
            {
                return true;
            }

            if (valueB is T variable)
            {
                return EqualityComparer<T>.Default.Equals(valueA, variable);
            }

            return false;
        }

        /// <summary>
        /// Tests the equality of two values.
        /// </summary>
        /// <typeparam name="T">
        /// The type of values to compare.
        /// </typeparam>
        /// <param name="itemA">
        /// The base item.
        /// </param>
        /// <param name="itemB">
        /// The comparison item.
        /// </param>
        /// <param name="selectors">
        /// The value selectors. If none are specified, the default equality comparer is used.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified properties of the values are equal; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="EqualityComparer{T}"/> when no selectors have been specified. 
        /// Calling this method from within an implementation of <see cref="IEquatable{T}"/> on an object of the same type 
        /// without specifying selectors will result in a recursive loop as <see cref="EqualityComparer{T}"/> 
        /// calls <see cref="System.IEquatable{T}.Equals(T)"/>. The same issue applies if the calling type is not 
        /// <see cref="System.IEquatable{T}.Equals(T)"/>.
        /// </remarks>
        public static bool Equals<T>(T itemA, T itemB, params Func<T, object>[] selectors)
        {
            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            if (IsNull(itemA) && IsNull(itemB))
            {
                return true;
            }

            if (IsNull(itemA) || IsNull(itemB))
            {
                return false;
            }

            return selectors.Any()
                       ? AreEqual(itemA, itemB, selectors)
                       : EqualityComparer<T>.Default.Equals(itemA, itemB);
        }

        /// <summary>
        /// Determines whether two values, one of a specified type and one of an unknown type, are the same type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to compare.
        /// </typeparam>
        /// <param name="valueA">
        /// The first object to compare.
        /// </param>
        /// <param name="valueB">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="valueA"/> and <paramref name="valueB"/>, as shown in the 
        /// following table.
        /// <para>
        /// Value Meaning 
        /// </para>
        /// <para>
        /// Less than zero <paramref name="valueA"/> is less than <paramref name="valueB"/>.
        /// </para>
        /// <para>
        /// Zero <paramref name="valueA"/> equals <paramref name="valueB"/>.
        /// </para>
        /// <para>
        /// Greater than zero <paramref name="valueA"/> is greater than <paramref name="valueB"/>.
        /// </para>
        /// </returns>
        public static int Compare<T>(T valueA, object valueB)
            where T : IComparable<T>
        {
            if (IsNull(valueA) && valueB == null)
            {
                return 0;
            }

            if (valueB == null)
            {
                return -1;
            }

            if (valueB.GetType() != typeof(T))
            {
                throw new ArgumentException(ErrorMessages.ComparisonValuesMustBeSameType, nameof(valueB));
            }

            return IsNull(valueA) ? 1 : valueA.CompareTo((T)valueB);
        }

        /// <summary>
        /// Performs a comparison of two objects of the same type and returns a value indicating whether one object is less than, 
        /// equal to, or greater than the other.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to compare.
        /// </typeparam>
        /// <param name="valueA">
        /// The first object to compare.
        /// </param>
        /// <param name="valueB">
        /// The second object to compare.
        /// </param>
        /// <param name="selectors">
        /// The property selectors for the specified object.
        /// </param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="valueA"/> and <paramref name="valueB"/>, as shown in the 
        /// following table.
        /// <para>
        /// Value Meaning 
        /// </para>
        /// <para>
        /// Less than zero <paramref name="valueA"/> is less than <paramref name="valueB"/>.
        /// </para>
        /// <para>
        /// Zero <paramref name="valueA"/> equals <paramref name="valueB"/>.
        /// </para>
        /// <para>
        /// Greater than zero <paramref name="valueA"/> is greater than <paramref name="valueB"/>.
        /// </para>
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="Comparer{T}"/> when no selectors have been specified. Calling this
        /// method from within an implementation of <see cref="IComparable{T}"/> on an object of the same type without 
        /// specifying selectors will result in a recursive loop as <see cref="Comparer{T}"/> calls 
        /// <see cref="System.IComparable{T}.CompareTo(T)"/>.
        /// </remarks>
        public static int Compare<T>(T valueA, T valueB, params Func<T, object>[] selectors)
        {
            if (IsNull(valueA) && IsNull(valueB))
            {
                return 0;
            }

            if (IsNull(valueA))
            {
                return -1;
            }

            if (IsNull(valueB))
            {
                return 1;
            }

            return selectors.Any()
                       ? selectors.Select(selector => Comparer.Default.Compare(selector(valueA), selector(valueB)))
                                  .FirstOrDefault(result => result != 0)
                       : Comparer<T>.Default.Compare(valueA, valueB);
        }

        /// <summary>
        /// Gets the hash code of the specified generic value, returning 0 for null values.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to retrieve the hash code for.
        /// </typeparam>
        /// <param name="value">
        /// The object to retrieve the hash code for.
        /// </param>
        /// <returns>
        /// The hash code of the object, or 0 if the object is null.
        /// </returns>
        public static int GetHashCode<T>(T value)
        {
            return IsDefaultValue(value) ? 0 : value.GetHashCode();
        }

        /// <summary>
        /// Gets the hash code for the specified values.
        /// </summary>
        /// <param name="values">
        /// The values to retrieve a hash code for.
        /// </param>
        /// <returns>
        /// An integer hash code based on the specified values.
        /// </returns>
        public static int GenerateHashCode(params object[] values)
        {
            return values?.Aggregate(0, AggregateHash) ?? 0;
        }

        /// <summary>
        /// Gets the hash code for the specified collection. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of value contained in the collection.
        /// </typeparam>
        /// <param name="values">
        /// The values to retrieve a hash code for.
        /// </param>
        /// <returns>
        /// A hash code based on ORing all the hash codes for the specified values.
        /// </returns>
        public static int GenerateHashCode<T>(IEnumerable<T> values)
        {
            unchecked
            {
                return values?.Aggregate(0, AggregateHash) ?? 0;
            }
        }

        /// <summary>
        /// Gets the hash code for the specified values.
        /// </summary>
        /// <param name="item">
        /// The item to retrieve a hash code for.
        /// </param>
        /// <param name="selectors">
        /// The selectors used to retrieve values from the item.
        /// </param>
        /// <typeparam name="T">
        /// The type of item to generate a hash code for.
        /// </typeparam>
        /// <returns>
        /// An integer hash code based on the values selected for the item.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="selectors"/> is null.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Allows passing of Func<T, object> collections which are not difficult to create.")]
        public static int GenerateHashCode<T>(T item, IEnumerable<Func<T, object>> selectors)
        {
            if (IsNull(item))
            {
                return 0;
            }

            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            var items = new List<object>();

            // Avoid the LINQ statement here unless you want to debug this A LOT.

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var selector in selectors)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                var value = selector(item);

                switch (value)
                {
                    // Don't allow strings to fall through to IEnumerable
                    case string _:
                        items.Add(value);
                        break;
                    case IEnumerable collection:
                        {
                            var iterator = collection.GetEnumerator();

                            while (iterator.MoveNext())
                            {
                                items.Add(iterator.Current);
                            }

                            break;
                        }

                    default:
                        items.Add(value);
                        break;
                }
            }

            return GenerateHashCode(items);
        }

        #endregion

        /// <summary>
        /// Determine whether the list of selectors return equal values for <paramref name="itemA"/> and <paramref name="itemB"/>. If the
        /// selection returns an <see cref="IEnumerable"/>, the enumerable is iterated item by item in order.
        /// </summary>
        /// <param name="itemA">
        /// The first item.
        /// </param>
        /// <param name="itemB">
        /// The second item.
        /// </param>
        /// <param name="selectors">
        /// The selectors to evaluate.
        /// </param>
        /// <typeparam name="T">
        /// The type of item being evaluated.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if all the values in the item are equal; otherwise, <c>false</c>.
        /// </returns>
        private static bool AreEqual<T>(T itemA, T itemB, IEnumerable<Func<T, object>> selectors)
        {
            return !(from selector in selectors
                     let valueA = selector(itemA)
                     let valueB = selector(itemB)
                     where RecursiveEquals(valueA, valueB) == false
                     select valueA).Any();
        }

        /// <summary>
        /// Aggregates a hash code.
        /// </summary>
        /// <typeparam name="T">
        /// The type of item to hash.
        /// </typeparam>
        /// <param name="current">
        /// The current hash code.
        /// </param>
        /// <param name="value">
        /// The value to hash.
        /// </param>
        /// <returns>
        /// The resulting hash code as an <see cref="int"/>.
        /// </returns>
        private static int AggregateHash<T>(int current, T value)
        {
            return (current * 397) + GetHashCode(value);
        }
    }
}
