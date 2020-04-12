// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredCommandText.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    /// <summary>
    /// The structured command text.
    /// </summary>
    public class StructuredCommandText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredCommandText"/> class.
        /// </summary>
        /// <param name="text">
        /// The command text.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        public StructuredCommandText(string text, string parameterName)
        {
            this.Text = text;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParameterName { get; }
    }
}