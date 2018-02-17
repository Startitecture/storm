// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The ActionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The ActionContext interface.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Gets the current person.
        /// </summary>
        Person CurrentPerson { get; }
    }
}
