// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectWithAttribute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Declares the selection statement to use for the current class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// Declares the selection statement to use for the current class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SelectWithAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectWithAttribute"/> class.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        public SelectWithAttribute(string statement)
        {
            this.Statement = statement;
        }

        /// <summary>
        /// Gets the selection statement.
        /// </summary>
        public string Statement { get; private set; }
    }
}
