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
    using System.Diagnostics;
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
        [DebuggerHidden]
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
        [DebuggerHidden]
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
        [DebuggerHidden]
        public static bool IsDefaultValue<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Determines whether two byte arrays are equal.
        /// </summary>
        /// <param name="reference">
        /// The byte array to reference.
        /// </param>
        /// <param name="comparison">
        /// The byte array to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if each byte in the reference array is equivalent to the byte at the same index in the comparison array;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool Equals(byte[] reference, byte[] comparison)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (comparison == null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return reference.Length == comparison.Length && NativeMethods.memcmp(reference, comparison, reference.Length) == 0;
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
        [DebuggerHidden]
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
        /// <param name="valueA">
        /// The base value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <param name="selectors">
        /// The value selectors. If none are specified, the default equality comparer is used.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified properties of the values are equal; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="T:System.Collections.Generic.EqualityComparer`1"/> when no selectors have been specified. 
        /// Calling this method from within an implementation of <see cref="T:System.IEquatable`1"/> on an object of the same type 
        /// without specifying selectors will result in a recursive loop as <see cref="T:System.Collections.Generic.EqualityComparer`1"/> 
        /// calls <see cref="System.IEquatable{T}.Equals(T)"/>. The same issue applies if the calling type is not 
        /// <see cref="System.IEquatable{T}.Equals(T)"/>.
        /// </remarks>
        [DebuggerHidden]
        public static bool Equals<T>(T valueA, T valueB, params Func<T, object>[] selectors)
        {
            if (IsNull(valueA) && IsNull(valueB))
            {
                return true;
            }

            if (IsNull(valueA) || IsNull(valueB))
            {
                return false;
            }

            // TODO: Compare collections and byte arrays specifically
            return selectors.Any()
                       ? selectors.All(selector => Equals(selector(valueA), selector(valueB)))
                       : EqualityComparer<T>.Default.Equals(valueA, valueB);
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
        /// This method uses <see cref="T:System.Collections.Generic.Comparer`1"/> when no selectors have been specified. Calling this
        /// method from within an implementation of <see cref="T:System.IComparable`1"/> on an object of the same type without 
        /// specifying selectors will result in a recursive loop as <see cref="T:System.Collections.Generic.Comparer`1"/> calls 
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
        /// Tests the equality of two collections.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the objects to compare.
        /// </typeparam>
        /// <param name="collectionA">
        /// The base collection.
        /// </param>
        /// <param name="collectionB">
        /// The comparison collection.
        /// </param>
        /// <returns>
        /// True if the collections contain equal elements (ordinal); otherwise, false.
        /// </returns>
        [DebuggerHidden]
        public static bool CollectionEquals<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB)
        {
            return CollectionComparer<T>.Default.Equals(collectionA, collectionB);
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
        [DebuggerHidden]
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
        [DebuggerHidden]
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
        [DebuggerHidden]
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
        [DebuggerHidden]
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
                items.Add(selector(item));
            }

            return GenerateHashCode(items);
        }

        #endregion

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
        [DebuggerHidden]
        private static int AggregateHash<T>(int current, T value)
        {
            return (current * 397) + GetHashCode(value);
        }
    }
}
