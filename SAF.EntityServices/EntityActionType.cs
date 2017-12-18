// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityActionType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Enumerates the actions handled by an <see cref="T:SAF.EntityServices.EntityProxy`1" />.
    /// </summary>
    public struct EntityActionType : IEquatable<EntityActionType>
    {
        #region Constants

        /// <summary>
        /// The remove all value.
        /// </summary>
        internal const int RemoveAllValue = 7;

        /// <summary>
        /// The remove any value.
        /// </summary>
        internal const int RemoveAnyValue = 8;

        /// <summary>
        /// The remove one value.
        /// </summary>
        internal const int RemoveOneValue = 9;

        /// <summary>
        /// The save value.
        /// </summary>
        internal const int SaveValue = 4;

        /// <summary>
        /// The select all value.
        /// </summary>
        internal const int SelectAllValue = 1;

        /// <summary>
        /// The select any value.
        /// </summary>
        internal const int SelectAnyValue = 2;

        /// <summary>
        /// The select one value.
        /// </summary>
        internal const int SelectOneValue = 3;

        /// <summary>
        /// The update all value.
        /// </summary>
        internal const int UpdateAllValue = 5;

        /// <summary>
        /// The update any value.
        /// </summary>
        internal const int UpdateAnyValue = 6;

        /// <summary>
        /// The remove all action name.
        /// </summary>
        private const string RemoveAllActionName = "Remove All";

        /// <summary>
        /// The remove selection action name.
        /// </summary>
        private const string RemoveSelectionActionName = "Remove Selection";

        /// <summary>
        /// The remove first matching action name.
        /// </summary>
        private const string RemoveFirstMatchingActionName = "Remove First Matching";

        /// <summary>
        /// The save action name.
        /// </summary>
        private const string SaveActionName = "Save";

        /// <summary>
        /// The select all action name.
        /// </summary>
        private const string SelectAllActionName = "Select All";

        /// <summary>
        /// The select match action name.
        /// </summary>
        private const string SelectMatchActionName = "Select Match";

        /// <summary>
        /// The select first matching action name.
        /// </summary>
        private const string SelectFirstMatchingActionName = "Select First Matching";

        /// <summary>
        /// The update all action name.
        /// </summary>
        private const string UpdateAllActionName = "Update All";

        /// <summary>
        /// The update selection action name.
        /// </summary>
        private const string UpdateSelectionActionName = "Update Selection";

        #endregion

        #region Static Fields

        /// <summary>
        /// The remove all value.
        /// </summary>
        private static readonly EntityActionType RemoveAllType = new EntityActionType(
            RemoveAllValue, RemoveAllActionName, ActionMessages.RemovingAllItemsFromRepository);

        /// <summary>
        /// The remove any value.
        /// </summary>
        private static readonly EntityActionType RemoveAnyType = new EntityActionType(
            RemoveAnyValue, RemoveSelectionActionName, ActionMessages.RemovingItemsFromRepository);

        /// <summary>
        /// The remove matching value.
        /// </summary>
        private static readonly EntityActionType RemoveOneType = new EntityActionType(
            RemoveOneValue, RemoveFirstMatchingActionName, ActionMessages.RemovingFirstItemFromRepository);

        /// <summary>
        /// The save value.
        /// </summary>
        private static readonly EntityActionType SaveType = new EntityActionType(
            SaveValue, SaveActionName, ActionMessages.SavingItemInRepository);

        /// <summary>
        /// The select all value.
        /// </summary>
        private static readonly EntityActionType SelectAllType = new EntityActionType(
            SelectAllValue, SelectAllActionName, ActionMessages.SelectAllItemsFromRepository);

        /// <summary>
        /// The select any value.
        /// </summary>
        private static readonly EntityActionType SelectAnyType = new EntityActionType(
            SelectAnyValue, SelectMatchActionName, ActionMessages.SelectMatchingItemsInRepository);

        /// <summary>
        /// The select matching value.
        /// </summary>
        private static readonly EntityActionType SelectOneType = new EntityActionType(
            SelectOneValue, SelectFirstMatchingActionName, ActionMessages.SelectFirstMatchingItemFromRepository);

        /// <summary>
        /// The update all value.
        /// </summary>
        private static readonly EntityActionType UpdateAllType = new EntityActionType(
            UpdateAllValue, UpdateAllActionName, ActionMessages.UpdateAllItemsInRepository);

        /// <summary>
        /// The update any value.
        /// </summary>
        private static readonly EntityActionType UpdateAnyType = new EntityActionType(
            UpdateAnyValue, UpdateSelectionActionName, ActionMessages.UpdateMatchingItemsInRepository);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityActionType"/> struct.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="actionType">
        /// The action type.
        /// </param>
        /// <param name="descriptionFormat">
        /// The description format.
        /// </param>
        private EntityActionType(int value, string actionType, string descriptionFormat)
            : this()
        {
            this.Value = value;
            this.ActionType = actionType;
            this.DescriptionFormat = descriptionFormat;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the action type for removing all matching entities.
        /// </summary>
        public static EntityActionType RemoveAll
        {
            get
            {
                return RemoveAllType;
            }
        }

        /// <summary>
        /// Gets the action type for removing any matching entities.
        /// </summary>
        public static EntityActionType RemoveAny
        {
            get
            {
                return RemoveAnyType;
            }
        }

        /// <summary>
        /// Gets the action type for removing a single matching entity.
        /// </summary>
        public static EntityActionType RemoveOne
        {
            get
            {
                return RemoveOneType;
            }
        }

        /// <summary>
        /// Gets the action type for saving an entity.
        /// </summary>
        public static EntityActionType Save
        {
            get
            {
                return SaveType;
            }
        }

        /// <summary>
        /// Gets the action type for selecting all matching entities.
        /// </summary>
        public static EntityActionType SelectAll
        {
            get
            {
                return SelectAllType;
            }
        }

        /// <summary>
        /// Gets the action type for selecting any matching entity.
        /// </summary>
        public static EntityActionType SelectAny
        {
            get
            {
                return SelectAnyType;
            }
        }

        /// <summary>
        /// Gets the action type for selecting a single matching entity.
        /// </summary>
        public static EntityActionType SelectOne
        {
            get
            {
                return SelectOneType;
            }
        }

        /// <summary>
        /// Gets the action type for updating all matching entities.
        /// </summary>
        public static EntityActionType UpdateAll
        {
            get
            {
                return UpdateAllType;
            }
        }

        /// <summary>
        /// Gets the action type for updating any matching entities.
        /// </summary>
        public static EntityActionType UpdateAny
        {
            get
            {
                return UpdateAnyType;
            }
        }

        /// <summary>
        /// Gets the action type.
        /// </summary>
        public string ActionType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the description format.
        /// </summary>
        public string DescriptionFormat { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether two values of the same type are equal.
        /// </summary>
        /// <param name="left">
        /// The first value.
        /// </param>
        /// <param name="right">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the two values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EntityActionType left, EntityActionType right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two values of the same type are not equal.
        /// </summary>
        /// <param name="left">
        /// The first value.
        /// </param>
        /// <param name="right">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the two values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(EntityActionType left, EntityActionType right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return obj is EntityActionType && this.Equals((EntityActionType)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EntityActionType other)
        {
            return Evaluate.CollectionEquals(
                new object[] { this.ActionType, this.DescriptionFormat, this.Value },
                new object[] { this.ActionType, this.DescriptionFormat, this.Value });
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.ActionType, this.DescriptionFormat, this.Value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="EntityActionType" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="EntityActionType" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.ActionType;
        }

        #endregion
    }
}