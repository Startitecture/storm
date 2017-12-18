// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogOnType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    /// <summary>
    /// The logon type.
    /// </summary>
    public enum LogOnType
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The logo n 32_ logo n_ interactive.
        /// </summary>
        LogOn32LogOnInteractive = 2, 

        /// <summary>
        /// The logo n 32_ logo n_ network.
        /// </summary>
        LogOn32LogOnNetwork = 3, 

        /// <summary>
        /// The logo n 32_ logo n_ batch.
        /// </summary>
        LogOn32LogOnBatch = 4, 

        /// <summary>
        /// The logo n 32_ logo n_ service.
        /// </summary>
        LogOn32LogOnService = 5, 

        /// <summary>
        /// The logo n 32_ logo n_ unlock.
        /// </summary>
        LogOn32LogOnUnlock = 7, 

        /// <summary>
        /// The logon 32 logon network clear text.
        /// </summary>
        LogOn32LogOnNetworkClearText = 8, // Win2K or higher

        /// <summary>
        /// The logo n 32_ logo n_ ne w_ credentials.
        /// </summary>
        LogOn32LogOnNewCredentials = 9 // Win2K or higher
    }
}