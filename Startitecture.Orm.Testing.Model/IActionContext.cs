// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The ActionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The ActionContext interface.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Gets the current person.
        /// </summary>
        PM.Person CurrentPerson { get; }
    }
}
