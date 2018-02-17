// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OracleProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An Oracle data provider for early versions of Oracle drivers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Data.Common;
    using System.Reflection;

    using SAF.StringResources;

    /// <summary>
    /// An Oracle data provider for early versions of Oracle drivers.
    /// </summary>
    public class OracleProvider : DbProviderFactory
    {
        /* 
        Thanks to Adam Schroder (@schotime) for this.

        This extra file provides an implementation of DbProviderFactory for early versions of the Oracle
        drivers that don't include include it.  For later versions of Oracle, the standard OracleProviderFactory
        class should work fine

        Uses reflection to load Oracle.DataAccess assembly and in-turn create connections and commands

        Currently untested.

        Usage:   

                new PetaPoco.Database("<connstring>", new PetaPoco.OracleProvider())

        Or in your app/web config (be sure to change ASSEMBLYNAME to the name of your 
        assembly containing OracleProvider.cs)

            <connectionStrings>
                <add
                    name="oracle"
                    connectionString="WHATEVER"
                    providerName="Oracle"
                    />
            </connectionStrings>

            <system.data>
                <DbProviderFactories>
                    <add name="PetaPoco Oracle Provider" invariant="Oracle" description="PetaPoco Oracle Provider" 
                                    type="PetaPoco.OracleProvider, ASSEMBLYNAME" />
                </DbProviderFactories>
            </system.data>

        */

        #region Constants

        /// <summary>
        /// The assembly name.
        /// </summary>
        private const string AssemblyName = "Oracle.DataAccess";

        /// <summary>
        /// The command type name.
        /// </summary>
        private const string CommandTypeName = "Oracle.DataAccess.Client.OracleCommand";

        /// <summary>
        /// The connection type name.
        /// </summary>
        private const string ConnectionTypeName = "Oracle.DataAccess.Client.OracleConnection";

        #endregion

        // Required for DbProviderFactories.GetFactory() to work.
        #region Static Fields

        /// <summary>
        /// The provider instance.
        /// </summary>
        private static readonly OracleProvider ProviderInstance = new OracleProvider();

        /// <summary>
        /// The command type.
        /// </summary>
        private static Type commandType;

        /// <summary>
        /// The connection type.
        /// </summary>
        private static Type connectionType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProvider"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The connection type could not be found.
        /// </exception>
        public OracleProvider()
        {
            connectionType = TypeFromAssembly(ConnectionTypeName, AssemblyName);
            commandType = TypeFromAssembly(CommandTypeName, AssemblyName);

            if (connectionType == null)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.ConnectionTypeNotFound, ConnectionTypeName));
            }
        }

        /// <summary>
        /// Gets the default instance of the provider.
        /// </summary>
        public static OracleProvider Instance
        {
            get
            {
                return ProviderInstance;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates a command for an Oracle database.
        /// </summary>
        /// <returns>
        /// A <see cref="DbCommand"/> compatible with an Oracle database.
        /// </returns>
        public override DbCommand CreateCommand()
        {
            var command = (DbCommand)Activator.CreateInstance(commandType);
            PropertyInfo oracleCommandBindByName = commandType.GetProperty("BindByName");
            oracleCommandBindByName.SetValue(command, true, null);
            return command;
        }

        /// <summary>
        /// Creates a connection for an Oracle database.
        /// </summary>
        /// <returns>
        /// A <see cref="DbConnection"/> for an Oracle database.
        /// </returns>
        public override DbConnection CreateConnection()
        {
            return (DbConnection)Activator.CreateInstance(connectionType);
        }

        #endregion

        /// <summary>
        /// Gets a type from the specified type name or assembly.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> specified in the <paramref name="typeName"/> parameter.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// The type specified in <paramref name="typeName"/> could not be loaded.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The assembly specified in <paramref name="assemblyName"/> could not be found.
        /// </exception>
        private static Type TypeFromAssembly(string typeName, string assemblyName)
        {
            // Try to get the type from an already loaded assembly
            Type type = Type.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            if (assemblyName == null)
            {
                // No assembly was specified for the type, so just fail
                string message = String.Format(ErrorMessages.TypeCouldNotBeLoaded, typeName);
                throw new TypeLoadException(message);
            }

            Assembly assembly = Assembly.Load(assemblyName);

            if (assembly == null)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.AssemblyNotFound, assemblyName));
            }

            return assembly.GetType(typeName);
        }
    }
}