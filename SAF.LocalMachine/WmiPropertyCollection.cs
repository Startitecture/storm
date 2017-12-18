namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Xml.Serialization;

    using SAF.Core;

    /// <summary>
    /// Contains a collection of <see cref="PropertyData"/>.
    /// </summary>
    [Serializable, XmlInclude(typeof(PropertyData))]
    public class WmiPropertyCollection : SerializableCollection<PropertyData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiPropertyCollection"/> class.
        /// </summary>
        public WmiPropertyCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiPropertyCollection"/> class with the specified drive 
        /// mappings.
        /// </summary>
        /// <param name="propertyData">The <see cref="PropertyData"/> items to initialize the collection with.</param>
        public WmiPropertyCollection(IEnumerable<PropertyData> propertyData)
            : base(propertyData)
        {
        }

        /// <summary>
        /// Gets the first <see cref="PropertyData"/> in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="PropertyData"/> to retrieve.</param>
        /// <returns>
        /// The <see cref="PropertyData"/> with the specified name, or null if no element matches <paramref name="name"/>
        /// </returns>
        public PropertyData this[string name]
        {
            get { return this.ValueList.FirstOrDefault(x => x.Name == name); }
        }
    }
}
