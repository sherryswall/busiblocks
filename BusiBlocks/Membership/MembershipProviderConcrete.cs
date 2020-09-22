using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;

namespace BusiBlocks.Membership
{
    /// <summary>
    /// The provider implementation for membership. So users and roles.
    /// </summary>
    public class MembershipProviderConcrete : MembershipProvider
    {
        private ConnectionParameters _configuration;

        #region Properties

        private string _applicationName;
        private string providerName;

        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }

        #endregion

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MembershipProviderConcrete";

            base.Initialize(name, config);

            providerName = name;
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
        private string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
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

        public override User GetUser(string userId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var uDS = new UserDataStore(transaction);
                return uDS.FindByKey(userId);
            }
        }

        public override User GetUserByName(string userName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var uDS = new UserDataStore(transaction);
                return uDS.FindByName(_applicationName, userName);
            }
        }

        public override User GetUserByPersonId(string personId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var dataStore = new UserDataStore(transaction);
                User user = dataStore.FindByPerson(_applicationName, personId);
                return user;
            }
        }

        public override void UpdateUser(User user)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var uDS = new UserDataStore(transaction);
                uDS.Update(user);
                transaction.Commit();
            }
        }

        public override IList<User> GetAllUsers()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var dataStore = new UserDataStore(transaction);
                IList<User> users = dataStore.FindAllEnabled();
                if (users == null || users.Count == 0)
                    return null;

                transaction.Commit();

                return users;
            }
        }
    }
}