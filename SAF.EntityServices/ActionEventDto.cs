// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionEventDto.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System;
    using System.Runtime.Serialization;

    using SAF.Core;

    /// <summary>
    /// A data contract (DTO) for action events.
    /// </summary>
    [DataContract]
    public class ActionEventDto : IEquatable<ActionEventDto>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} at {1} by {2}{3}";

        /// <summary>
        /// The error format.
        /// </summary>
        private const string ErrorFormat = ": {0}";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ActionEventDto, object>[] ComparisonProperties =
            {
                item => item.GlobalIdentifier,
                item => item.ActionName,
                item => item.ActionSource,
                item => item.AdditionalData,
                item => item.CompletionTime,
                item => item.Description,
                item => item.ErrorCode,
                item => item.ErrorData,
                item => item.ErrorMessage,
                item => item.ErrorType,
                item => item.FullErrorOutput,
                item => item.InitiationTime,
                item => item.Item,
                item => item.ItemType,
                item => item.UserAccount,
                item => item.UserDisplayName
            };

        /// <summary>
        /// Gets or sets the global identifier for this event.
        /// </summary>
        [DataMember]
        public Guid GlobalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        [DataMember]
        public string ActionName { get; set; }

        /// <summary>
        /// Gets or sets the action source.
        /// </summary>
        [DataMember]
        public string ActionSource { get; set; }

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        [DataMember]
        public string AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the target item.
        /// </summary>
        [DataMember]
        public string Item { get;  set; }

        /// <summary>
        /// Gets or sets the target item type.
        /// </summary>
        [DataMember]
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the completion time of the event.
        /// </summary>
        [DataMember]
        public DateTimeOffset CompletionTime { get; set; }

        /// <summary>
        /// Gets or sets the initiation time of the event.
        /// </summary>
        [DataMember]
        public DateTimeOffset InitiationTime { get; set; }

        /// <summary>
        /// Gets or sets the user account that initiated the request.
        /// </summary>
        [DataMember]
        public string UserAccount { get; set; }

        /// <summary>
        /// Gets or sets the user display name.
        /// </summary>
        [DataMember]
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        [DataMember]
        public string ErrorData { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        [DataMember]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the full error output.
        /// </summary>
        [DataMember]
        public string FullErrorOutput { get; set; }

        /// <summary>
        /// Determines whether two values of the same type are equal.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ActionEventDto firstValue, ActionEventDto secondValue)
        {
            return Evaluate.Equals(firstValue, secondValue);
        }

        /// <summary>
        /// Determines whether two values of the same type are not equal.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ActionEventDto firstValue, ActionEventDto secondValue)
        {
            return !(firstValue == secondValue);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(
                ToStringFormat,
                this.Description,
                this.InitiationTime,
                this.UserDisplayName,
                String.IsNullOrWhiteSpace(this.ErrorCode) ? String.Empty : String.Format(ErrorFormat, this.ErrorMessage));
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ActionEventDto other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
