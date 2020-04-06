﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueDataTypeRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The field value data type row.
    /// </summary>
    [TableType("FieldValueDataType")]
    public class FieldValueDataTypeRow
    {
        /// <summary>
        /// Gets or sets the field value id.
        /// </summary>
        [Column(Order = 1)]
        public long? FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the field id.
        /// </summary>
        [Column(Order = 2)]
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets the last modified by domain identifier id.
        /// </summary>
        [Column(Order = 3)]
        public int LastModifiedByDomainIdentifierId { get; set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        [Column(Order = 4)]
        public DateTimeOffset LastModifiedTime { get; set; }
    }
}