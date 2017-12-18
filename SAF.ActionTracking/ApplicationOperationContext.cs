// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationOperationContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Web;

    using SAF.Core;
    using SAF.Security;

    /// <summary>
    /// Represents a set of properties that are related to a specific session of program activity.
    /// </summary>
    public class ApplicationOperationContext : IActionContext
    {
        /// <summary>
        /// The could not retrieve caller method message.
        /// </summary>
        private const string CouldNotRetrieveCallerMethodMessage = "{Could not retrieve caller method.}";

        /// <summary>
        /// The identity format.
        /// </summary>
        private const string IdentityFormat = @"{0}\{1}";

        /// <summary>
        /// The default types to skip.
        /// </summary>
        private static readonly Type[] DefaultTypesToSkip =
            {
                typeof(ActionRequest), 
                typeof(ExtensionMethods), 
                typeof(ExecutionPolicy), 
                typeof(ActionEvent), 
                typeof(ApplicationOperationContext), 
                typeof(ActionEventProxy)
            };

        /// <summary>
        /// The current context.
        /// </summary>
        private static readonly ApplicationOperationContext CurrentContext = new ApplicationOperationContext();

        /// <summary>
        /// The identity.
        /// </summary>
        private readonly Lazy<IIdentity> identity = new Lazy<IIdentity>(GetCurrentIdentity);

        /// <summary>
        /// The account info.
        /// </summary>
        private readonly Lazy<IAccountInfo> accountInfo;

        /// <summary>
        /// The action context types to skip.
        /// </summary>
        private readonly List<Type> actionContextTypesToSkip = new List<Type>(DefaultTypesToSkip);

        /// <summary>
        /// The current process name.
        /// </summary>
        private readonly Lazy<string> currentProcessName = new Lazy<string>(() => AppDomain.CurrentDomain.FriendlyName);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationOperationContext"/> class. 
        /// </summary>
        /// <param name="accountInfoProvider">
        /// The account Info Provider.
        /// </param>
        /// <param name="typesToIgnore">
        /// The types to skip when determining the action source.
        /// </param>
        public ApplicationOperationContext(IAccountInfoProvider accountInfoProvider, params Type[] typesToIgnore)
        {
            this.accountInfo = new Lazy<IAccountInfo>(() => accountInfoProvider.GetAccountInfo(this.CurrentIdentity));
            this.actionContextTypesToSkip.AddRange(typesToIgnore);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ApplicationOperationContext"/> class from being created. 
        /// </summary>
        private ApplicationOperationContext()
            : this(new WindowsAccountInfoProvider())
        {
        }

        /// <summary>
        /// Gets the current application security context.
        /// </summary>
        public static ApplicationOperationContext Current
        {
            get
            {
                return CurrentContext;
            }
        }

        /// <summary>
        /// Gets the current action.
        /// </summary>
        public string CurrentAction
        {
            get
            {
                StackFrame frame = GetCallerFrame(this.actionContextTypesToSkip);

                if (frame == null)
                {
                    return CouldNotRetrieveCallerMethodMessage;
                }

                MethodBase method = frame.GetMethod();

                if (method != null)
                {
                    return method.Name;
                }

                return CouldNotRetrieveCallerMethodMessage;
            }
        }

        /// <summary>
        /// Gets the current action source.
        /// </summary>
        public string CurrentActionSource
        {
            get
            {
                StackFrame frame = GetCallerFrame(this.actionContextTypesToSkip);

                if (frame == null)
                {
                    return CouldNotRetrieveCallerMethodMessage;
                }

                MethodBase method = frame.GetMethod();

                if (method != null && method.DeclaringType != null)
                {
                    return method.DeclaringType.ToRuntimeName();
                }

                return CouldNotRetrieveCallerMethodMessage;
            }
        }

        /// <summary>
        /// Gets the endpoint at which the action is executing.
        /// </summary>
        public string Endpoint
        {
            get
            {
                if (OperationContext.Current != null)
                {
                    if (OperationContext.Current.Channel != null && OperationContext.Current.Channel.LocalAddress != null
                        && OperationContext.Current.Channel.LocalAddress.Uri != null)
                    {
                        return Convert.ToString(OperationContext.Current.Channel.LocalAddress.Uri);
                    }

                    if (OperationContext.Current.EndpointDispatcher != null
                        && OperationContext.Current.EndpointDispatcher.EndpointAddress != null
                        && OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri != null)
                    {
                        return Convert.ToString(OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri);
                    }
                }

                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.RawUrl;
                }

                return this.currentProcessName.Value;
            }
        }

        /// <summary>
        /// Gets the host on which the action is executing.
        /// </summary>
        public string Host
        {
            get
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Gets the identity for the current context.
        /// </summary>
        public IIdentity CurrentIdentity
        {
            get
            {
                return this.identity.Value;
            }
        }

        /// <summary>
        /// Gets the current account.
        /// </summary>
        public IAccountInfo CurrentAccount
        {
            get
            {
                return this.accountInfo.Value;
            }
        }

        /// <summary>
        /// Gets the top-most stack frame, excluding frames from the specified classes.
        /// </summary>
        /// <param name="typesToSkip">
        /// The types to skip as declaring method types.
        /// </param>
        /// <returns>
        /// The caller method as a <see cref="string"/>.
        /// </returns>
        private static StackFrame GetCallerFrame(IEnumerable<Type> typesToSkip)
        {
            var skips = DefaultTypesToSkip.Union(typesToSkip);
            var trace = new StackTrace();

            StackFrame[] frames = trace.GetFrames();

            return frames != null ? frames.SkipWhile(x => skips.Contains(x.GetMethod().DeclaringType)).FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the identity of the current user context.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Security.Principal.IIdentity"/> of the current user context.
        /// </returns>
        private static IIdentity GetCurrentIdentity()
        {
            var fallbackIdentity = new GenericIdentity(String.Format(IdentityFormat, Environment.UserDomainName, Environment.UserName));

            if (ServiceSecurityContext.Current != null)
            {
                var currentIdentity = ServiceSecurityContext.Current.PrimaryIdentity;

                if (currentIdentity != null && !String.IsNullOrEmpty(currentIdentity.Name))
                {
                    return currentIdentity;
                }

                currentIdentity = ServiceSecurityContext.Current.WindowsIdentity;

                if (!String.IsNullOrEmpty(currentIdentity.Name))
                {
                    return currentIdentity;
                }
            }

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.User != null)
                {
                    var identity = HttpContext.Current.User.Identity;

                    if (!String.IsNullOrEmpty(identity.Name))
                    {
                        return identity;
                    }
                }
            }

            var windowsIdentity = WindowsIdentity.GetCurrent();

            if (string.IsNullOrEmpty(windowsIdentity.Name) == false)
            {
                return windowsIdentity;
            }

            return fallbackIdentity;
        }
    }
}
