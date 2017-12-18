// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpersonatedIdentity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Allows code to be executed under the security context of a specified user account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    /// <summary>
    /// Allows code to be executed under the security context of a specified user account.
    /// </summary>
    /// <remarks> 
    /// Implements IDispose, so can be used via a using-directive or method calls;
    /// <code>
    ///  var imp = new ImpersonatedIdentity( "myUsername", "myDomainname", "myPassword" );
    ///  imp.UndoImpersonation();
    ///  var imp = new ImpersonatedIdentity();
    ///  imp.Impersonate("myUsername", "myDomainname", "myPassword");
    ///  imp.UndoImpersonation();
    ///
    ///  using ( new ImpersonatedIdentity( "myUsername", "myDomainname", "myPassword" ) )
    ///  {
    ///   ...
    ///  }
    /// </code>
    /// </remarks>
    public sealed class ImpersonatedIdentity : IDisposable
    {
        /// <summary>
        /// The impersonation context.
        /// </summary>
        private WindowsImpersonationContext impersonationContext;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.UndoImpersonation();
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <param name="domainName">
        /// Name of the domain.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <exception cref="Win32Exception">
        /// An error occurred while calling the native method.
        /// </exception>
        public void Impersonate(string userName, string domainName, string password)
        {
            this.Impersonate(userName, domainName, password, LogOnType.LogOn32LogOnInteractive, LogOnProvider.LogOn32ProviderDefault);
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <param name="domainName">
        /// Name of the domain.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="logOnType">
        /// Type of the logon.
        /// </param>
        /// <param name="logOnProvider">
        /// The logon provider. 
        /// </param>
        /// <exception cref="Win32Exception">
        /// An error occurred while calling the native method.
        /// </exception>
        public void Impersonate(string userName, string domainName, string password, LogOnType logOnType, LogOnProvider logOnProvider)
        {
            this.UndoImpersonation();

            IntPtr logonToken = IntPtr.Zero;
            IntPtr logonTokenDuplicate = IntPtr.Zero;

            try
            {
                // revert to the application pool identity, saving the identity of the current requestor
                this.impersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);

                // do logon & impersonate
                var logonUser = NativeMethods.LogonUser(
                    userName,
                    domainName,
                    password,
                    (int)logOnType,
                    (int)logOnProvider,
                    ref logonToken);

                var lastWin32Error = Marshal.GetLastWin32Error();

                if (logonUser != 0)
                {
                    var duplicateToken = NativeMethods.DuplicateToken(
                        logonToken,
                        (int)ImpersonationLevel.SecurityImpersonation,
                        ref logonTokenDuplicate);

                    lastWin32Error = Marshal.GetLastWin32Error();

                    if (duplicateToken != 0)
                    {
                        var wi = new WindowsIdentity(logonTokenDuplicate);
                        wi.Impersonate(); // discard the returned identity context (which is the context of the application pool)
                    }
                    else
                    {
                        throw new Win32Exception(lastWin32Error);
                    }
                }
                else
                {
                    throw new Win32Exception(lastWin32Error);
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(logonToken);
                }

                if (logonTokenDuplicate != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(logonTokenDuplicate);
                }
            }
        }

        /// <summary>
        /// Stops impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            // restore saved requestor identity
            if (this.impersonationContext != null)
            {
                this.impersonationContext.Undo();
            }

            this.impersonationContext = null;
        }
    }
}
