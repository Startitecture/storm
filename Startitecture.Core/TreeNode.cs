// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNode.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The tree node.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item in the node.
    /// </typeparam>
    public class TreeNode<T>
    {
        /// <summary>
        /// The children.
        /// </summary>
        private readonly List<TreeNode<T>> children = new List<TreeNode<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public TreeNode(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public ReadOnlyCollection<TreeNode<T>> Children => this.children.AsReadOnly();

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public TreeNode<T> Parent { get; private set; }

        /// <summary>
        /// Gets the value of the node.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Retrieves the node at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index to retrieve a node from.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{T}"/> at the specified index.
        /// </returns>
        public TreeNode<T> this[int index] => this.children[index];

        /// <summary>
        /// Adds a child to the current node.
        /// </summary>
        /// <param name="value">
        /// The value to set at the node.
        /// </param>
        /// <returns>
        /// The newly added <see cref="TreeNode{T}"/>.
        /// </returns>
        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            this.children.Add(node);
            return node;
        }

        /// <summary>
        /// Adds children to the current node.
        /// </summary>
        /// <param name="values">
        /// The values to add.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="TreeNode{T}"/> items added to the current node.
        /// </returns>
        public IEnumerable<TreeNode<T>> AddChildren(params T[] values)
        {
            return values.Select(this.AddChild).ToArray();
        }

        /// <summary>
        /// Flattens the tree node and its children into an enumerable.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> of the node and its children.
        /// </returns>
        public IEnumerable<T> Flatten()
        {
            return new[]
                       {
                           this.Value
                       }.Concat(this.children.SelectMany(x => x.Flatten()));
        }

        /// <summary>
        /// Removes the specified child node from the current node.
        /// </summary>
        /// <param name="node">
        /// The node to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if the node is removed; otherwise, <c>false</c>.
        /// </returns>
        public bool RemoveChild(TreeNode<T> node)
        {
            return this.children.Remove(node);
        }

        /// <summary>
        /// Traverses the tree to perform an action on the values.
        /// </summary>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        public void Traverse(Action<T> action)
        {
            action(this.Value);

            foreach (var child in this.children)
            {
                child.Traverse(action);
            }
        }

        /// <summary>
        /// Finds the first tree node with the value matching the predicate.
        /// </summary>
        /// <param name="predicate">
        /// The predicate to find.
        /// </param>
        /// <returns>
        /// The first <see cref="TreeNode{T}"/> matching the predicate, or null if no match is found.
        /// </returns>
        public TreeNode<T> FirstOrDefault(Predicate<T> predicate)
        {
            if (predicate(this.Value))
            {
                return this;
            }

            foreach (var child in this.Children)
            {
                var result = child.FirstOrDefault(predicate);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first common ancestor of the selected endpoint nodes.
        /// </summary>
        /// <param name="endpoints">
        /// The endpoints to find the common ancestor of.
        /// </param>
        /// <returns>
        /// The lowest level <see cref="TreeNode{T}"/> in which all of the <paramref name="endpoints"/> share a path.
        /// </returns>
        public TreeNode<T> FirstCommonAncestor(IEnumerable<TreeNode<T>> endpoints)
        {
            // Assumption - the top node will always be a common parent.
            var pathsList = new List<LinkedList<TreeNode<T>>>();

            foreach (var endpoint in endpoints)
            {
                var path = new LinkedList<TreeNode<T>>();

                // Add the endpoint itself for situations in which two endpoints share a common immediate parent.
                path.AddFirst(endpoint);
                var parent = endpoint.Parent;

                while (parent != null)
                {
                    // Each parent gets precedence in the list over the last.
                    path.AddFirst(parent);
                    parent = parent.Parent;
                }

                pathsList.Add(path);
            }

            // We assume here that all of the first values are the root tree node.
            var level = 1;
            var ancestor = pathsList.First().First();
            var allCommon = true;

            while (allCommon)
            {
                foreach (var pathList in pathsList)
                {
                    var candidate = pathList.Skip(level).FirstOrDefault();

                    // References are the same for the same parent in the tree.
                    if (ReferenceEquals(candidate?.Parent, ancestor))
                    {
                        continue;
                    }

                    allCommon = false;
                    ancestor = ancestor.Parent;
                    break;
                }

                if (allCommon)
                {
                    ancestor = pathsList.First().Skip(level).First();
                }

                level++;
            }

            return ancestor;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Convert.ToString(this.Value);
        }
    }
}