// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BadMappingDto.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Startitecture.Core;

    /// <summary>
    /// Bad mapping DTO.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BadMappingDto
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<BadMappingDto, object>[] ComparisonProperties =
            {
                dto => dto.Name,
                dto => dto.Description?.ToUpperInvariant(),
                dto => dto.DateValue
            };

        /// <summary>
        /// Gets or sets the fake entity id.
        /// </summary>
        public int? FakeEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        public string Description
        {
            get
            {
                return "Bad Mapping";
            }

            set
            {
                throw new InvalidOperationException("Bad Mapping!");
            }
        }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
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
        public static bool operator ==(BadMappingDto left, BadMappingDto right)
        {
            return EqualityComparer<BadMappingDto>.Default.Equals(left, right);
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
        public static bool operator !=(BadMappingDto left, BadMappingDto right)
        {
            return !EqualityComparer<BadMappingDto>.Default.Equals(left, right);
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
        public bool Equals(BadMappingDto other)
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
            return $"DTO [{this.FakeEntityId}] {this.Name}: {this.Description}";
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
    }
}
