using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace BusiBlocks.SchemaGenerator
{
    //Note about SQL Server 2005 varbinary(max) and nvarchar(max):
    //
    // SQL Server 2005 dialect defines these properties:
    //	    RegisterColumnType( DbType.String, 1073741823, "NVARCHAR(MAX)" );
    //		RegisterColumnType( DbType.AnsiString, 2147483647, "VARCHAR(MAX)" );
    //		RegisterColumnType( DbType.Binary, 2147483647, "VARBINARY(MAX)" );
    // so to use the new NVARCHAR(MAX) and VARBINARY(MAX) you must simply use as length 1073741823 and 2147483647 respectively
    //

    /// <summary>
    /// Class used to automatically generate the schema of the database.
    /// To generate schema for custom entities use the SetupMappingAttribute to specify the category for each entity.
    /// </summary>
    public class GenericGenerator
    {
        private readonly ConnectionParameters mConfiguration;

        /// <summary>
        /// A dictionary with the category as a string and a list of the types for the category
        /// </summary>
        private readonly Dictionary<string, List<Type>> mMappings = new Dictionary<string, List<Type>>();

        public GenericGenerator(ConnectionParameters config)
        {
            mConfiguration = config;

            LoadAssembliesMappings();
        }

        /// <summary>
        /// Create the specified database schema category.
        /// Remember that these methods delete any existing data on the database and recreate the database structure.
        /// </summary>
        /// <param name="section"></param>
        public void CreateSchemaTable(string schemaCategory)
        {
            NHibernate.Cfg.Configuration hibConfig =
                mConfiguration.CreateNHibernateConfiguration(ConfigurationFlags.Settings);

            Type[] entities = GetEntities(schemaCategory);

            foreach (Type ent in entities)
            {
                if (ent != null)
                    hibConfig.AddClass(ent);
            }

            var ddlExport = new SchemaExport(hibConfig);
            ddlExport.Create(false, true);
        }

        /// <summary>
        /// Get the status of the schema.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public SchemaStatus GetStatus(string schemaCategory)
        {
            try
            {
                Type[] entities = GetEntities(schemaCategory);

                //TODO Check if there is a way to see if a table exist without catching exception
                using (var transaction = new TransactionScope(mConfiguration))
                {
                    foreach (Type ent in entities)
                    {
                        try
                        {
                            ICriteria criteria = transaction.NHibernateSession.CreateCriteria(ent);
                            criteria.SetMaxResults(1);

                            //If the query works is because the table exist
                            criteria.List();
                        }
                        catch (Exception)
                        {
                            //If the query fails is because the table don't exist
                            return SchemaStatus.NotExist;
                        }
                    }

                    return SchemaStatus.AlreadyExist;
                }
            }
            catch (Exception)
            {
                return SchemaStatus.ConnectionError;
            }
        }


        //public SchemaStatus GetStatus(SchemaSection section)
        //{
        //    try
        //    {
        //        string query;

        //        if (string.Equals(mConfiguration.Connection_DriverClass, ConnectionParameters.DRIVER_SQLITE, StringComparison.InvariantCultureIgnoreCase))
        //            query = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}'";
        //        else if (string.Equals(mConfiguration.Connection_DriverClass, ConnectionParameters.DRIVER_SQLSERVER, StringComparison.InvariantCultureIgnoreCase))
        //            query = "SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')";
        //        else
        //            return SchemaStatus.UnknownDriver;

        //        Type[] entities = GetEntities(section);

        //        int founded = 0;
        //        foreach (Type ent in entities)
        //        {
        //            using (TransactionScope transaction = new TransactionScope(mConfiguration))
        //            {
        //                IDbCommand command = transaction.CreateDbCommand();
        //                command.CommandText = string.Format(query, ent.Name);

        //                //Note: I don't use command parameters for a better support for special database (where the parameters are not identified by @)
        //                //IDbDataParameter param = transaction.CreateDbCommandParameter(command, "@tableName", DbType.String, ent.Name);
        //                //command.Parameters.Add(param);

        //                int count = Convert.ToInt32( command.ExecuteScalar() );
        //                if (count == 1)
        //                    founded++;
        //            }
        //        }

        //        if (entities.Length == founded)
        //            return SchemaStatus.AlreadyExist;
        //        else if (founded == 0)
        //            return SchemaStatus.NotExist;
        //        else if (entities.Length > founded)
        //            return SchemaStatus.PartialExist;
        //        else
        //            throw new BusiBlocksException("Returned value not valid");
        //    }
        //    catch (Exception)
        //    {
        //        return SchemaStatus.ConnectionError;
        //    }
        //}

        /// <summary>
        /// Get the list of entities for the specified category
        /// </summary>
        /// <param name="schemaCategory"></param>
        /// <returns></returns>
        public Type[] GetEntities(string schemaCategory)
        {
            List<Type> list;
            if (mMappings.TryGetValue(schemaCategory, out list) == false)
                throw new ArgumentException(schemaCategory + " value not supported", "section");

            return list.ToArray();
        }

        /// <summary>
        /// Get the list of schema categories.
        /// </summary>
        /// <returns></returns>
        public string[] GetSchemaCategories()
        {
            var keys = new string[mMappings.Keys.Count];
            mMappings.Keys.CopyTo(keys, 0);

            return keys;
        }

        private void LoadAssembliesMappings()
        {
            foreach (Assembly assembly in mConfiguration.MappingAssemblies)
            {
                object[] attributes = assembly.GetCustomAttributes(typeof (SetupMappingAttribute), false);

                foreach (SetupMappingAttribute attribute in attributes)
                {
                    if (mMappings.ContainsKey(attribute.Category) == false)
                        mMappings.Add(attribute.Category, new List<Type>());

                    foreach (Type type in attribute.MappingTypes)
                        mMappings[attribute.Category].Add(type);
                }
            }
        }
    }

    public enum SchemaStatus
    {
        AlreadyExist,
        PartialExist,
        NotExist,
        ConnectionError,
        UnknownDriver
    }
}