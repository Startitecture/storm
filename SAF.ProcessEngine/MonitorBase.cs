namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Timers;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Represents an event process that notifies monitors of activity.
    /// </summary>
    public class MonitorBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// An object that controls access to the notification queue.
        /// </summary>
        private readonly object notificationControl = new object();

        /// <summary>
        /// A list of property changes since the last interval.
        /// </summary>
        private readonly Dictionary<string, bool> propertyChanges = new Dictionary<string, bool>();

        /// <summary>
        /// A timer for event notification.
        /// </summary>
        private System.Timers.Timer notificationTimer = new System.Timers.Timer(100);

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorBase"/> class.
        /// </summary>
        protected MonitorBase()
        {
            this.NotificationInterval = TimeSpan.FromMilliseconds(100);
            this.notificationTimer.AutoReset = true;
            this.notificationTimer.Elapsed += this.ProcessNotificationsFromTimerInterval;
            this.notificationTimer.Start();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MonitorBase"/> class.
        /// </summary>
        ~MonitorBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when one of this monitor's properties has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the length of time between aggregate event notifications.
        /// </summary>
        public TimeSpan NotificationInterval { get; set; }

        /// <summary>
        /// Releases all the resources managed by this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources managed by this object.
        /// </summary>
        /// <param name="disposing">A value indicating whether this method is explicitly being called.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.notificationTimer != null)
                {
                    this.notificationTimer.Dispose();
                    this.notificationTimer = null;
                }
            }

            // Dispose of native resources here.
        }

        /// <summary>
        /// Use this method to detect changed properties using the AddChangedProperty event. This method is executed
        /// directly before property change events are fired on the notification interval.
        /// </summary>
        protected virtual void DetectChangedProperties()
        {
        }

        /// <summary>
        /// Adds a changed property name to the list of changed properties for this interval.
        /// </summary>
        /// <param name="property">The name of the property that has changed.</param>
        [System.Diagnostics.DebuggerHidden]
        protected void AddChangedProperty(string property)
        {
            lock (this.notificationControl)
            {
                if (!this.propertyChanges.ContainsKey(property))
                {
                    this.propertyChanges.Add(property, true);
                }
                else
                {
                    this.propertyChanges[property] = true;
                }
            }
        }

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> associated with the event.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler temp = this.PropertyChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Updates the caller with the sum of changes since the last interval.
        /// </summary>
        /// <param name="sender">The timer that controls the interval.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> associated with the event.</param>
        [System.Diagnostics.DebuggerHidden]
        private void ProcessNotificationsFromTimerInterval(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.DetectChangedProperties();

            lock (this.notificationControl)
            {
                foreach (string propertyKey in this.propertyChanges.Keys.ToList())
                {
                    if (this.propertyChanges[propertyKey])
                    {
                        this.OnPropertyChanged(new PropertyChangedEventArgs(propertyKey));
                        this.propertyChanges[propertyKey] = false;
                    }
                }
            }

            var timer = sender as Timer;

            if (timer != null)
            {
                timer.Interval = this.NotificationInterval.Duration().TotalMilliseconds;
            }
            else
            {
                throw new OperationException(
                    this,
                    String.Format(ValidationMessages.TypeIsNotInstanceOfExpectedType, sender, typeof(Timer)));
            }
        }
    }
}
