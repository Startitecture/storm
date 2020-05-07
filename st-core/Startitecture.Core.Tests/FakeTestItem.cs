// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeTestItem.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The fake test item.
    /// </summary>
    public class FakeTestItem : IEquatable<FakeTestItem>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeTestItem, object>[] ComparisonProperties =
            {
                item => item.TestInt,
                item => item.TestString,
                item => item.TestDateTime,
                item => item.testList
            };

        /// <summary>
        /// The test list.
        /// </summary>
        private readonly List<string> testList = new List<string>();

        /// <summary>
        /// Gets or sets the test string.
        /// </summary>
        public string TestString { get; set; }

        /// <summary>
        /// Gets or sets the test integer.
        /// </summary>
        [XmlElement]
        public int TestInt { get; set; }

        /// <summary>
        /// Gets or sets the test date time.
        /// </summary>
        [XmlElement]
        public DateTime TestDateTime { get; set; }

        /// <summary>
        /// The test list.
        /// </summary>
        public IEnumerable<string> TestList => this.testList;

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
        public static bool operator ==(FakeTestItem valueA, FakeTestItem valueB)
        {
            return EqualityComparer<FakeTestItem>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeTestItem valueA, FakeTestItem valueB)
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
            return $"{this.TestInt}:{this.TestString}:{this.TestDateTime}:{string.Join(";", this.testList)}";
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
        public bool Equals(FakeTestItem other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public void AddItem(string item)
        {
            this.testList.Add(item);
        }
    }
}