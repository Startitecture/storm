// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfiguredEventRepositoryFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Creates <see cref="IEventRepository"/> instances based on the current application configuration.
    /// </summary>
    public class ConfiguredEventRepositoryFactory : IEventRepositoryFactory
    {
        /// <summary>
        /// The default factory.
        /// </summary>
        private static readonly ConfiguredEventRepositoryFactory DefaultFactory = new ConfiguredEventRepositoryFactory();

        /// <summary>
        /// The dependencies.
        /// </summary>
        private readonly object[] dependencies;

        /// <summary>
        /// Indicates whether the event repository is configured in the application configuration.
        /// </summary>
        private bool isConfigured;

        /// <summary>
        /// The repository type.
        /// </summary>
        private Type repositoryType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguredEventRepositoryFactory"/> class.
        /// </summary>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        public ConfiguredEventRepositoryFactory(params object[] dependencies)
        {
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfiguredEventRepositoryFactory"/> class from being created.
        /// </summary>
        private ConfiguredEventRepositoryFactory()
            : this(new object[0])
        {
        }

        /// <summary>
        /// Gets the default configured event repository factory.
        /// </summary>
        public static ConfiguredEventRepositoryFactory Default
        {
            get
            {
                return DefaultFactory;
            }
        }

        /// <summary>
        /// Resolves the event repository for the current application.
        /// TODO: Unit Tests
        /// </summary>
        /// <returns>
        /// The <see cref="IEventRepository"/> for the current application.
        /// </returns>
        public IEventRepository Create()
        {
            IEventRepository repository;

            if (this.isConfigured)
            {
                // The type has already been resolved.
                repository = this.CreateRepository(this.repositoryType);
            }
            else
            {
                // We do not know where the repository is configured. First try to get the configuration.
                var configurationProvider = new AuditConfigurationProvider();
                var configurationSection = configurationProvider.Configuration;

                if (configurationSection == null)
                {
                    throw new ApplicationConfigurationException(
                        ErrorMessages.EventRepositoryNotConfigured, typeof(IEventRepositoryFactory).Name);
                }

                this.repositoryType = GetEventRepositoryType(configurationSection);
                repository = this.CreateRepository(this.repositoryType);
                repository.Connection = configurationSection.EventRepositoryConnection;
                this.isConfigured = true;
            }

            return repository;
        }

        /// <summary>
        /// Gets the event repository for the current context using the specified type and configuration.
        /// </summary>
        /// <param name="serviceConfiguration">
        /// The service configuration.
        /// </param>
        /// <returns>
        /// An <see cref="IEventRepository"/> based on the specified type and configuration.
        /// </returns>
        private static Type GetEventRepositoryType(AuditConfigurationSection serviceConfiguration)
        {
            if (serviceConfiguration == null)
            {
                throw new ArgumentNullException("serviceConfiguration");
            }

            var type = Type.GetType(serviceConfiguration.EventRepositoryType);

            if (type == null)
            {
                throw new ApplicationConfigurationException(
                    String.Format(ErrorMessages.EventRepositoryTypeNotFound, serviceConfiguration.EventRepositoryType));
            }

            return type;
        }

        /// <summary>
        /// Resolves a repository from the configuration file.
        /// </summary>
        /// <param name="handlerType">
        /// The handler type.
        /// </param>
        /// <returns>
        /// The <see cref="IEventRepository"/>.
        /// </returns>
        private IEventRepository CreateRepository(Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException("handlerType");
            }

            // We throw an exception here because otherwise it would be difficult to track down the mismatched type.
            if (!typeof(IEventRepository).IsAssignableFrom(handlerType))
            {
                throw new ApplicationConfigurationException(
                    String.Format(ValidationMessages.TypeIsNotInstanceOfExpectedType, handlerType.Name, typeof(IEventRepository).Name), 
                    typeof(AuditConfigurationSection).Name);
            }

            try
            {
                return (IEventRepository)Activator.CreateInstance(handlerType, this.dependencies);
            }
            catch (ArgumentException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (NotSupportedException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (TargetInvocationException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (MemberAccessException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (InvalidComObjectException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (COMException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
            catch (TypeLoadException ex)
            {
                throw new ApplicationConfigurationException(ex.Message, ex, typeof(AuditConfigurationSection).Name);
            }
        }
    }
}
