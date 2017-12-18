// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentAvailabilityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SAF.Core;
    using SAF.Observer;
    using SAF.StringResources;

    /// <summary>
    /// Compares the status of two components for availability. The most available component will be placed ahead of the least 
    /// available component.
    /// </summary>
    public class ComponentAvailabilityComparer : Comparer<ComponentStatus>
    {
        /// <summary>
        /// The resource weight comparer.
        /// </summary>
        private static readonly ComponentAvailabilityComparer RankedWeightComparer =
            new ComponentAvailabilityComparer(AvailabilityComparison.RankedWeight);

        /// <summary>
        /// The uptime comparer.
        /// </summary>
        private static readonly ComponentAvailabilityComparer TotalWeightComparer =
            new ComponentAvailabilityComparer(AvailabilityComparison.TotalWeight);

        /// <summary>
        /// The uptime comparer.
        /// </summary>
        private static readonly ComponentAvailabilityComparer RankedTotalWeightComparer =
            new ComponentAvailabilityComparer(AvailabilityComparison.RankedTotalWeight);

        /// <summary>
        /// The resource weight.
        /// </summary>
        private static readonly Func<RankedResourceStatus, double> ResourceWeight = status => status.ResourceStatus.Weight;

        /// <summary>
        /// The group key selector.
        /// </summary>
        private static readonly Func<IGrouping<int, RankedResourceStatus>, int> GroupKeySelector = g => g.Key;

        /// <summary>
        /// The comparison.
        /// </summary>
        private readonly AvailabilityComparison comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAvailabilityComparer"/> class.
        /// </summary>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        private ComponentAvailabilityComparer(AvailabilityComparison comparison)
        {
            this.comparison = comparison;
        }

        /// <summary>
        /// Gets the ranked weight comparer. Each rank is compared and the component with the first rank having a lower weight will be 
        /// selected first.
        /// </summary>
        public static ComponentAvailabilityComparer RankedWeight
        {
            get
            {
                return RankedWeightComparer;
            }
        }

        /// <summary>
        /// Gets the total weight comparer. The component with the lowest total weight will be selected first.
        /// </summary>
        public static ComponentAvailabilityComparer TotalWeight
        {
            get
            {
                return TotalWeightComparer;
            }
        }

        /// <summary>
        /// Gets the ranked total weight comparer. Each resource weight is divided by its rank and the component with the lowest
        /// result is selected first.
        /// </summary>
        public static ComponentAvailabilityComparer RankedTotalWeight
        {
            get
            {
                return RankedTotalWeightComparer;
            }
        }

        #region Public Methods and Operators

        /// <summary>
        /// Compares two <see cref="SAF.Observer.ComponentStatus"/> items to determine which component has the highest availability.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the
        /// following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than
        /// <paramref name="y"/>.
        /// </returns>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        public override int Compare(ComponentStatus x, ComponentStatus y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            switch (this.comparison)
            {
                case AvailabilityComparison.TotalWeight:
                    return Evaluate.Compare(x.ResourceWeight, y.ResourceWeight);

                case AvailabilityComparison.RankedWeight:

                    var groupsX = x.Resources.GroupBy(status => status.ResourceRank).ToList();
                    var groupsY = y.Resources.GroupBy(status => status.ResourceRank).ToList();
                    var matchedRanks = Evaluate.CollectionEquals(groupsX.Select(GroupKeySelector), groupsY.Select(GroupKeySelector));

                    // If the groups are not evenly matched, then calculate the sum of the weights divided by their rank.
                    if (matchedRanks == false)
                    {
                        return Evaluate.Compare(x.RankedResourceWeight, y.RankedResourceWeight);
                    }

                    for (int i = 0; i < groupsX.Count; i++)
                    {
                        var valueA = groupsX[i].Sum(ResourceWeight);
                        var valueB = groupsY[i].Sum(ResourceWeight);
                        var weightCompare = Evaluate.Compare(valueA, valueB);

                        if (weightCompare != 0)
                        {
                            Trace.TraceInformation(
                                "Returning on rank {0} with {1}={2} ~ {3}={4} as {5}", 
                                i, 
                                groupsX[i].Key, 
                                valueA, 
                                groupsY[i].Key, 
                                valueB, 
                                weightCompare);

                            return weightCompare;
                        }
                    }

                    return 0;

                case AvailabilityComparison.RankedTotalWeight:
                    return Evaluate.Compare(x.RankedResourceWeight, y.RankedResourceWeight);

                default:
                    throw new NotSupportedException(String.Format(ValidationMessages.ActionIsNotSupportedForMethod, this.comparison));
            }
        }

        #endregion
    }
}