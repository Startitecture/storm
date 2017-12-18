// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The win 32 native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The logon user.
        /// </summary>
        /// <param name="lpszUserName">
        /// The lpsz user name.
        /// </param>
        /// <param name="lpszDomain">
        /// The lpsz domain.
        /// </param>
        /// <param name="lpszPassword">
        /// The lpsz password.
        /// </param>
        /// <param name="dwLogonType">
        /// The dw logon type.
        /// </param>
        /// <param name="dwLogonProvider">
        /// The dw logon provider.
        /// </param>
        /// <param name="phToken">
        /// The ph token.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)] // Note: Set to Unicode to satisfy code analysis. Untested.
        public static extern int LogonUser(
            string lpszUserName, 
            string lpszDomain, 
            string lpszPassword, 
            int dwLogonType, 
            int dwLogonProvider, 
            ref IntPtr phToken);

        /// <summary>
        /// The duplicate token.
        /// </summary>
        /// <param name="hToken">
        /// The h token.
        /// </param>
        /// <param name="impersonationLevel">
        /// The impersonation level.
        /// </param>
        /// <param name="hNewToken">
        /// The h new token.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        /// <summary>
        /// The revert to self.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        /// <summary>
        /// The close handle.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}