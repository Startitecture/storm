// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPredicate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    /// <summary>
    /// The null predicate.
    /// </summary>
    public class NullPredicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullPredicate"/> class.
        /// </summary>
        /// <param name="returnIfNull">
        /// The return true if null.
        /// </param>
        /// <param name="nullEquivalent">
        /// The null equivalent.
        /// </param>
        public NullPredicate(bool returnIfNull, object nullEquivalent)
        {
            this.ReturnIfNull = returnIfNull;
            this.NullEquivalent = nullEquivalent;
        }

        /// <summary>
        /// Gets a value indicating whether to return true if the candidate value is equal to the <see cref="NullEquivalent"/>.
        /// </summary>
        public bool ReturnIfNull { get; }

        /// <summary>
        /// Gets the null equivalent for the predicate.
        /// </summary>
        public object NullEquivalent { get; }
    }
}