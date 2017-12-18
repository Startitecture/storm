namespace SAF.LocalMachine
{
    using System;

    using Microsoft.Win32;

    /// <summary>
    /// Represents a registry value.
    /// </summary>
    public class RegistryValue
    {
        /// <summary>
        /// The format for to use for the <see cref="ToString"/> method.
        /// </summary>
        private const string ToStringFormat = "{0} ({1}) = {2}";

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryValue"/> class.
        /// </summary>
        public RegistryValue()
        {
        }

        /// <summary>
        /// Gets or sets the name of the parent <see cref="RegistryKey"/>.
        /// </summary>
        public string ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the name of this <see cref="RegistryValue"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data represented by this <see cref="RegistryValue"/>.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the type of data stored by this <see cref="RegistryValue"/>.
        /// </summary>
        public RegistryValueKind ValueKind { get; set; }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="RegistryValue"/>.
        /// </summary>
        /// <returns>The name, <see cref="RegistryValueKind"/>, and data in a string.</returns>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.Name, this.ValueKind, this.Data);
        }
    }
}
