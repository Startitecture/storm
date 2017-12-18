// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMapper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides a way to hook into the PetaPoco database to POCO mapping mechanism to either customize or completely replace it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides a way to hook into the PetaPoco database to POCO mapping mechanism to either customize or completely replace it.
    /// </summary>
    /// <remarks>
    /// To use this functionality, instantiate a class that implements IMapper and then pass it to PetaPoco through the static method
    /// Mappers.Register().
    /// </remarks>
    public interface IMapper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Supply a function to convert a database value to the correct property value.
        /// </summary>
        /// <param name="targetProperty">
        /// The target property.
        /// </param>
        /// <param name="sourceType">
        /// The type of data returned by the database.
        /// </param>
        /// <returns>
        /// A function that can do the conversion, or null for no conversion.
        /// </returns>
        Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType);

        /// <summary>
        /// Supply a function to convert a property value into a database value.
        /// </summary>
        /// <param name="sourceProperty">
        /// The property to be converted.
        /// </param>
        /// <returns>
        /// A function that can do the conversion.
        /// </returns>
        /// <remarks>
        /// This conversion is only used for converting values from POCO's that are being inserted or updated. Conversion is not 
        /// available for parameter values passed directly to queries.
        /// </remarks>
        Func<object, object> GetToDbConverter(PropertyInfo sourceProperty);

        #endregion
    }
}