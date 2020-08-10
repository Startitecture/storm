// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface for mapping one value to another.
    /// </summary>
    public interface IValueMapper
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Convert(object input);
    }
}