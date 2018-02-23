// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeTestItem.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    using Startitecture.Core;

    /// <summary>
    /// The fake test item.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeTestItem
    {
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
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[int index]
        {
            get
            {
                return this.testList[index];
            }
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is FakeTestItem && this.Equals(obj as FakeTestItem));
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(FakeTestItem obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Evaluate.CollectionEquals(
                new object[]
                    {
                        this.TestString,
                        this.TestInt
                    },
                new object[]
                    {
                        obj.TestString,
                        obj.TestInt
                    });
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.TestString, this.TestInt);
        }
    }
}