// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Singleton.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides access to a singleton of a specified type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core
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
        public static T Instance => DefaultInstance;
    }
}