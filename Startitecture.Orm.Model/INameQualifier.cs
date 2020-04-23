// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INameQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using JetBrains.Annotations;

    /// <summary>
    /// The NameQualifier interface.
    /// </summary>
    public interface INameQualifier
    {
        /// <summary>
        /// Qualifies the name of an attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to qualify.
        /// </param>
        /// <returns>
        /// The qualified name of the attribute as a <see cref="string"/>.
        /// </returns>
        string Qualify(EntityAttributeDefinition attribute);

        /// <summary>
        /// Qualifies the name of an attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to qualify.
        /// </param>
        /// <param name="entityLocation">
        /// The entity location.
        /// </param>
        /// <returns>
        /// The qualified name of the attribute as a <see cref="string"/>.
        /// </returns>
        string Qualify(EntityAttributeDefinition attribute, EntityLocation entityLocation);

        /// <summary>
        /// Gets the canonical name for the <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation of the <paramref name="attribute"/>.
        /// </returns>
        string GetCanonicalName(EntityAttributeDefinition attribute);

        /// <summary>
        /// Gets the canonical name for the <paramref name="location"/>.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation for the <paramref name="location"/>.
        /// </returns>
        string GetCanonicalName(EntityLocation location);

        /// <summary>
        /// Escapes an identifier.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to escape.
        /// </param>
        /// <returns>
        /// The escaped identifier as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is null or whitespace.
        /// </exception>
        string Escape([NotNull] string identifier);
    }
}