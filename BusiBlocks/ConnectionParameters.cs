// <copyright file="ConnectionParameters.cs" company="BusiBlocks">
//     BusiBlocks. All rights reserved.
// </copyright>
// <author>BusiBlocks employee</author>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using BusiBlocks.Configuration;
using NHibernate;

namespace BusiBlocks
{
    /// <summary>
    /// ConnectionParameters class is used to mantain the configuration required for NHibernate.
    /// Can read the configuration from a connection string class using the static method ConnectionParameters.Create().
    /// The ConnectionParameters is automatically added to the cache for future invocations.
    /// Use the static ConnectionParameters.AddCachedConfiguration to manually add a configuration to the cache.
    /// Use the OpenSession method to directly open an NHibernate session from the specified configuration.
    /// The class automatically load the the assemblies to use for the mappings from the BusiBlocks configuration section.
    /// The static methods are safe for multithread operations and also the OpenSession instance method.
    /// </summary>
    [Serializable]
    public class ConnectionParameters
    {
        public const string DefaultApp = "Default";

        #region constructor

        /// <summary>
        /// Initializes a new instance of the ConnectionParameters class.
        /// </summary>
        /// <param name="name"></param>
        public ConnectionParameters(string name)
        {
            mName = name;

            MappingAssemblies.Add(Assembly.GetExecutingAssembly());

            LoadAssembliesFromConfiguration();
        }

        #endregion

        #region Properties

        private readonly Collection<Assembly> mMappingAssemblies = new Collection<Assembly>();
        private readonly string mName;

        private string connectionProvider = "NHibernate.Connection.DriverConnectionProvider";
        private string mConnection_ConnectionString;

        private string mConnection_DriverClass;
        private string mDialect;
        private IInterceptor mInterceptor = new BusiBlocksInterceptor();
        private bool mShowSql;

        /// <summary>
        /// Gets the Name of the configuration.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets the NHibernate provider to use. Default is NHibernate.Connection.DriverConnectionProvider
        /// </summary>
        public virtual string ConnectionProvider
        {
            get { return connectionProvider; }
            set { connectionProvider = value; }
        }

        /// <summary>
        /// Gets or sets the NHibernate driver to use. For example NHibernate.Driver.SQLite20Driver
        /// </summary>
        public virtual string ConnectionDriverClass
        {
            get { return mConnection_DriverClass; }
            set { mConnection_DriverClass = value; }
        }

        /// <summary>
        /// Gets or sets the NHibernate ConnectionString to use.
        /// </summary>
        public virtual string ConnectionConnectionString
        {
            get { return mConnection_ConnectionString; }
            set { mConnection_ConnectionString = value; }
        }

        /// <summary>
        /// Gets or sets the NHibernate dialect to use NHibernate.Dialect.SQLiteDialect.
        /// </summary>
        public virtual string Dialect
        {
            get { return mDialect; }
            set { mDialect = value; }
        }

        public virtual bool ShowSql
        {
            get { return mShowSql; }
            set { mShowSql = value; }
        }

        /// <summary>
        /// Gets the List of the assemblies to use in the mapping. 
        /// Already contains the BusiBlocks library to load the default mapping files.
        /// You can add your custom assemblies.
        /// </summary>
        public Collection<Assembly> MappingAssemblies
        {
            get { return mMappingAssemblies; }
        }

