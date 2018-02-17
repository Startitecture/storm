// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The FormRepository interface.
    /// </summary>
    public interface IFormRepository
    {
        /// <summary>
        /// Gets the <see cref="Form"/> with the specified ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the form to get.
        /// </param>
        /// <returns>
        /// A <see cref="Form"/> with a matching <paramref name="id"/>, or null if no match is found.
        /// </returns>
        Form GetForm(int id);

        /// <summary>
        /// Gets the <see cref="Form"/> with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the form to get.
        /// </param>
        /// <returns>
        /// A <see cref="Form"/> with a matching <paramref name="name"/>, or null if no match is found.
        /// </returns>
        Form GetForm(string name);

        /// <summary>
        /// Searches the repository for forms that match the specified full or partial name.
        /// </summary>
        /// <param name="nameSearch">
        /// The name search.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Form"/> items.
        /// </returns>
        IEnumerable<Form> SearchForms(string nameSearch);

        /// <summary>
        /// Saves a new form.
        /// </summary>
        /// <param name="form">
        /// The form to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="Form"/>.
        /// </returns>
        Form Save(Form form);
    }
}