using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

namespace BusiBlocks.ItemApprovalStatusLayer
{
    public class BusiBlocksItemApprovalStatusProvider : ItemApprovalStatusProvider
    {
        private ConnectionParameters _configuration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "BusiBlocksGroupProvider";

            base.Initialize(name, config);

            _providerName = name;
            _applicationName = ExtractConfigValue(config, "applicationName", ConnectionParameters.DefaultApp);
                //System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath

            string connName = ExtractConfigValue(config, "connectionStringName", null);
            _configuration = ConnectionParameters.Create(connName);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;
            
            config.Remove(key);
            return val;
        }


        public override ItemApprovalStatus GetItemApprovalStatusByName(string name)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new ItemApprovalStatusDataStore(transaction);
                return store.FindByName(name);
            }
        }

        public override ItemApprovalStatus GetItemApprovalStatus(string id)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new ItemApprovalStatusDataStore(transaction);
                return store.Find(id);
            }
        }

        #region Properties

        private string _applicationName;
        private string _providerName;

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        #endregion
    }
}