using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;

namespace BusiBlocks.ContactFeedback
{
    public class BusiBlocksFeedbackProvider : FeedbackFormProvider
    {
        #region Properties

        private string _applicationName;
        private string _providerName;

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        #endregion

        private ConnectionParameters _configuration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksSiteProvider";

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
                    throw new ProviderException("Unrecognized attribute: " +
                                                attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private static string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;

            config.Remove(key);
            return val;
        }

        public override void CreateFeedbackFormRequest(FeedbackForm form)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                FeedbackFormDataStore ffDS = new FeedbackFormDataStore(transaction);
                ffDS.Insert(form);
                transaction.Commit();
            }
        }
    }
}
