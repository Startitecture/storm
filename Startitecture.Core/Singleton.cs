// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Singleton.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides access to a singleton of a specified type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    /// <summary>
    /// Provides access to a singleton of a specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item stored as a singleton.
    /// </typeparam>
    public static class Singleton<T>
        where T : new()
    {
        #region Static Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static readonly T DefaultInstance = new T();

        #endregion

        /// <summary>
        /// Gets the singleton instance for the current type.
        /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Instance => DefaultInstance;
#pragma warning restore CA1000 // Do not declare static members on generic types
    }
}