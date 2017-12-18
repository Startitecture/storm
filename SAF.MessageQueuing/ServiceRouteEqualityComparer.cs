// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRouteEqualityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The service route equality comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The service route equality comparer.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message processed by the route.
    /// </typeparam>
    public class ServiceRouteEqualityComparer<TMessage> : EqualityComparer<IServiceRoute<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Static Fields

        /// <summary>
        /// The instance comparer.
        /// </summary>
        private static readonly ServiceRouteEqualityComparer<TMessage> InstanceComparer =
            new ServiceRouteEqualityComparer<TMessage>(route => route.InstanceIdentifier);

        /// <summary>
        /// The name and type comparer.
        /// </summary>
        private static readonly ServiceRouteEqualityComparer<TMessage> NameAndTypeComparer =
            new ServiceRouteEqualityComparer<TMessage>(route => route.Name, route => route.GetType().FullName);

        /// <summary>
        /// The name comparer.
        /// </summary>
        private static readonly ServiceRouteEqualityComparer<TMessage> NameComparer =
            new ServiceRouteEqualityComparer<TMessage>(route => route.Name);

        #endregion

        #region Fields

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private readonly Func<IServiceRoute<TMessage>, object>[] comparisonProperties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRouteEqualityComparer{TMessage}"/> class.
        /// </summary>
        /// <param name="comparisonProperties">
        /// The comparison properties.
        /// </param>
        private ServiceRouteEqualityComparer(params Func<IServiceRoute<TMessage>, object>[] comparisonProperties)
        {
            this.comparisonProperties = comparisonProperties;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the equality comparer for service route instances.
        /// </summary>
        public static ServiceRouteEqualityComparer<TMessage> Instance
        {
            get
            {
                return InstanceComparer;
            }
        }

        /// <summary>
        /// Gets the equality comparer for service route names.
        /// </summary>
        public static ServiceRouteEqualityComparer<TMessage> Name
        {
            get
            {
                return NameComparer;
            }
        }

        /// <summary>
        /// Gets the equality comparer for service route names and types.
        /// </summary>
        public static ServiceRouteEqualityComparer<TMessage> NameAndType
        {
            get
            {
                return NameAndTypeComparer;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type 
        /// <see cref="T:SAF.MessageQueuing.IServiceRoute`1"/> are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
        public override bool Equals(IServiceRoute<TMessage> x, IServiceRoute<TMessage> y)
        {
            return Evaluate.Equals(x, y, this.comparisonProperties);
        }

        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The object for which to get a hash code.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public override int GetHashCode(IServiceRoute<TMessage> obj)
        {
            return Evaluate.GenerateHashCode(obj, this.comparisonProperties);
        }

        #endregion
    }
}