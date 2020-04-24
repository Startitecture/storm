// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PocoDelegateInfo.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// CContains metadata for a POCO delegate.
    /// </summary>
    public class PocoDelegateInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PocoDelegateInfo"/> class.
        /// </summary>
        /// <param name="mappingDelegate">
        /// The mapping delegate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mappingDelegate"/> is null.
        /// </exception>
        public PocoDelegateInfo([NotNull] Delegate mappingDelegate)
        {
            this.MappingDelegate = mappingDelegate ?? throw new ArgumentNullException(nameof(mappingDelegate));
        }

        /// <summary>
        /// Gets the mapping delegate.
        /// </summary>
        public Delegate MappingDelegate { get; }
    }
}