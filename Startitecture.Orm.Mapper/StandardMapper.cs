// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardMapper.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   StandardMapper is the default implementation of IMapper used by PetaPoco.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Reflection;

    /// <summary>
    /// StandardMapper is the default implementation of IMapper.
    /// </summary>
    public class StandardMapper : IMapper
    {
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
        public Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
        {
            return null;
        }

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
        public Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return null;
        }
    }
}