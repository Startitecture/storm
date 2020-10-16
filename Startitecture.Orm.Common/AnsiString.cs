// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnsiString.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Wrap strings in an instance of this class to force use of DBType.AnsiString.
// </summary>

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Wraps strings to force use of DBType.AnsiString.
    /// </summary>
    public class AnsiString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnsiString"/> class.
        /// </summary>
        /// <param name="value">
        /// The C# string to be converted to ANSI before being passed to the DB.
        /// </param>
        public AnsiString(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value { get; private set; }
    }
}