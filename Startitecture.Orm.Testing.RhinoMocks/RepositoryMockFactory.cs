// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryMockFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.RhinoMocks
{
    using System;

    using AutoMapper;

    using Common;

    using JetBrains.Annotations;

    using Rhino.Mocks;

    using Sql;

    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Repository;

    /// <summary>
    /// Exposes methods to create RhinoMocks mock objects related to repositories.
    /// </summary>
    public static class RepositoryMockFactory
    {
        /// <summary>
        /// Creates an entity mapper with the specified configuration.
        /// </summary>
        /// <param name="configurationAction">
        /// The configuration action to perform.
        /// </param>
        /// <returns>
        /// An <see cref="Startitecture.Orm.Common.IEntityMapper"/> with the specified configuration.
        /// </returns>
        public static IEntityMapper CreateEntityMapper(Action<IMapperConfigurationExpression> configurationAction)
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(configurationAction);
            return entityMapper;
        }

        /// <summary>
        /// Creates a mock repository adapter.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper to use in the repository provider.
        /// </param>
        /// <returns>
        /// A mocked <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with entity mapper and dependency container set.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityMapper"/> is null.
        /// </exception>
        public static IRepositoryProvider CreateProvider([NotNull] IEntityMapper entityMapper)
        {
            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            var repositoryProvider = MockRepository.GenerateMock<IRepositoryProvider>();
            repositoryProvider.Stub(provider => provider.EntityMapper).Return(entityMapper);
            var dependencyContainer = new DependencyContainer();
            repositoryProvider.Stub(provider => provider.DependencyContainer).Return(dependencyContainer);
            return repositoryProvider;
        }

        /// <summary>
        /// The create concrete provider.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of data context that the provider will access.
        /// </typeparam>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="adapter">
        /// The repository adapter.
        /// </param>
        /// <returns>
        /// A concrete <see cref="Startitecture.Orm.Common.IRepositoryProvider"/> with the specified adapter factory.
        /// </returns>
        public static IRepositoryProvider CreateConcreteProvider<TContext>(
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IRepositoryAdapter adapter)
            where TContext : IDatabaseContext
        {
            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            var adapterFactory = CreateAdapterFactory(adapter);
            return new DatabaseRepositoryProvider(GenericDatabaseFactory<TContext>.Default, entityMapper,  adapterFactory);
        }

        /// <summary>
        /// Creates a mock repository provider factory.
        /// </summary>
        /// <param name="provider">
        /// The provider to return.
        /// </param>
        /// <returns>
        /// A mocked <see cref="Startitecture.Orm.Common.IRepositoryProviderFactory"/>.
        /// </returns>
        public static IRepositoryProviderFactory CreateProviderFactory([NotNull] IRepositoryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();
            repositoryProviderFactory.Stub(factory => factory.Create()).Return(provider);
            return repositoryProviderFactory;
        }

        /// <summary>
        /// Creates a mock repository adapter.
        /// </summary>
        /// <returns>
        /// The mocked <see cref="Startitecture.Orm.Common.IRepositoryAdapter"/>.
        /// </returns>
        public static IRepositoryAdapter CreateAdapter()
        {
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            return repositoryAdapter;
        }

        /// <summary>
        /// The create adapter factory.
        /// </summary>
        /// <param name="adapter">
        /// The adapter to return from the factory.
        /// </param>
        /// <returns>
        /// A mocked <see cref="Startitecture.Orm.Common.IRepositoryAdapterFactory"/> that returns the <paramref name="adapter"/>.
        /// </returns>
        public static IRepositoryAdapterFactory CreateAdapterFactory([NotNull] IRepositoryAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(adapter);
            return repositoryAdapterFactory;
        }

        /// <summary>
        /// Creates a mock structured command provider. This must be stubbed out for each structure type.
        /// </summary>
        /// <returns>
        /// A mocked <see cref="Startitecture.Orm.Sql.IStructuredCommandProvider"/>.
        /// </returns>
        public static IStructuredCommandProvider CreateStructuredCommandProvider()
        {
            return MockRepository.GenerateMock<IStructuredCommandProvider>();
        }

        /// <summary>
        /// The create structured command provider factory.
        /// </summary>
        /// <param name="structuredSqlCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Sql.IStructuredCommandProviderFactory"/>.
        /// </returns>
        public static IStructuredCommandProviderFactory CreateStructuredCommandProviderFactory(IStructuredCommandProvider structuredSqlCommandProvider)
        {
            var structuredSqlCommandProviderFactory = MockRepository.GenerateMock<IStructuredCommandProviderFactory>();
            structuredSqlCommandProviderFactory.Stub(factory => factory.Create(Arg<IDatabaseContextProvider>.Is.Anything))
                .Return(structuredSqlCommandProvider);

            return structuredSqlCommandProviderFactory;
        }
    }
}