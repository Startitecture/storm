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

        /// <summary>
        /// Qualifies the name of an attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to qualify.
        /// </param>
        /// <returns>
        /// The qualified name of the attribute as a <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// The qualified name is the entity (aliased, if applicable) and the physical attribute name as referenced in the FROM clause.
        /// </remarks>
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
        /// <remarks>
        /// The qualified name is the entity (aliased, if applicable) and the physical attribute name as referenced in the FROM clause.
        /// </remarks>
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
        /// <remarks>
        /// The canonical name is the fully aliased (if applicable) entity and attribute names.
        /// </remarks>
        string GetCanonicalName(EntityAttributeDefinition attribute);

        /// <summary>
        /// Gets the physical name of the <paramref name="location"/>.
        /// </summary>
        /// <param name="location">
        /// The location to get the physical reference for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation for the entity's physical <paramref name="location"/>.
        /// </returns>
        string GetPhysicalName(EntityLocation location);

        /// <summary>
        /// Gets the reference name for the <paramref name="attribute"/>. 
        /// </summary>
        /// <param name="attribute">
        /// The attribute to get the reference for.
        /// </param>
        /// <returns>
        /// The full reference path to the attribute as a <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// The reference name includes the alias of the entity but not the alias of the attribute.
        /// </remarks>
        string GetReferenceName(EntityAttributeDefinition attribute);

        /// <summary>
        /// Adds a prefix to a parameter for use in a prepared statement.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// The prefixed parameter name as a <see cref="string"/>.
        /// </returns>
        string AddParameterPrefix(string parameterName);
    }
}