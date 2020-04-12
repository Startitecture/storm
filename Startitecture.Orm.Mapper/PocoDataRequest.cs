// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PocoDataRequest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Defines a POCO data request for a POCO factory. The hash code is unique to the entity and columns by order.
    /// </summary>
    public class PocoDataRequest : IEquatable<PocoDataRequest>
    {
        /// <summary>
        /// The hash objects.
        /// </summary>
        private readonly List<object> hashObjects;

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly int hashCode;

        /// <summary>
        /// The to string text.
        /// </summary>
        private readonly string toStringText;

        /// <summary>
        /// Initializes a new instance of the <see cref="PocoDataRequest"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data record.
        /// </param>
        /// <param name="entityDefinition">
        /// The definition of the type represented by the <paramref name="dataReader"/>.
        /// </param>
        public PocoDataRequest([NotNull] IDataReader dataReader, [NotNull] IEntityDefinition entityDefinition)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException(nameof(dataReader));
            }

            if (entityDefinition == null)
            {
                throw new ArgumentNullException(nameof(entityDefinition));
            }

            this.DataReader = dataReader;
            this.FieldCount = dataReader.FieldCount;
            this.EntityDefinition = entityDefinition ?? throw new ArgumentNullException(nameof(entityDefinition));

            // We need a unique hash for each query formation.
            this.hashObjects = this.GetHashObjects();

            this.hashCode = Evaluate.GenerateHashCode(this.hashObjects);
            this.toStringText = $"{this.EntityDefinition.QualifiedName}:{string.Join(",", this.hashObjects.Skip(1))}";
        }

        /// <summary>
        /// Gets the data record.
        /// </summary>
        public IDataReader DataReader { get; }

        /// <summary>
        /// Gets the entity definition.
        /// </summary>
        public IEntityDefinition EntityDefinition { get; }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        public int FieldCount { get; }

        /// <summary>
        /// Gets or sets the first column to read.
        /// </summary>
        public int FirstColumn { get; set; }

        #region Equality and Comparison Methods and Operators

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(PocoDataRequest valueA, PocoDataRequest valueB)
        {
            return EqualityComparer<PocoDataRequest>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(PocoDataRequest valueA, PocoDataRequest valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.toStringText;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object. 
        /// </param>
        /// <filterpriority>2</filterpriority>
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(PocoDataRequest other)
        {
            return Evaluate.Equals(this.hashObjects, other?.hashObjects ?? new List<object>());
        }

        #endregion

        /// <summary>
        /// Gets hash objects for comparison for this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="object"/> items to use for generating a hash.
        /// </returns>
        private List<object> GetHashObjects()
        {
            var objects = new List<object>
                              {
                                  this.EntityDefinition.QualifiedName
                              };

            for (int i = 0; i < this.DataReader.FieldCount; i++)
            {
                objects.Add(this.DataReader.GetName(i));
            }

            return objects;
        }
    }
}