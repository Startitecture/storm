// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelatedEntityMappingContainerTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The related entity mapping container tests.
    /// </summary>
    [TestClass]
    public class RelatedEntityMappingContainerTests
    {
        /// <summary>
        /// The resolve unmapped entity test.
        /// </summary>
        [TestMethod]
        public void ResolveUnmappedEntity_ChildEntityWithRecursiveParentWithoutDependencyContainer_MatchesExpected()
        {
            var mapper = RepositoryMockFactory.CreateEntityMapper(
                expression =>
                    {
                        expression.AddProfile<ChildWithUnmappedResolutionProfile>();
                    });

            var container = new Container(435) { Name = "Container" };
            var parent = new Recursive(container, 85746) { Name = "RecursiveParent" };
            var recursive = new Recursive(container, 578) { Name = "RecursiveChild", Parent = parent };
            var expected = new Child(recursive, 45873);

            var row = mapper.Map<ChildRow>(expected);
            var actual = mapper.Map<Child>(row);

            Assert.IsNull(actual.Parent);

            // Apply the parent back to the recursive child, because this would not have done it.
            actual.Recursive.Parent = parent;
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The resolve unmapped entity test.
        /// </summary>
        [TestMethod]
        public void ResolveUnmappedEntity_ChildEntityWithRecursiveParentWithDependencyContainer_MatchesExpected()
        {
            var mapper = RepositoryMockFactory.CreateEntityMapper(
                expression =>
                {
                    expression.AddProfile<ChildWithUnmappedResolutionProfile>();
                });

            var container = new Container(435) { Name = "Container" };
            var parent = new Recursive(container, 85746) { Name = "RecursiveParent" };
            var recursive = new Recursive(container, 578) { Name = "RecursiveChild", Parent = parent };
            var expected = new Child(recursive, 45873);

            // Using the repository provider, we have a dependency container that we can use to get the parent.
            using (var dependencyContainer = new DependencyContainer())
            using (var repositoryProvider = MockRepository.GenerateMock<IRepositoryProvider>())
            {
                ////repositoryProvider.Stub(provider => provider.EntityMapper).Return(mapper);
                repositoryProvider.Stub(provider => provider.DependencyContainer).Return(dependencyContainer);

                // Prime the dependency container with the parent object.
                dependencyContainer.SetDependency(parent.RecursiveId, parent);

                var row = mapper.Map<ChildRow>(expected);
                row.SetTransactionProvider(repositoryProvider);
                var actual = mapper.Map<Child>(row);

                Assert.IsNotNull(actual.Parent);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The resolve related entity test.
        /// </summary>
        [TestMethod]
        public void MapRelatedEntity_UnmappableEntity_MatchesExpected()
        {
            var mapper = RepositoryMockFactory.CreateEntityMapper(
                expression =>
                {
                    expression.AddProfile<ChildWithIgnoreUnmappedProfile>();
                });

            var container = new Container(435) { Name = "Container" };
            var parent = new Recursive(container, 85746) { Name = "RecursiveParent" };
            var recursive = new Recursive(container, 578) { Name = "RecursiveChild", Parent = parent };
            var expected = new Child(recursive, 45873);

            var row = mapper.Map<ChildRow>(expected);
            var actual = mapper.Map<Child>(row);

            Assert.IsNull(actual.Parent);

            // Apply the parent back to the recursive child, because this would not have done it.
            actual.Recursive.Parent = parent;
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The container mapping profile.
        /// </summary>
        private class ChildWithIgnoreUnmappedProfile : EntityMappingProfile<Child, ChildRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChildWithIgnoreUnmappedProfile"/> class. 
            /// </summary>
            public ChildWithIgnoreUnmappedProfile()
            {
                this.SetPrimaryKey(child => child.ChildId, row => row.ChildId);

                this.CreateRelatedEntityProfile(child => child.Recursive, row => row.RecursiveId)
                    .MapRelatedEntity(recursive => recursive.Container, row => row.ContainerId)
                    .Ignore(recursive => recursive.Parent)
                    .MapEntityProperty(recursive => recursive.Name, row => row.RecursiveName);

                this.CreateRelatedEntityProfile(child => child.Container, row => row.ContainerId)
                    .MapEntityProperty(container => container.Name, row => row.ContainerName);
            }
        }

        /// <summary>
        /// The container mapping profile.
        /// </summary>
        private class ChildWithUnmappedResolutionProfile : EntityMappingProfile<Child, ChildRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChildWithUnmappedResolutionProfile" /> class.
            /// </summary>
            public ChildWithUnmappedResolutionProfile()
            {
                this.SetPrimaryKey(child => child.ChildId, row => row.ChildId);

                this.CreateRelatedEntityProfile(child => child.Recursive, row => row.RecursiveId)
                    .MapRelatedEntity(recursive => recursive.Container, row => row.ContainerId)
                    .ResolveUnmappedEntity(recursive => recursive.Parent, row => row.ParentRecursiveId)
                    .MapEntityProperty(recursive => recursive.Name, row => row.RecursiveName);

                this.CreateRelatedEntityProfile(child => child.Container, row => row.ContainerId)
                    .MapEntityProperty(container => container.Name, row => row.ContainerName);
            }
        }

        /// <summary>
        /// The container row.
        /// </summary>
        private class ContainerRow : TransactionContainer
        {
            /// <summary>
            /// Gets or sets the container id.
            /// </summary>
            public int ContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// The container.
        /// </summary>
        private class Container : IEquatable<Container>
        {
            /// <summary>
            /// The comparison properties.
            /// </summary>
            private static readonly Func<Container, object>[] ComparisonProperties = { item => item.Name };

            /// <summary>
            /// Initializes a new instance of the <see cref="Container"/> class.
            /// </summary>
            /// <param name="containerId">
            /// The container id.
            /// </param>
            public Container(int containerId)
            {
                this.ContainerId = containerId;
            }

            /// <summary>
            /// Gets the container id.
            /// </summary>
            public int ContainerId { get; private set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }


            #region Equality and Comparison Methods

            /// <summary>
            /// Determines if two values of the same type are equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator ==(Container valueA, Container valueB)
            {
                return EqualityComparer<Container>.Default.Equals(valueA, valueB);
            }

            /// <summary>
            /// Determines if two values of the same type are not equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator !=(Container valueA, Container valueB)
            {
                return !(valueA == valueB);
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return this.Name;
            }

            /// <summary>
            /// Serves as the default hash function. 
            /// </summary>
            /// <returns>
            /// A hash code for the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return Evaluate.GenerateHashCode(this, ComparisonProperties);
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <returns>
            /// true if the specified object  is equal to the current object; otherwise, false.
            /// </returns>
            /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                return Evaluate.Equals(this, obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Container other)
            {
                return Evaluate.Equals(this, other, ComparisonProperties);
            }

            #endregion

        }

        /// <summary>
        /// The recursive row.
        /// </summary>
        private class RecursiveRow : TransactionContainer
        {
            /// <summary>
            /// Gets or sets the recursive id.
            /// </summary>
            public int RecursiveId { get; set; }

            /// <summary>
            /// Gets or sets the container id.
            /// </summary>
            public int ContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the parent recursive id.
            /// </summary>
            public int? ParentRecursiveId { get; set; }

            /// <summary>
            /// Gets or sets the container name.
            /// </summary>
            public string ContainerName { get; set; }
        }

        /// <summary>
        /// The recursive entity.
        /// </summary>
        private class Recursive : IEquatable<Recursive>
        {
            /// <summary>
            /// The comparison properties.
            /// </summary>
            private static readonly Func<Recursive, object>[] ComparisonProperties =
                {
                    item => item.Container,
                    item => item.Name,
                    item => item.ParentRecursiveId
                };

            /// <summary>
            /// Initializes a new instance of the <see cref="Recursive"/> class.
            /// </summary>
            /// <param name="container">
            /// The container.
            /// </param>
            public Recursive(Container container)
                : this(container, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Recursive"/> class.
            /// </summary>
            /// <param name="container">
            /// The container.
            /// </param>
            /// <param name="recursiveId">
            /// The recursive id.
            /// </param>
            public Recursive(Container container, int? recursiveId)
            {
                this.Container = container;
                this.RecursiveId = recursiveId;
            }

            /// <summary>
            /// Gets the recursive id.
            /// </summary>
            public int? RecursiveId { get; private set; }

            /// <summary>
            /// Gets the container.
            /// </summary>
            public Container Container { get; private set; }

            /// <summary>
            /// Gets the container id.
            /// </summary>
            public int? ContainerId
            {
                get
                {
                    return this.Container?.ContainerId;
                }
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            public Recursive Parent { get; set; }

            /// <summary>
            /// Gets the parent recursive id.
            /// </summary>
            public int? ParentRecursiveId
            {
                get
                {
                    return this.Parent?.RecursiveId;
                }
            }

            #region Equality and Comparison Methods

            /// <summary>
            /// Determines if two values of the same type are equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator ==(Recursive valueA, Recursive valueB)
            {
                return EqualityComparer<Recursive>.Default.Equals(valueA, valueB);
            }

            /// <summary>
            /// Determines if two values of the same type are not equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator !=(Recursive valueA, Recursive valueB)
            {
                return !(valueA == valueB);
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return this.Name;
            }

            /// <summary>
            /// Serves as the default hash function. 
            /// </summary>
            /// <returns>
            /// A hash code for the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return Evaluate.GenerateHashCode(this, ComparisonProperties);
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <returns>
            /// true if the specified object  is equal to the current object; otherwise, false.
            /// </returns>
            /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                return Evaluate.Equals(this, obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Recursive other)
            {
                return Evaluate.Equals(this, other, ComparisonProperties);
            }

            #endregion

        }

        /// <summary>
        /// The child row.
        /// </summary>
        private class ChildRow : TransactionContainer
        {
            /// <summary>
            /// Gets the child id.
            /// </summary>
            public int? ChildId { get; private set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the recursive id.
            /// </summary>
            public int RecursiveId { get; set; }

            /// <summary>
            /// Gets or sets the container id.
            /// </summary>
            public int ContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string RecursiveName { get; set; }

            /// <summary>
            /// Gets or sets the parent recursive id.
            /// </summary>
            public int? ParentRecursiveId { get; set; }

            /// <summary>
            /// Gets or sets the container name.
            /// </summary>
            public string ContainerName { get; set; }
        }

        /// <summary>
        /// The child entity.
        /// </summary>
        private class Child : IEquatable<Child>
        {
            /// <summary>
            /// The comparison properties.
            /// </summary>
            private static readonly Func<Child, object>[] ComparisonProperties = { item => item.Recursive, item => item.Description };

            /// <summary>
            /// Initializes a new instance of the <see cref="Child"/> class.
            /// </summary>
            /// <param name="recursive">
            /// The recursive entity.
            /// </param>
            public Child(Recursive recursive)
                : this(recursive, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Child"/> class.
            /// </summary>
            /// <param name="recursive">
            /// The recursive entity.
            /// </param>
            /// <param name="childId">
            /// The child id.
            /// </param>
            public Child(Recursive recursive, int? childId)
            {
                this.ChildId = childId;
                this.Recursive = recursive;
            }

            /// <summary>
            /// Gets the child id.
            /// </summary>
            public int? ChildId { get; private set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets the recursive entity.
            /// </summary>
            public Recursive Recursive { get; private set; }

            /// <summary>
            /// Gets the recursive entity id.
            /// </summary>
            public int? RecursiveId
            {
                get
                {
                    return this.Recursive?.RecursiveId;
                }
            }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            public Recursive Parent
            {
                get
                {
                    return this.Recursive?.Parent;
                }
            }

            /// <summary>
            /// Gets the recursive parent id.
            /// </summary>
            public int? RecursiveParentId
            {
                get
                {
                    return this.Parent?.RecursiveId;
                }
            }

            /// <summary>
            /// Gets the container.
            /// </summary>
            public Container Container
            {
                get
                {
                    return this.Recursive.Container;
                }
            }

            /// <summary>
            /// Gets the container id.
            /// </summary>
            public int? ContainerId
            {
                get
                {
                    return this.Container?.ContainerId;
                }
            }
            
            #region Equality and Comparison Methods

            /// <summary>
            /// Determines if two values of the same type are equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator ==(Child valueA, Child valueB)
            {
                return EqualityComparer<Child>.Default.Equals(valueA, valueB);
            }

            /// <summary>
            /// Determines if two values of the same type are not equal.
            /// </summary>
            /// <param name="valueA">
            /// The first value to compare.
            /// </param>
            /// <param name="valueB">
            /// The second value to compare.
            /// </param>
            /// <returns>
            /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
            /// </returns>
            public static bool operator !=(Child valueA, Child valueB)
            {
                return !(valueA == valueB);
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return this.Description;
            }

            /// <summary>
            /// Serves as the default hash function. 
            /// </summary>
            /// <returns>
            /// A hash code for the current object.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return Evaluate.GenerateHashCode(this, ComparisonProperties);
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <returns>
            /// true if the specified object  is equal to the current object; otherwise, false.
            /// </returns>
            /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                return Evaluate.Equals(this, obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Child other)
            {
                return Evaluate.Equals(this, other, ComparisonProperties);
            }

            #endregion
        }
    }
}