using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace BusiBlocks.FormsBlock
{
    public class FormsManager
    {
        private FormsManager() { }

        static FormsManager()
        {
            //Get the feature's configuration info
            FormsProviderConfiguration qc =
                (FormsProviderConfiguration)ConfigurationManager.GetSection("formsManager");

            if (qc == null || qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for formsManager.");

            //Instantiate the providers
            providerCollection = new FormsProviderCollection();
            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(FormsProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[qc.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the formsManager.",
                    qc.ElementInformation.Properties["defaultProvider"].Source,
                    qc.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        //Public feature API
        private static FormsProvider defaultProvider;
        private static FormsProviderCollection providerCollection;

        public static FormsProvider Provider
        {
            get {return defaultProvider;}
        }

        public static FormsProviderCollection Providers
        {
            get {return providerCollection;}
        }

        #region Static methods

        #region FormDefinition
        public static FormDefinition CreateFormDefinition(string name)
        {
            return Provider.CreateFormDefinition(name);
        }

        public static void UpdateFormDefinition(FormDefinition formDefinition)
        {
            Provider.UpdateFormDefinition(formDefinition);
        }

        public static void DeleteFormDefinition(FormDefinition formDefinition)
        {
            Provider.DeleteFormDefinition(formDefinition);
        }

        public static FormDefinition GetFormDefinition(string id)
        {
            return Provider.GetFormDefinition(id);
        }

        public static FormDefinition GetFormDefinitionByName(string name, bool throwIfNotFound)
        {
            return Provider.GetFormDefinitionByName(name, throwIfNotFound);
        }

        public static IList<FormDefinition> GetAllFormDefinitions()
        {
            return Provider.GetAllFormDefinitions();
        }
        #endregion

        #region FormProperty
        public static FormProperty CreateFormProperty(FormDefinition formDefinition, string name, string datatype, int sequenceNo)
        {
            return Provider.CreateFormProperty(formDefinition, name, datatype, sequenceNo);
        }

        public static void UpdateFormProperty(FormProperty formProperty)
        {
            Provider.UpdateFormProperty(formProperty);
        }

        public static void DeleteFormProperty(FormProperty formProperty)
        {
            Provider.DeleteFormProperty(formProperty);
        }

        public static FormProperty GetFormProperty(string id)
        {
            return Provider.GetFormProperty(id);
        }

        public static FormProperty GetFormPropertyByName(string name, bool throwIfNotFound)
        {
            return Provider.GetFormPropertyByName(name, throwIfNotFound);
        }

        public static IList<FormProperty> GetAllFormProperties(FormDefinition formDefinition)
        {
            return Provider.GetAllFormProperties(formDefinition);
        }
        #endregion


        #region FormInstance
        public static FormInstance CreateFormInstance(FormDefinition formDefinition, string createdBy)
        {
            return Provider.CreateFormInstance(formDefinition, createdBy);
        }

        public static void UpdateFormInstance(FormInstance formInstance)
        {
            Provider.UpdateFormInstance(formInstance);
        }

        public static void DeleteFormInstance(FormInstance formInstance)
        {
            Provider.DeleteFormInstance(formInstance);
        }

        public static FormInstance GetFormInstance(string id)
        {
            return Provider.GetFormInstance(id);
        }

        public static FormInstance GetFormInstanceByName(string name, bool throwIfNotFound)
        {
            return Provider.GetFormInstanceByName(name, throwIfNotFound);
        }

        public static IList<FormInstance> GetAllFormInstances()
        {
            return Provider.GetAllFormInstances();
        }
        #endregion


        #region FormPropertyInstance
        public static FormPropertyInstance CreateFormPropertyInstance(FormInstance formInstance, FormProperty formProperty, string value)
        {
            return Provider.CreateFormPropertyInstance(formInstance, formProperty, value);
        }

        public static void UpdateFormPropertyInstance(FormPropertyInstance formPropertyInstance)
        {
            Provider.UpdateFormPropertyInstance(formPropertyInstance);
        }

        public static void DeleteFormPropertyInstance(FormPropertyInstance formPropertyInstance)
        {
            Provider.DeleteFormPropertyInstance(formPropertyInstance);
        }

        public static FormPropertyInstance GetFormPropertyInstance(string id)
        {
            return Provider.GetFormPropertyInstance(id);
        }
        #endregion


        #endregion
    }
}
