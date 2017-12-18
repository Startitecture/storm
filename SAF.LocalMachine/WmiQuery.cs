// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiQuery.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   Contains instructions for performing a WMI query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.LocalMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Management;

    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// Contains instructions for performing a WMI query.
    /// </summary>
    public class WmiQuery : DataSource
    {
        /// <summary>
        /// Default host prefix for the WMI path ("\\")
        /// </summary>
        public const string WmiHostRoot = "\\\\";

        /// <summary>
        /// Default initial WMI path
        /// </summary>
        public const string WmiPathRoot = "\\root\\cimv2";

        /// <summary>
        /// Queries the Win32_ComputerSystem class.
        /// </summary>
        public static readonly WmiQuery ComputerSystemQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_ComputerSystem",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// Queries the Win32_Processor class.
        /// </summary>
        public static readonly WmiQuery ProcessorQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_Processor",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// Queries the Win32_ComputerSystemProduct class.
        /// </summary>
        public static readonly WmiQuery ComputerSystemProductQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_ComputerSystemProduct",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// Queries the Win32_SystemEnclosure class.
        /// </summary>
        public static readonly WmiQuery SystemEnclosureQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_SystemEnclosure",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// Queries the Win32_BIOS class.
        /// </summary>
        public static readonly WmiQuery BiosQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_BIOS",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// Queries the Win32_Product class (software installations).
        /// </summary>
        public static readonly WmiQuery SoftwareProductQuery =
            new WmiQuery
            {
                ObjectClass = "Win32_Product",
                UniqueProperty = "Caption"
            };

        /// <summary>
        /// The format of the location string (host prefix, fully qualified hostname, path root, class).
        /// </summary>
        private const string LocationFormat = "{0}{1}{2}{3}";

        /// <summary>
        /// WMI query path format string where {0} is the host root and {1} is the path root.
        /// </summary>
        private const string LocalMachinePathFormat = "{0}.{1}";

        /// <summary>
        /// A SELECT statement format for selecting all objects from a specific class, where {0} is the class name.
        /// </summary>
        private const string ClassSelectFormat = "SELECT * FROM {0}";

        /// <summary>
        /// Gets the WMI time format for parsing into a DateTimeOffset.
        /// </summary>
        private const string FixedWmiDateTimeFormat = "yyyyMMddHHmmss.ffffffzzz";

        /// <summary>
        /// The management scope for the local host.
        /// </summary>
        private static readonly ManagementScope ManagementScope = 
            new ManagementScope(
                new ManagementPath(String.Format(LocalMachinePathFormat, WmiHostRoot, WmiPathRoot)),
                new ConnectionOptions { Impersonation = ImpersonationLevel.Impersonate });

        /// <summary>
        /// A function that determines whether to include a property.
        /// </summary>
        private static readonly Func<WmiQuery, PropertyData, bool> PropertyFilter = (q, p) => 
            q.IncludedProperties.Contains(p.Name) || 
            q.RequiredProperties.Contains(p.Name) ||
            q.UniqueProperty == p.Name ||
            q.IncludedProperties.Count == 0;

        /// <summary>
        /// The domain of the current user's security context.
        /// </summary>
        private readonly string qualifiedHostName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiQuery"/> class.
        /// </summary>
        public WmiQuery()
            : base("WMI Query")
        {
            string domainName = NetworkEnvironment.DomainName;

            this.qualifiedHostName = 
                String.Format(
                    "{0}{1}{2}", 
                    Environment.MachineName, 
                    String.IsNullOrEmpty(domainName) ? String.Empty : ".", 
                    domainName);

            this.RequiredProperties = new StringCollection();
            this.IncludedProperties = new StringCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiQuery"/> class.
        /// </summary>
        /// <param name="requiredProperties">A list of properties that must be set for an value to be included in 
        /// the result.</param>
        /// <param name="includedProperties">An inclusive list of properties to include in the result.</param>
        public WmiQuery(
            IEnumerable<string> requiredProperties,
            IEnumerable<string> includedProperties)
            : this()
        {
            this.RequiredProperties.AddRange(requiredProperties.ToArray());
            this.IncludedProperties.AddRange(includedProperties.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiQuery"/> class.
        /// </summary>
        /// <param name="objectClass">The object class to search.</param>
        /// <param name="requiredUniqueProperty">The unique identifier required to include an value in the result.</param>
        /// <param name="requiredProperties">A list of properties that must be set for an value to be included in 
        /// the result.</param>
        /// <param name="includedProperties">An inclusive list of properties to include in the result.</param>
        public WmiQuery(
            string objectClass, 
            string requiredUniqueProperty,
            IEnumerable<string> requiredProperties, 
            IEnumerable<string> includedProperties)
            : this(requiredProperties, includedProperties)
        {
            this.ObjectClass = objectClass;
            this.UniqueProperty = requiredUniqueProperty;
        }

        /// <summary>
        /// Gets the <see cref="ManagementScope"/> for the local machine.
        /// </summary>
        public static ManagementScope LocalManagementScope
        {
            get { return ManagementScope; }
        }

        /// <summary>
        /// Gets the last modified date of the file.
        /// </summary>
        public override DateTimeOffset LastModified
        {
            get { return DateTimeOffset.Now; }
        }

        /// <summary>
        /// Gets the location of the WMI query.
        /// </summary>
        public override string Location
        {
            get 
            { 
                return String.Format(
                    LocationFormat, WmiHostRoot, this.qualifiedHostName, WmiPathRoot, this.ObjectClass); 
            }
        }

        /// <summary>
        /// Gets or sets the object class to search.
        /// </summary>
        public string ObjectClass { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier required to include an value in the result.
        /// </summary>
        public string UniqueProperty { get; set; }

        /// <summary>
        /// Gets the list of properties that must be set for an value to be included in the result.
        /// </summary>
        public StringCollection RequiredProperties { get; private set; }

        /// <summary>
        /// Gets the inclusive list of properties to include in the result. If this is empty, all properties are retrieved.
        /// </summary>
        public StringCollection IncludedProperties { get; private set; }

        /// <summary>
        /// Gets a <see cref="String"/> representation of a <see cref="PropertyData"/> value.
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="String"/>.</param>
        /// <returns>
        /// The string representation of the <see cref="PropertyData"/> value, or an empty string if the value is null.
        /// </returns>
        public static string GetValue(PropertyData value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Value == null)
            {
                return String.Empty;
            }

            if (value.IsArray)
            {
                switch (value.Type)
                {
                    case CimType.Boolean:
                        return String.Join(",", ((bool[])value.Value).ToStringCollection());

                    case CimType.Char16:
                        return String.Join(",", ((char[])value.Value).ToStringCollection());

                    case CimType.DateTime:
                        return String.Join(",", ((DateTime[])value.Value).ToStringCollection());

                    case CimType.None:
                    case CimType.Object:
                        return String.Join(",", ((object[])value.Value).ToStringCollection());

                    case CimType.Real32:
                        return String.Join(",", ((float[])value.Value).ToStringCollection());

                    case CimType.Real64:
                        return String.Join(",", ((double[])value.Value).ToStringCollection());

                    case CimType.Reference:
                    case CimType.SInt16:
                        return String.Join(",", ((short[])value.Value).ToStringCollection());

                    case CimType.SInt32:
                        return String.Join(",", ((int[])value.Value).ToStringCollection());

                    case CimType.SInt64:
                        return String.Join(",", ((long[])value.Value).ToStringCollection());

                    case CimType.SInt8:
                        return String.Join(",", ((sbyte[])value.Value).ToStringCollection());

                    case CimType.String:
                        return String.Join(",", ((string[])value.Value).ToStringCollection());

                    case CimType.UInt16:
                        return String.Join(",", ((ushort[])value.Value).ToStringCollection());

                    case CimType.UInt32:
                        return String.Join(",", ((uint[])value.Value).ToStringCollection());

                    case CimType.UInt64:
                        return String.Join(",", ((ulong[])value.Value).ToStringCollection());

                    case CimType.UInt8:
                        return String.Join(",", ((byte[])value.Value).ToStringCollection());
                }
            }
            
            return Convert.ToString(value.Value).Trim();
        }

        /// <summary>
        /// Retrieves a string value from a <see cref="PropertyData"/> value at the specified index.
        /// </summary>
        /// <param name="value">The <see cref="PropertyData"/> value containing the array.</param>
        /// <param name="index">The index of the array to retrieve.</param>
        /// <returns>A <see cref="String"/> representation at the specified index of the <see cref="PropertyData"/> 
        /// value array.</returns>
        /// <exception cref="InvalidOperationException">
        /// The type of <see cref="CimType"/> is not supported.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is outside the bounds of the array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than 0 and the value type is not an array type.
        /// </exception>
        public static string GetValue(PropertyData value, int index)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Value == null)
            {
                return String.Empty;
            }

            if (value.IsArray)
            {
                switch (value.Type)
                {
                    case CimType.Boolean:
                        return Convert.ToString(((bool[])value.Value)[index]);

                    case CimType.Char16:
                        return Convert.ToString(((char[])value.Value)[index]);

                    case CimType.DateTime:
                        return Convert.ToString(((DateTime[])value.Value)[index]);

                    case CimType.None:
                    case CimType.Object:
                        return Convert.ToString(((object[])value.Value)[index]);

                    case CimType.Real32:
                        return Convert.ToString(((float[])value.Value)[index]);

                    case CimType.Real64:
                        return Convert.ToString(((double[])value.Value)[index]);

                    case CimType.Reference:
                    case CimType.SInt16:
                        return Convert.ToString(((short[])value.Value)[index]);

                    case CimType.SInt32:
                        return Convert.ToString(((int[])value.Value)[index]);

                    case CimType.SInt64:
                        return Convert.ToString(((long[])value.Value)[index]);

                    case CimType.SInt8:
                        return Convert.ToString(((sbyte[])value.Value)[index]);

                    case CimType.String:
                        return Convert.ToString(((string[])value.Value)[index]);

                    case CimType.UInt16:
                        return Convert.ToString(((ushort[])value.Value)[index]);

                    case CimType.UInt32:
                        return Convert.ToString(((uint[])value.Value)[index]);

                    case CimType.UInt64:
                        return Convert.ToString(((ulong[])value.Value)[index]);

                    case CimType.UInt8:
                        return Convert.ToString(((byte[])value.Value)[index]);

                    default:
                        throw new InvalidOperationException(String.Format("Type {0} is not supported.", value.Type));
                }
            }

            if (index == 0)
            {
                return Convert.ToString(value.Value);
            }

            throw new ArgumentOutOfRangeException("index");
        }

        /// <summary>
        /// Gets a DateTimeOffset from a WMI time string.
        /// </summary>
        /// <param name="timestamp">The WMI time string to parse.</param>
        /// <returns>A DateTimeOffset representing the WMI time string.</returns>
        public static DateTimeOffset ParseWmiTime(string timestamp)
        {
            if (String.IsNullOrEmpty(timestamp))
            {
                throw new ArgumentNullException("timestamp");
            }

            // Necessary because we must turn the number of minutes (the last 3 digits) into a time span.
            int offsetMinutes = Int32.Parse(timestamp.Substring(timestamp.Length - 3, 3));
            var offset = new TimeSpan(0, offsetMinutes, 0);
            string fixedTimeString =
                String.Format(
                    "{0}{1}:{2:00}", timestamp.Substring(0, timestamp.Length - 3), offset.Hours, offset.Minutes);

            return DateTimeOffset.ParseExact(
                fixedTimeString, FixedWmiDateTimeFormat, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Indicates whether the unique property of a <see cref="WmiQuery"/> is present in the specified
        /// <see cref="ManagementBaseObject"/>.
        /// </summary>
        /// <param name="query">The <see cref="WmiQuery"/> containing the unique property name.</param>
        /// <param name="baseObject">The <see cref="ManagementBaseObject"/> to check.</param>
        /// <returns><c>true</c> if the unique property is present in the base object; otherwise, <c>false</c>.</returns>
        public static bool HasUniqueProperty(WmiQuery query, ManagementBaseObject baseObject)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (baseObject == null)
            {
                throw new ArgumentNullException("baseObject");
            }

            if (!String.IsNullOrEmpty(query.UniqueProperty))
            {
                if (String.IsNullOrEmpty(Convert.ToString(baseObject.Properties[query.UniqueProperty].Value)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Indicates all required properties of a <see cref="WmiQuery"/> are present in the specified
        /// <see cref="ManagementBaseObject"/>.
        /// </summary>
        /// <param name="query">The <see cref="WmiQuery"/> containing the required property collection.</param>
        /// <param name="baseObject">The <see cref="ManagementBaseObject"/> to check.</param>
        /// <returns><c>true</c> if no required properties ares missing from the base object; otherwise, <c>false</c>.</returns>
        public static bool HasRequiredProperties(WmiQuery query, ManagementBaseObject baseObject)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (baseObject == null)
            {
                throw new ArgumentNullException("baseObject");
            }

            return
                query.RequiredProperties.Cast<string>()
                     .All(reqProperty => !String.IsNullOrEmpty(Convert.ToString(baseObject.Properties[reqProperty].Value)));
        }

        /// <summary>
        /// Creates an <see cref="ObjectQuery"/> that will query the specified class name.
        /// </summary>
        /// <param name="className">The name of the class to retrieve.</param>
        /// <returns>An <see cref="ObjectQuery"/> for the specified class name.</returns>
        public static ObjectQuery CreateClassQuery(string className)
        {
            return new ObjectQuery(String.Format(ClassSelectFormat, className));
        }

        /// <summary>
        /// Retrieves properties of the <see cref="ManagementBaseObject"/> based on the specified <see cref="WmiQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="WmiQuery"/> containing the properties to include.</param>
        /// <param name="baseObject">The <see cref="ManagementBaseObject"/> containing the properties to retrieve.</param>
        /// <returns>An enumerable list of <see cref="PropertyData"/>s containing the properties of the <see cref="WmiQuery"/>, 
        /// or null if no properties matched the query.</returns>
        public static WmiPropertyCollection RetrieveProperties(WmiQuery query, ManagementBaseObject baseObject)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (baseObject == null)
            {
                throw new ArgumentNullException("baseObject");
            }

            var propertyArray = new PropertyData[baseObject.Properties.Count];
            baseObject.Properties.CopyTo(propertyArray, 0);

            var propertyList = propertyArray.Where(property => PropertyFilter(query, property)).ToList();

            return new WmiPropertyCollection(propertyList);
        }

        /// <summary>
        /// Queries the local system using the specified <see cref="WmiQuery"/>.
        /// </summary>
        /// <param name="query">The <see cref="WmiQuery"/> to use.</param>
        /// <returns>A collection of <see cref="WmiPropertyCollection"/>s containing the results of the query.</returns>
        public static IEnumerable<WmiPropertyCollection> QueryLocal(WmiQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var collections = new List<WmiPropertyCollection>();

            if (!ManagementScope.IsConnected)
            {
                ManagementScope.Connect();
            }

            ObjectQuery oq = CreateClassQuery(query.ObjectClass);

            using (var searcher = new ManagementObjectSearcher(ManagementScope, oq))
            {
                using (ManagementObjectCollection moc = searcher.Get())
                {
                    collections.AddRange(
                        moc.Cast<ManagementObject>()
                           .Select(mgmtObj => RetrieveProperties(query, mgmtObj))
                           .Where(collection => collection != null));
                }
            }

            return collections;
        }
    }
}
