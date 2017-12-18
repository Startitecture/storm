// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParametersHelper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A helper class for processing parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using SAF.StringResources;

    /// <summary>
    /// A helper class for processing parameters.
    /// </summary>
    internal static class ParametersHelper
    {
        // Helper to handle named parameters from object properties
        #region Static Fields

        /// <summary>
        /// The parameters Regex.
        /// </summary>
        private static readonly Regex ParamsRegex = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Processes parameters in the SQL statement.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement.
        /// </param>
        /// <param name="sourceParams">
        /// The source parameters.
        /// </param>
        /// <param name="destParams">
        /// The destination parameters.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> with the parameters replaced.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The same number of parameters was not specified as the number of values.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A parameter was passed with a name that does not exist as a property in one of the arguments.
        /// </exception>
        public static string ProcessParams(string sql, object[] sourceParams, List<object> destParams)
        {
            return ParamsRegex.Replace(sql, match => FormatMatch(sql, sourceParams, destParams, match));
        }

        /// <summary>
        /// Formats a regex match.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to process.
        /// </param>
        /// <param name="sourceParams">
        /// The source parameters.
        /// </param>
        /// <param name="destParams">
        /// The destination parameters.
        /// </param>
        /// <param name="match">
        /// The match to process.
        /// </param>
        /// <returns>
        /// A formatted parameter invocation as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The same number of parameters was not specified as the number of values.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A parameter was passed with a name that does not exist as a property in one of the arguments.
        /// </exception>
        private static string FormatMatch(string sql, IList<object> sourceParams, ICollection<object> destParams, Capture match)
        {
            string param = match.Value.Substring(1);
            object parameterValue;
            int paramIndex;

            if (Int32.TryParse(param, out paramIndex))
            {
                // Numbered parameter
                if (paramIndex < 0 || paramIndex >= sourceParams.Count)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(ValidationMessages.SqlParameterHasNoMatchingValue, paramIndex, sourceParams.Count, sql));
                }

                parameterValue = sourceParams[paramIndex];
            }
            else
            {
                // Look for a property on one of the arguments with this name
                bool found = false;
                parameterValue = null;

                foreach (object value in sourceParams)
                {
                    PropertyInfo propertyInfo = value.GetType().GetProperty(param);

                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    parameterValue = propertyInfo.GetValue(value, null);
                    found = true;
                    break;
                }

                if (found == false)
                {
                    throw new ArgumentException(String.Format(ValidationMessages.SqlParameterDoesNotMatchArguments, param, sql));
                }
            }

            // Expand collections to parameter lists
            if ((parameterValue as IEnumerable) != null && (parameterValue as string) == null && (parameterValue as byte[]) == null)
            {
                var builder = new StringBuilder();

                foreach (object i in parameterValue as IEnumerable)
                {
                    builder.Append((builder.Length == 0 ? "@" : ",@") + destParams.Count);
                    destParams.Add(i);
                }

                return builder.ToString();
            }

            destParams.Add(parameterValue);
            return String.Format("@{0}", destParams.Count - 1);
        }

        #endregion
    }
}