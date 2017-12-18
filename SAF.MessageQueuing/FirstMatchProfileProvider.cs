// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstMatchProfileProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Routes messages to the first matching profile as provided in the constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Routes messages to the first matching profile as provided in the constructor.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that is routed by the resolved profile.
    /// </typeparam>
    public abstract class FirstMatchProfileProvider<TMessage> : IRoutingProfileProvider<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Fields

        /// <summary>
        /// The profiles to use when matching.
        /// </summary>
        private readonly Lazy<IEnumerable<IMessageRoutingProfile<TMessage>>> profiles;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.FirstMatchProfileProvider`1"/> class.
        /// </summary>
        /// <param name="continuationProvider">
        /// The continuation provider for the specified profiles.
        /// </param>
        protected FirstMatchProfileProvider(IRoutingContinuationProvider<TMessage> continuationProvider)
        {
            if (continuationProvider == null)
            {
                throw new ArgumentNullException("continuationProvider");
            }

            this.ContinuationProvider = continuationProvider;
            this.profiles = new Lazy<IEnumerable<IMessageRoutingProfile<TMessage>>>(this.LoadProfiles);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the name to apply to the pending request queue.
        /// </summary>
        public abstract string PendingMessageQueueName { get; }

        /// <summary>
        /// Gets the equality comparer for determining message identity.
        /// </summary>
        public abstract IEqualityComparer<TMessage> IdentityComparer { get; }

        /// <summary>
        /// Gets the comparer for determining message priority.
        /// </summary>
        public abstract IComparer<TMessage> PriorityComparer { get; }

        /// <summary>
        /// Gets the comparer for determining message sequence.
        /// </summary>
        public abstract IComparer<TMessage> SequenceComparer { get; }

        /// <summary>
        /// Gets the equality comparer for superseding duplicate messages.
        /// </summary>
        public abstract IEqualityComparer<TMessage> DuplicateEqualityComparer { get; }

        /// <summary>
        /// Gets the failure route.
        /// </summary>
        public abstract IServiceRoute<TMessage> FailureRoute { get; }

        /// <summary>
        /// Gets the failure policy for routing failed messages.
        /// </summary>
        public abstract IRoutingFailurePolicy<TMessage> FailurePolicy { get; }

        /// <summary>
        /// Gets the continuation provider.
        /// </summary>
        public IRoutingContinuationProvider<TMessage> ContinuationProvider { get; private set; }

        /// <summary>
        /// Resolves a routing profile for the specified <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">
        /// The request to resolve.
        /// </param>
        /// <exception cref="BusinessException">
        /// No profile matched the specified <paramref name="message"/>.
        /// </exception>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IMessageRoutingProfile`1"/> that matches the request, or <c>null</c> if no profile 
        /// could be resolved.
        /// </returns>
        public IMessageRoutingProfile<TMessage> ResolveProfile(TMessage message)
        {
            var routingProfiles = this.profiles.Value;
            var messageRoutingProfiles = routingProfiles as IList<IMessageRoutingProfile<TMessage>> ?? routingProfiles.ToList();

            if (messageRoutingProfiles.Any() == false)
            {
                throw new BusinessException(this, ErrorMessages.NoProfilesSpecified);
            }

            if (messageRoutingProfiles.Any(x => x == null))
            {
                throw new BusinessException(this, ValidationMessages.RoutingProfileWasNull);
            }

            var profile = messageRoutingProfiles.FirstOrDefault(x => x.MatchesProfile(message));

            if (profile == null)
            {
                throw new BusinessException(message, String.Format(ValidationMessages.RoutingProfileNotFoundForMessage, message));
            }

            return profile;
        }

        /// <summary>
        /// Finalizes a routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The delivery to finalize.
        /// </param>
        public abstract void FinalizeRequest(MessageRoutingRequest<TMessage> routingRequest);

        #endregion

        /// <summary>
        /// Sets the profiles for the profile provider.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:SAF.MessageQueuing.IMessageRoutingProfile`1"/> elements contained in the profile.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Usage is not difficult to understand.")]
        protected abstract IEnumerable<IMessageRoutingProfile<TMessage>> LoadProfiles();
    }
}