// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredCommandText.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
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
        public string Text { get; private set; }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParameterName { get; private set; }
    }
}