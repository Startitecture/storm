// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
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
