// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.ComponentModel;

    using Startitecture.Orm.Common;

    /// <summary>
    /// A date time mapper for PostgreSQL returning DateTimeOffset because lower resolution datetime types are not supported.
    /// </summary>
    internal class DateTimeMapper : IValueMapper
    {
        /// <inheritdoc />
        public object Convert(object input)
        {
            if (input is DateTime dateTime)
            {
                return new DateTimeOffset(dateTime);
            }

            var converter = new DateTimeOffsetConverter();
            return converter.ConvertFrom(input);
        }
    }
}