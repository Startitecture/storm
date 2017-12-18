// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolicyResult.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Specifies the state of a policy action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Persistence
{
    /// <summary>
    /// Specifies the state of a policy action.
    /// </summary>
    public enum PolicyResult
    {
        /// <summary>
        /// The policy was applied successfully.
        /// </summary>
        PolicySuccess = 0,

        /// <summary>
        /// The policy did not apply.
        /// </summary>
        PolicyDoesNotApply = 1,

        /// <summary>
        /// The policy failed to apply.
        /// </summary>
        PolicyFailure = 2
    }
}
