// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditConfigurationProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Resolves the entity services configuration from the current configuration file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;
    using System.Configuration;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Resolves the entity services configuration from the current configuration file.
    /// </summary>
    /// <remarks>
    /// The configuration is resolved one time when the instance is created. Changes to the configuration will not take effect until
    /// the application domain is reloaded.
    /// </remarks>
    public class AuditConfigurationProvider : IAuditConfigurationProvider
    {
        /// <summary>
        /// The configuration section.
        /// </summary>
        private readonly Lazy<AuditConfigurationSection> configurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConfigurationProvider"/> class. 
        /// </summary>
        /// <exception cref="ApplicationConfigurationException">
        /// The configuration section does not exist in the application configuration file.
        /// </exception>
        public AuditConfigurationProvider()
        {
            this.configurationSection = new Lazy<AuditConfigurationSection>(LoadConfiguration);
        }

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public AuditConfigurationSection Configuration
        {
            get
            {
                return this.configurationSection.Value;
            }
        }

        /// <summary>
        /// Loads the entity services configuration for the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="AuditConfigurationSection"/> in the current application configuration.
        /// </returns>
        /// <exception cref="ApplicationConfigurationException">
        /// The application configuration did not contain the configuration element.
        /// </exception>
        private static AuditConfigurationSection LoadConfiguration()
        {
            var configuration =
                ConfigurationManager.GetSection(AuditConfigurationSection.ConfigurationPath) as AuditConfigurationSection;

            if (configuration == null)
            {
                string message = string.Format(
                    ErrorMessages.AuditConfigurationNotFound, AuditConfigurationSection.ConfigurationPath);

                throw new ApplicationConfigurationException(message, typeof(AuditConfigurationSection).Name);
            }

            return configuration;
        }
    }
}