        /// <summary>
        /// Gets or sets the Interceptor to use. The default is an instance of BusiBlocksInterceptor.
        /// </summary>
        public IInterceptor Interceptor
        {
            get { return mInterceptor; }
            set { mInterceptor = value; }
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Used to syncronize the factory creation
        /// </summary>
        private readonly object mSyncObj = new object();

        [NonSerialized] private ISessionFactory _factory;

        /// <summary>
        /// Load the assemblies from the BusiBlocks configuration section.
        /// </summary>
        private void LoadAssembliesFromConfiguration()
        {
            // Get the configuration section
            BusiBlocksSection section = BusiBlocksSection.Section;

            if (section == null)
                return;

            foreach (AssemblyMappingElement element in section.Mappings)
            {
                MappingAssemblies.Add(Assembly.Load(element.Assembly));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <exception cref="ConnectionElementNotFoundException"></exception>
        public void ReadConnectionParameters(ConnectionStringSettings settings)
        {
            var connectionString = new DbConnectionStringBuilder();
            connectionString.ConnectionString = settings.ConnectionString;

            if (connectionString.ContainsKey("DriverClass"))
            {
                ConnectionDriverClass = (string) connectionString["DriverClass"];
                connectionString.Remove("DriverClass");
            }
            else
                throw new ConnectionElementNotFoundException("DriverClass");

            if (connectionString.ContainsKey("Dialect"))
            {
                Dialect = (string) connectionString["Dialect"];
                connectionString.Remove("Dialect");
            }
            else
                throw new ConnectionElementNotFoundException("Dialect");

            ConnectionConnectionString = connectionString.ConnectionString;
        }

        /// <summary>
        /// Create an NHibernate configuration class.
        /// </summary>
        /// <param name="options">Configuration options.</param>
        /// <returns>The configuration.</returns>
        public NHibernate.Cfg.Configuration CreateNHibernateConfiguration(ConfigurationFlags options)
        {
            var configuration = new NHibernate.Cfg.Configuration();

            if ((options & ConfigurationFlags.Settings) == ConfigurationFlags.Settings)
            {
                configuration.SetProperty("hibernate.connection.connection_string", ConnectionConnectionString);
                configuration.SetProperty("hibernate.connection.provider", ConnectionProvider);
                configuration.SetProperty("hibernate.connection.driver_class", ConnectionDriverClass);
                configuration.SetProperty("hibernate.show_sql",
                                          ShowSql.ToString(CultureInfo.InvariantCulture).ToUpper(
                                              CultureInfo.InvariantCulture));
                configuration.SetProperty("hibernate.dialect", Dialect);
            }

            if ((options & ConfigurationFlags.Mappings) == ConfigurationFlags.Mappings)
            {
                foreach (Assembly assembly in MappingAssemblies)
                    configuration.AddAssembly(assembly);
            }

            if ((options & ConfigurationFlags.Interceptor) == ConfigurationFlags.Interceptor)
            {
                configuration.SetInterceptor(Interceptor);
            }

            return configuration;
        }

        /// <summary>
        /// This method use a lock to syncronize the factory creation.
        /// </summary>
        private void CheckFactory()
        {
            if (_factory == null)
            {
                lock (mSyncObj)
                {
                    if (_factory == null)
                    {
                        NHibernate.Cfg.Configuration configuration =
                            CreateNHibernateConfiguration(ConfigurationFlags.Default);

                        _factory = configuration.BuildSessionFactory();
                    }
                }
            }
        }

        /// <summary>
        /// Returns an active NHibernate session.
        /// Usually you don't need to call this method directly, 
        /// I suggest to use the TransactionScope class.
        /// </summary>
        /// <returns></returns>
        public ISession OpenSession()
        {
            CheckFactory();

            return _factory.OpenSession();
        }

        #endregion

        #region Static methods

        private static readonly Dictionary<string, ConnectionParameters> mList =
            new Dictionary<string, ConnectionParameters>();

        /// <summary>
        /// Add the specified configuration to the list of saved configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="throwErrorIfExists">True to throw an exception if there is already a connection with the same name, otherwise if a connection already exist this method simply return the previous connection.</param>
        /// <returns>Return the connection just added or the previous connection is there is already a connection with the same name.</returns>
        public static ConnectionParameters AddCachedConfiguration(ConnectionParameters configuration,
                                                                  bool throwErrorIfExists)
        {
            lock (mList)
            {
                ConnectionParameters savedConfig;
                if (mList.TryGetValue(configuration.Name, out savedConfig))
                {
                    if (throwErrorIfExists)
                        throw new ConfigurationAlreadyExistsException(configuration.Name);
                    else
                        return savedConfig;
                }
                else
                {
                    mList.Add(configuration.Name, configuration);
                    return configuration;
                }
            }
        }

        /// <summary>
        /// Check if a there is already a configuration with the specified name. Return null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConnectionParameters Find(string name)
        {
            lock (mList)
            {
                ConnectionParameters savedConfig;
                if (mList.TryGetValue(name, out savedConfig))
                    return savedConfig;
                else
                    return null;
            }
        }

        /// <summary>
        /// This method first look if there is already a saved configuration in the cached list. 
        /// If not exist try to check if there is a connection string in the config file with the same name.
        /// If not exist throw an exception.
        /// </summary>
        /// <param name="name">The name of the connection string to use or a custom connection name. 
        /// To use custom connection use the AddCachedConfiguration method to configure it, otherwise a new configuration is automatically created.</param>
        public static ConnectionParameters Create(string name)
        {
            lock (mList)
            {
                ConnectionParameters savedConfig;
                if (mList.TryGetValue(name, out savedConfig))
                    return savedConfig;
            }

            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString != null &&
                connectionString.ConnectionString != null &&
                connectionString.ConnectionString.Length > 0)
            {
                var configuration = new ConnectionParameters(name);
                configuration.ReadConnectionParameters(connectionString);

                return AddCachedConfiguration(configuration, false);
            }

            throw new ConfigurationNotFoundException(name);
        }

        #endregion
    }
}