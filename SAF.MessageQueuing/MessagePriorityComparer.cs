// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagePriorityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Compares two  instances by priority, processing delay, request time, document location
//   and document identifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Compares two <see cref="IPriorityMessage"/> instances by priority, processing delay, request time, document location 
    /// and document identifier.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> to compare.
    /// </typeparam>
    public class MessagePriorityComparer<TMessage> : Comparer<TMessage>
        where TMessage : IPriorityMessage
    {
        /// <summary>
        /// The message comparer.
        /// </summary>
        private static readonly MessagePriorityComparer<TMessage> MessageComparer = new MessagePriorityComparer<TMessage>();

        /// <summary>
        /// Gets the priority message comparer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Using the Comparer<T> pattern.")]
        public static MessagePriorityComparer<TMessage> Priority
        {
            get
            {
                return MessageComparer;
            }
        }

        /// <summary>
        /// Performs a comparison of two <see cref="IPriorityMessage"/> objects and returns a value indicating whether one 
        /// object is less than, equal to, or greater than the other. If the 
        /// <see cref="P:SAF.MessageQueuing.IPriorityMessage.RequestGuid"/> properties are the same, this method will
        /// return <c>0</c>.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the 
        /// following table.
        /// <para>
        /// Value Meaning 
        /// </para>
        /// <para>
        /// Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// </para>
        /// <para>
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// </para>
        /// <para>
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </para>
        /// </returns>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        public override int Compare(TMessage x, TMessage y)
        {
            if (Evaluate.IsNull(x) && Evaluate.IsNull(y))
            {
                return 0;
            }

            if (Evaluate.IsNull(x))
            {
                return -1;
            }

            if (Evaluate.IsNull(y))
            {
                return 1;
            }

            var compareTime = DateTimeOffset.Now;
            var priorityComparison = GetPriority(x, compareTime).CompareTo(GetPriority(y, compareTime));
            return priorityComparison != 0 ? priorityComparison : Evaluate.Compare(x, y, message => message.RequestGuid);
        }

        /// <summary>
        /// Gets the priority for the current request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="compareTime">
        /// The comparison time.
        /// </param>
        /// <returns>
        /// A <see cref="double"/> representing the current priority.
        /// </returns>
        private static double GetPriority(TMessage request, DateTimeOffset compareTime)
        {
            var minutesLeft = (request.Deadline - compareTime).TotalMinutes;
            var escalationPoint = request.Deadline - request.EscalationThreshold;
            var minutesIntoEscalationThreshold = (compareTime - escalationPoint).TotalMinutes;
            var escalationThresholdMinutes = request.EscalationThreshold != TimeSpan.Zero ? request.EscalationThreshold.TotalMinutes : 1;

            var escalationPercentage = minutesIntoEscalationThreshold / escalationThresholdMinutes;
            var priority = minutesLeft - (minutesLeft * escalationPercentage);

            return priority;
        }
    }
}
