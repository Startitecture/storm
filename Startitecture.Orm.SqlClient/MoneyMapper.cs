// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoneyMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System.Data.SqlTypes;

    using Startitecture.Orm.Common;

    /// <summary>
    /// Converts common BCL types into the <see cref="SqlMoney"/> type.
    /// </summary>
    internal class MoneyMapper : IValueMapper
    {
        /// <inheritdoc />
        public object Convert(object input)
        {
            return input switch
            {
                decimal decimalInput => new SqlMoney(decimalInput),
                double doubleInput => new SqlMoney(doubleInput),
                int integerInput => new SqlMoney(integerInput),
                long longInput => new SqlMoney(longInput),
                _ => null
            };
        }
    }
}