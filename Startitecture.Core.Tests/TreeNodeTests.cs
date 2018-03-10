// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNodeTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The tree node tests.
    /// </summary>
    [TestClass]
    public class TreeNodeTests
    {
        /// <summary>
        /// The first common ancestor test.
        /// </summary>
        [TestMethod]
        public void FirstCommonAncestor_TreeNodeOfIntsCompletelySeparatedNodes_ReturnsTopNode()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 11 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            var node16 = firstLevel.First().AddChildren(6).First().AddChildren(11, 12).Last().AddChildren(16).First();
            firstLevel.Skip(1).First().AddChildren(7, 4);
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            node5Children.First().AddChildren(13);
            var node15 = node5Children.Skip(1).First().AddChildren(14).First().AddChildren(15, 17).First();

            var actual = rootNode.FirstCommonAncestor(
                new[]
                    {
                        node16,
                        node15
                    });
            Assert.AreEqual(rootNode.Value, actual.Value);
        }

        /// <summary>
        /// The first common ancestor test.
        /// </summary>
        [TestMethod]
        public void FirstCommonAncestor_TreeNodeOfIntsSharedParent_ReturnsParent()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 11 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            firstLevel.First().AddChildren(6).First().AddChildren(11, 12).Last().AddChildren(16);
            firstLevel.Skip(1).First().AddChildren(7, 4);
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            node5Children.First().AddChildren(13);
            var node14 = node5Children.Skip(1).First().AddChildren(14).First();
            var node14Children = node14.AddChildren(15, 17).ToList();

            var actual = rootNode.FirstCommonAncestor(
                new[]
                    {
                        node14Children.First(),
                        node14Children.Last()
                    });
            Assert.AreEqual(node14.Value, actual.Value);
        }

        /// <summary>
        /// The first common ancestor test.
        /// </summary>
        [TestMethod]
        public void FirstCommonAncestor_TreeNodeOfIntsSameNode_ReturnsNode()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 11 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            firstLevel.First().AddChildren(6).First().AddChildren(11, 12).Last().AddChildren(16);
            firstLevel.Skip(1).First().AddChildren(7, 4);
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            node5Children.First().AddChildren(13);
            var node15 = node5Children.Skip(1).First().AddChildren(14).First().AddChildren(15, 17).First();

            var actual = rootNode.FirstCommonAncestor(
                new[]
                    {
                        node15,
                        node15
                    });
            Assert.AreEqual(node15.Value, actual.Value);
        }

        /// <summary>
        /// The first common ancestor test.
        /// </summary>
        [TestMethod]
        public void FirstCommonAncestor_TreeNodeOfIntsFourPaths_ReturnsCommonParent()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 11 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            firstLevel.First().AddChildren(6).First().AddChildren(11, 12).Last().AddChildren(16);
            var node7 = firstLevel.Skip(1).First().AddChildren(7, 4).First();
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            var node13 = node5Children.First().AddChildren(13).First();
            var node14 = node5Children.Skip(1).First().AddChildren(14).First();
            var node14Children = node14.AddChildren(15, 17).ToList();

            var actual = rootNode.FirstCommonAncestor(
                new[]
                    {
                        node7,
                        node14Children.First(),
                        node13,
                        node14Children.Last()
                    });
            Assert.AreEqual(rootNode.Value, actual.Value);
        }
    }
}