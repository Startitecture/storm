// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsertConflictAction.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the InsertConflictAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    /// <summary>
    /// An enumeration of actions to take when an INSERT operation causes a constraint violation.
    /// </summary>
    public enum InsertConflictAction
    {
        /// <summary>
        /// Allows the constraint violation to be raised.
        /// </summary>
        RaiseConstraintViolation = 0,

        /// <summary>
        /// Does nothing for the affected row (skip insert).
        /// </summary>
        DoNothing = 1,

        /// <summary>
        /// Updates the row with the source row.
        /// </summary>
        Update = 2
    }
}