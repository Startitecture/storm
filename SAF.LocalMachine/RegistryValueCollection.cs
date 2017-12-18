namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using SAF.Core;

    /// <summary>
    /// Contains a collection of <see cref="RegistryValue"/>s.
    /// </summary>
    [Serializable, XmlInclude(typeof(RegistryValue))]
    public class RegistryValueCollection : SerializableCollection<RegistryValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryValueCollection"/> class.
        /// </summary>
        public RegistryValueCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryValueCollection"/> class.
        /// </summary>
        /// <param name="values">The collection of <see cref="RegistryValue"/>s to </param>
        public RegistryValueCollection(IEnumerable<RegistryValue> values)
            : base(values)
        {
        }
    }
}
