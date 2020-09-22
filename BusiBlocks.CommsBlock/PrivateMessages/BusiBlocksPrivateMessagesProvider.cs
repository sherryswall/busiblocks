using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Configuration;
using System.Net.Mail;
using BusiBlocks.Membership;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    /// <summary>
    /// Implementation of PrivateMessageProvider that use NHibernate to save and retrive informations.
    ///
    /// Configuration:
    /// connectionStringName = the name of the connection string to use
    /// 
    /// </summary>    
    public class BusiBlocksPrivateMessagesProvider : PrivateMessagesProvider
    {

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksPrivatesMessagesProvider";

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
        #endregion

        public override IList<PrivateMessage> FindByRecipient(User User)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                PrivateMessageDataStore pmDS = new PrivateMessageDataStore(transaction);
                return pmDS.FindByRecipient(User);
            }
        }

        public override IList<PrivateMessage> FindBySender(User User)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                PrivateMessageDataStore pmDS = new PrivateMessageDataStore(transaction);
                return pmDS.FindBySender(User);
            }
        }


        public override PrivateMessage FindById(string PrivateMessageId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                PrivateMessageDataStore pmDS = new PrivateMessageDataStore(transaction);
                return pmDS.FindById(PrivateMessageId);
            }
        }


        public override void Update(PrivateMessage PrivateMessage)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                PrivateMessageDataStore pmDS = new PrivateMessageDataStore(transaction);
                pmDS.Update(PrivateMessage);
                transaction.Commit();
            }
        }


        public override void Insert(PrivateMessage PrivateMessage)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                PrivateMessageDataStore pmDS = new PrivateMessageDataStore(transaction);
                pmDS.Insert(PrivateMessage);
                transaction.Commit();
            }
        }
    }
}
