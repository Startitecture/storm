// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNodeTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Core.Tests
{
    using System.Collections.Generic;
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
        public void FirstCommonAncestor_TreeNodeOfIntegersCompletelySeparatedNodes_ReturnsTopNode()
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
        public void FirstCommonAncestor_TreeNodeOfIntegersSharedParent_ReturnsParent()
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
        public void FirstCommonAncestor_TreeNodeOfIntegersSameNode_ReturnsNode()
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
        public void FirstCommonAncestor_TreeNodeOfIntegersFourPaths_ReturnsCommonParent()
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

        /// <summary>
        /// The first or default test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_IntegerTreeNodeWithUniqueValues_GetsFirstValue()
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
            node14.AddChildren(15, 17);

            var actual = rootNode.FirstOrDefault(i => i == 14);
            Assert.AreSame(node14, actual);
        }

        /// <summary>
        /// The first or default test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_IntegerTreeNodeWithDuplicateValues_GetsFirstValue()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 14* 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            var childrenOf6 = firstLevel.First().AddChildren(6).First().AddChildren(14, 12).ToList();
            childrenOf6.Last().AddChildren(16);
            var expected = childrenOf6.First();
            firstLevel.Skip(1).First().AddChildren(7, 4);
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            node5Children.First().AddChildren(13);
            var node14B = node5Children.Skip(1).First().AddChildren(14).First();
            node14B.AddChildren(15, 17);

            var actual = rootNode.FirstOrDefault(i => i == 14);
            Assert.AreSame(expected, actual);
        }

        /// <summary>
        /// The traverse test.
        /// </summary>
        [TestMethod]
        public void Traverse_IntegerTreeNode_AllNodesTraversed()
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
            node14.AddChildren(15, 17);

            var expected = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var actual = new List<int>();
            rootNode.Traverse(
                i =>
                    {
                        i -= i;
                        actual.Add(i);
                    });

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The remove child test.
        /// </summary>
        [TestMethod]
        public void RemoveChild_IntegerTreeNode_AllChildrenCascadingRemoved()
        {
            // Create a tree like the following
            // 1
            // 2 3 5
            // 6 7 4 9 10
            // 11 12 13 14
            // 16 15 17
            var rootNode = new TreeNode<int>(1);
            var firstLevel = rootNode.AddChildren(2, 3, 5).ToList();
            var nodeToRemove = firstLevel.First().AddChildren(6).First();
            nodeToRemove.AddChildren(11, 12).Last().AddChildren(16);
            firstLevel.Skip(1).First().AddChildren(7, 4);
            var node5Children = firstLevel.Skip(2).First().AddChildren(9, 10).ToList();
            node5Children.First().AddChildren(13);
            var node14 = node5Children.Skip(1).First().AddChildren(14).First();
            node14.AddChildren(15, 17);

            var removed = nodeToRemove.Parent.RemoveChild(nodeToRemove);
            Assert.IsTrue(removed);

            foreach (var number in new List<int> { 6, 11, 12, 16 })
            {
                Assert.IsNull(rootNode.FirstOrDefault(i => i == number));
            }
        }

        /// <summary>
        /// The flatten test.
        /// </summary>
        [TestMethod]
        public void Flatten_IntegerTreeNode_EquivalentToExpectedList()
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
            node14.AddChildren(15, 17);

            var expected  = new List<int> { 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
            var actual = rootNode.Flatten().ToList();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}