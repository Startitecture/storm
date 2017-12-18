// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistributionChannelGroup.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Observer;
    using SAF.StringResources;

    /// <summary>
    /// Contains a group of distribution channels.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that is distributed.
    /// </typeparam>
    public abstract class DistributionChannelGroup<TMessage> : IDistributionChannelGroup<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The status selector.
        /// </summary>
        private static readonly Func<IDistributionChannel<TMessage>, ComponentStatus> StatusSelector = x => x.ComponentStatus;

        /// <summary>
        /// The availability selector.
        /// </summary>
        private static readonly Func<IDistributionChannel<TMessage>, bool> AvailabilitySelector = x => x.ComponentStatus.IsAvailable;

        #region Fields

        /// <summary>
        /// The status comparer.
        /// </summary>
        private readonly IComparer<ComponentStatus> statusComparer;

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The distribution channels.
        /// </summary>
        private readonly List<IDistributionChannel<TMessage>> distributionChannels = new List<IDistributionChannel<TMessage>>();

        /// <summary>
        /// The update lock.
        /// </summary>
        private readonly object updateLock = new object();

        /// <summary>
        /// The most available channel in the group.
        /// </summary>
        private IDistributionChannel<TMessage> bestChannel;

        /// <summary>
        /// Indicates whether the current object is disposed.
        /// </summary>
        private bool disposed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionChannelGroup{TMessage}"/> class.
        /// </summary>
        /// <param name="statusComparer">
        /// The status comparer.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        protected DistributionChannelGroup(IComparer<ComponentStatus> statusComparer, IActionEventProxy actionEventProxy)
        {
            this.statusComparer = statusComparer;
            this.actionEventProxy = actionEventProxy;
        }

        #region Public Properties

        /// <summary>
        /// Gets the component status for the local distribution channel.
        /// </summary>
        public abstract ComponentStatus LocalStatus { get; }

        /// <summary>
        /// Gets the best channel based on the channel status comparer in the current instance.
        /// </summary>
        [DoNotLog]
        public IDistributionChannel<TMessage> BestChannel
        {
            get
            {
                lock (this.distributionChannels)
                {
                    if (this.disposed)
                    {
                        throw new ObjectDisposedException(Convert.ToString(this));
                    }
                }

                lock (this.updateLock)
                {
                    return this.bestChannel ?? (this.bestChannel = this.GetBestChannel(this.distributionChannels, this.statusComparer));
                }
            }
        }

        /// <summary>
        /// Gets all aborted requests across the current group.
        /// </summary>
        [DoNotLog]
        public MessageRoutingRequestCollection<TMessage> AbortedRequests
        {
            get
            {
                return
                    new MessageRoutingRequestCollection<TMessage>(
                        this.distributionChannels.SelectMany(x => x.AbortedMessages).ToList());
            }
        }

        /// <summary>
        /// Gets all active requests across the current group.
        /// </summary>
        public MessageRoutingRequestCollection<TMessage> ActiveRequests
        {
            get
            {
                return
                    new MessageRoutingRequestCollection<TMessage>(this.distributionChannels.SelectMany(x => x.ActiveMessages).ToList());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the message is active in the local context of the distribution group.
        /// </summary>
        /// <param name="message">
        /// The message to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the message is active in local context of the distribution group; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsLocallyActive(TMessage message);

        /// <summary>
        /// Gets the channel that is actively routing the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IDistributionChannel`1"/> that is actively routing the specified message, or null if 
        /// the message is not found in any of the distribution channels.
        /// </returns>
        public IDistributionChannel<TMessage> GetCurrentChannel(TMessage message)
        {
            lock (this.distributionChannels)
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(Convert.ToString(this));
                }
            }

            return
                this.distributionChannels.AsParallel()
                    .Where(x => x.ComponentStatus.IsAvailable)
                    .FirstOrDefault(x => x.IsActive(message));
        }

        /// <summary>
        /// Determines if the current group contains the specified channel.
        /// </summary>
        /// <param name="channel">
        /// The channel to query for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the channel is contained within the group; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsChannel(IDistributionChannel<TMessage> channel)
        {
            return this.distributionChannels.Contains(channel);
        }

        /// <summary>
        /// Registers a distribution channel with the current group.
        /// </summary>
        /// <param name="channel">
        /// The channel to register.
        /// </param>
        public void RegisterChannel(IDistributionChannel<TMessage> channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            lock (this.distributionChannels)
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(Convert.ToString(this));
                }

                this.distributionChannels.Add(channel);
                channel.IsAvailable += this.UpdateBestChannel;
                channel.IsUnavailable += this.UpdateBestChannel;
                this.actionEventProxy.RecordAction(channel);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the current instance is being explicitly disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.distributionChannels)
                {
                    this.disposed = true;

                    foreach (var distributionChannel in this.distributionChannels)
                    {
                        this.actionEventProxy.RecordAction(distributionChannel);
                        distributionChannel.IsAvailable -= this.UpdateBestChannel;
                        distributionChannel.IsUnavailable -= this.UpdateBestChannel;
                        distributionChannel.Dispose();
                    }

                    this.distributionChannels.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the best channel within the group.
        /// </summary>
        /// <param name="channels">
        /// The channels to evaluate.
        /// </param>
        /// <param name="comparer">
        /// The status comparer.
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IDistributionChannel`1"/> that has the best availability.
        /// </returns>
        private IDistributionChannel<TMessage> GetBestChannel(
            IEnumerable<IDistributionChannel<TMessage>> channels, 
            IComparer<ComponentStatus> comparer)
        {
            if (channels == null)
            {
                throw new ArgumentNullException("channels");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            var distributionChannel =
                channels.AsParallel().Where(AvailabilitySelector).OrderBy(StatusSelector, comparer).FirstOrDefault();

            if (distributionChannel == null)
            {
                throw new OperationException(this, ErrorMessages.DistributionChannelsNotAvailable);
            }

            return distributionChannel;
        }

        /// <summary>
        /// Updates the best channel whenever availability changes in the channel group.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void UpdateBestChannel(object sender, EventArgs eventArgs)
        {
            lock (this.updateLock)
            {
                this.bestChannel = this.GetBestChannel(this.distributionChannels, this.statusComparer);
                this.actionEventProxy.RecordAction(this.bestChannel);
            }
        }
    }
}