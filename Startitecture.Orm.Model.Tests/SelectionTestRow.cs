﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionTestRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;

    /// <summary>
    /// The selection test row.
    /// </summary>
    public class SelectionTestRow
    {
        /// <summary>
        /// Gets or sets the selection test id.
        /// </summary>
        public int SelectionTestId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Gets or sets the some decimal.
        /// </summary>
        public decimal SomeDecimal { get; set; }

        /// <summary>
        /// Gets or sets the some date.
        /// </summary>
        public DateTime SomeDate { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public ParentRow Parent { get; set; }
    }
}