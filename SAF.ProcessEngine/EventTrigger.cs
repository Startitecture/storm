namespace SAF.ProcessEngine
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Base class for event-based triggers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that this trigger evaluates against its condition.</typeparam>
    public class EventTrigger<TEvent> : IEventTrigger
        where TEvent : EventArgs
    {
        /// <summary>
        /// Gets a predicate for 
        /// </summary>
        private static readonly Func<object, TEvent, bool> EventTypeMatchCondition = (s, e) => typeof(TEvent).IsAssignableFrom(e.GetType());

        /// <summary>
        /// The condition of the trigger, which must evaluate to true.
        /// </summary>
        private readonly Func<object, TEvent, bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrigger&lt;TEvent&gt;"/> class to match on event type.
        /// </summary>
        public EventTrigger()
            : this(EventTypeMatchCondition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTrigger&lt;TEvent&gt;"/> class, with the specified condition 
        /// that must evaluate to true.
        /// </summary>
        /// <param name="condition">The condition of the trigger.</param>
        public EventTrigger(Func<object, TEvent, bool> condition)
        {
            this.condition = condition;
        }

        /// <summary>
        /// Evaluates the provided event against the condition.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments associated with the event.</param>
        /// <returns>True if the condition evaluates to true, otherwise false.</returns>
        [DebuggerHidden]
        public bool IsConditionTrue(object sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e is TEvent)
            {
                return this.condition(sender, e as TEvent); //// && this.ValidateExternalConditions(sender, e);
            }
            else
            {
                return false;
            }
        }

        /////// <summary>
        /////// Validates external conditions related to this trigger class.
        /////// </summary>
        /////// <param name="sender">The sender of the event.</param>
        /////// <param name="e">Event data associated with the event.</param>
        /////// <returns>True if the external conditions are validated, otherwise false.</returns>
        ////[DebuggerHidden]
        ////protected virtual bool ValidateExternalConditions(object sender, EventArgs e)
        ////{
        ////    return true;
        ////}
    }
}
