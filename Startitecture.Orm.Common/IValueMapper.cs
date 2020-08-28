// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for mapping one value to another.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface for mapping one value to another.
    /// </summary>
    public interface IValueMapper
    {
        /// <summary>
        /// Coverts one value to an equivalent value of a different type.
        /// </summary>
        /// <param name="input">
        /// The input value.
        /// </param>
        /// <returns>
        /// The output value as an <see cref="object"/>.
        /// </returns>
        object Convert(object input);
    }
}