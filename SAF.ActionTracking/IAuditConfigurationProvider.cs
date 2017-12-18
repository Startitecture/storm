// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuditConfigurationProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to configuration providers that return an EntityServicesConfigurationSection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    /// <summary>
    /// Provides an interface to configuration providers that return an <see cref="AuditConfigurationSection"/>.
    /// </summary>
    public interface IAuditConfigurationProvider
    {
        /// <summary>
        /// Gets the current entity services configuration.
        /// </summary>
        AuditConfigurationSection Configuration { get; }
    }
}
