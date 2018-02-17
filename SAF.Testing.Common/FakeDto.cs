// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDto.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    using Startitecture.Core;
    using Startitecture.Orm.Common;

    /// <summary>
    /// The fake dto.
    /// </summary>
    [DataContract]
    [ExcludeFromCodeCoverage]
    public class FakeDto : IEquatable<FakeDto>, ITransactionContext
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeDto, object>[] ComparisonProperties =
            {
                dto => dto.Name, 
                dto => dto.Description?.ToUpperInvariant(), 
                dto => dto.DateValue
            };

        /// <summary>
        /// Gets or sets the fake entity id.
        /// </summary>
        [DataMember]
        public int? FakeEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        [DataMember]
        public DateTime DateValue { get; set; }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(FakeDto left, FakeDto right)
        {
            return EqualityComparer<FakeDto>.Default.Equals(left, right);
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(FakeDto left, FakeDto right)
        {
            return !EqualityComparer<FakeDto>.Default.Equals(left, right);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(FakeDto other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("DTO [{0}] {1}: {2}", this.FakeEntityId, this.Name, this.Description);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Gets the transaction provider.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// The set transaction provider.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }
    }
}
