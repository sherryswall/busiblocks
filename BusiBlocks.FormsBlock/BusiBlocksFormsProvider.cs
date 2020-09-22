using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Configuration;
using System.Net.Mail;

namespace BusiBlocks.FormsBlock
{
    /// <summary>
    /// Implementation of FormsProvider that use NHibernate to save and retrive informations.
    ///
    /// Configuration:
    /// connectionStringName = the name of the connection string to use
    /// 
    /// </summary>
    public class BusiBlocksFormsProvider : FormsProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksFormsProvider";

            base.Initialize(name, config);

            this.mProviderName = name;

            //Read the configurations
            //Connection string
            string connName = ExtractConfigValue(config, "connectionStringName", null);
            mConfiguration = ConnectionParameters.Create(connName);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new System.Configuration.Provider.ProviderException("Unrecognized attribute: " +
                    attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private string ExtractConfigValue(System.Collections.Specialized.NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;
            else
            {
                config.Remove(key);
                return val;
            }
        }

        #region Properties
        private string mProviderName;
        public string ProviderName
        {
            get { return mProviderName; }
            set { mProviderName = value; }
        }

        private ConnectionParameters mConfiguration;
        public ConnectionParameters Configuration
        {
            get { return mConfiguration; }
        }

        #endregion

        #region FormDefinition

        public override FormDefinition CreateFormDefinition(string name)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                FormDefinition formDefinition = new FormDefinition(name);
                dataStore.Insert(formDefinition);
                transaction.Commit();
                return formDefinition;
            }
        }

        public override void UpdateFormDefinition(FormDefinition formDefinition)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                dataStore.Update(formDefinition);
                transaction.Commit();
            }
        }

        public override void DeleteFormDefinition(FormDefinition formDefinition)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                formDefinition.Deleted = true;
                dataStore.Update(formDefinition);
                transaction.Commit();
            }
        }

        public override FormDefinition GetFormDefinition(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                FormDefinition formDefinition = dataStore.FindByKey(id);
                //if (formDefinition == null)
                //    throw new FormNotFoundException(id);
                return formDefinition;
            }
        }

        public override FormDefinition GetFormDefinitionByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                FormDefinition formDefinition = dataStore.FindByName(name);
                //if (formDefinition == null && throwIfNotFound)
                //    throw new FormNotFoundException(name);
                //else if (formDefinition == null)
                //    return null;
                return formDefinition;
            }
        }

        public override IList<FormDefinition> GetAllFormDefinitions()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormDefinitionDataStore dataStore = new FormDefinitionDataStore(transaction);
                return dataStore.FindAll();
            }
        }

        #endregion


        #region Form Property

        public override FormProperty CreateFormProperty(FormDefinition formDefinition, string name, string datatype, int sequenceNo)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                FormProperty formProperty = new FormProperty(formDefinition, name, datatype, sequenceNo);
                dataStore.Insert(formProperty);
                transaction.Commit();
                return formProperty;
            }
        }

        public override void UpdateFormProperty(FormProperty formProperty)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                dataStore.Update(formProperty);
                transaction.Commit();
            }
        }

        public override void DeleteFormProperty(FormProperty formProperty)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                formProperty.Deleted = true;
                dataStore.Update(formProperty);
                transaction.Commit();
            }
        }

        public override FormProperty GetFormProperty(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                FormProperty formProperty = dataStore.FindByKey(id);
                //if (formDefinition == null)
                //    throw new FormNotFoundException(id);
                return formProperty;
            }
        }

        public override FormProperty GetFormPropertyByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                FormProperty formProperty = dataStore.FindByName(name);
                //if (formDefinition == null && throwIfNotFound)
                //    throw new FormNotFoundException(name);
                //else if (formDefinition == null)
                //    return null;
                return formProperty;
            }
        }

        public override IList<FormProperty> GetAllFormProperties(FormDefinition formDefinition)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyDataStore dataStore = new FormPropertyDataStore(transaction);
                return dataStore.FindAll(formDefinition);
            }
        }

        #endregion


        #region Form Instance

        public override FormInstance CreateFormInstance(FormDefinition formDefinition, string createdBy)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                FormInstance formInstance = new FormInstance(formDefinition, createdBy);
                dataStore.Insert(formInstance);
                transaction.Commit();
                return formInstance;
            }
        }

        public override void UpdateFormInstance(FormInstance formInstance)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                dataStore.Update(formInstance);
                transaction.Commit();
            }
        }

        public override void DeleteFormInstance(FormInstance formInstance)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                formInstance.Deleted = true;
                dataStore.Update(formInstance);
                transaction.Commit();
            }
        }

        public override FormInstance GetFormInstance(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                FormInstance formInstance = dataStore.FindByKey(id);
                //if (formDefinition == null)
                //    throw new FormNotFoundException(id);
                return formInstance;
            }
        }

        public override FormInstance GetFormInstanceByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                FormInstance formInstance = dataStore.FindByName(name);
                //if (formDefinition == null && throwIfNotFound)
                //    throw new FormNotFoundException(name);
                //else if (formDefinition == null)
                //    return null;
                return formInstance;
            }
        }

        public override IList<FormInstance> GetAllFormInstances()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormInstanceDataStore dataStore = new FormInstanceDataStore(transaction);
                return dataStore.FindAll();
            }
        }

        #endregion


        #region Form Property Instance

        public override FormPropertyInstance CreateFormPropertyInstance(FormInstance formInstance, FormProperty formProperty, string value)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyInstanceDataStore dataStore = new FormPropertyInstanceDataStore(transaction);
                FormPropertyInstance formPropertyInstance = new FormPropertyInstance(formInstance, formProperty, value);
                dataStore.Insert(formPropertyInstance);
                transaction.Commit();
                return formPropertyInstance;
            }
        }

        public override void UpdateFormPropertyInstance(FormPropertyInstance formPropertyInstance)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyInstanceDataStore dataStore = new FormPropertyInstanceDataStore(transaction);
                dataStore.Update(formPropertyInstance);
                transaction.Commit();
            }
        }

        public override void DeleteFormPropertyInstance(FormPropertyInstance formPropertyInstance)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyInstanceDataStore dataStore = new FormPropertyInstanceDataStore(transaction);
                formPropertyInstance.Deleted = true;
                dataStore.Update(formPropertyInstance);
                transaction.Commit();
            }
        }

        public override FormPropertyInstance GetFormPropertyInstance(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FormPropertyInstanceDataStore dataStore = new FormPropertyInstanceDataStore(transaction);
                FormPropertyInstance formPropertyInstance = dataStore.FindByKey(id);
                //if (formDefinition == null)
                //    throw new FormNotFoundException(id);
                return formPropertyInstance;
            }
        }

        public override FormPropertyInstance GetFormPropertyInstanceByKeys(FormInstance formInstance, FormProperty formProperty, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                throw new NotImplementedException("GetFormPropertyInstanceByKeys not implemented");
                //FormPropertyInstanceDataStore dataStore = new FormPropertyInstanceDataStore(transaction);
                //FormPropertyInstance formPropertyInstance = dataStore.find.FindByName(name);
                //if (formDefinition == null && throwIfNotFound)
                //    throw new FormNotFoundException(name);
                //else if (formDefinition == null)
                //    return null;
                //return formPropertyInstance;
                //return null;
            }
        }
        #endregion
    }
}
