using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;

namespace BusiBlocks.AddressLayer
{
    public class BusiBlocksAddressProvider : AddressProvider
    {
        private ConnectionParameters _configuration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "BusiBlocksAddressProvider";

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

        public override void CreateAddress(Address address)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new AddressDataStore(transaction);
                ptDS.Insert(address);
                transaction.Commit();
            }
        }

        public override IList<Address> GetAllAddresss()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AddressDataStore(transaction);
                return store.FindAll();
            }
        }

        public override IList<Address> GetAllAddresssBySuburb(string suburb)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDS = new AddressDataStore(transaction);
                return sDS.FindAllBySuburb(suburb);
            }
        }

        public override IList<Address> GetAllAddresssByPostcode(string postcode)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDS = new AddressDataStore(transaction);
                return sDS.FindAllByPostcode(postcode);
            }
        }

        public override Address GetAddressById(string addressId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDS = new AddressDataStore(transaction);
                return sDS.FindByKey(addressId);
            }
        }

        public override Address GetAddressByDetails(string address1, string address2, string suburb, string postcode,
                                                    string state)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new AddressDataStore(transaction);
                return sDs.FindAllByProperties(address1, address2, suburb, postcode, state).FirstOrDefault();
            }
        }

        public override void UpdateAddress(Address address)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDS = new AddressDataStore(transaction);
                rDS.Update(address);
                transaction.Commit();
            }
        }

        public override void DeleteAddress(Address address)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDS = new AddressDataStore(transaction);
                address.Deleted = true;
                sDS.Update(address);
                transaction.Commit();
            }
        }

        public override bool AddressExists(string address1, string address2, string suburb, string postcode,
                                           string state)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var aDS = new AddressDataStore(transaction);
                IList<Address> addresses = aDS.FindAllByProperties(address1, address2, suburb, postcode, state);
                if (addresses != null)
                    if (addresses.Count > 0)
                        return true;
            }
            return false;
        }

        public override bool AddressExists(Address address)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var aDS = new AddressDataStore(transaction);
                IList<Address> addresses = aDS.FindAllByProperties(address.Address1, address.Address2, address.Suburb,
                                                                   address.Postcode, address.State);
                if (addresses != null)
                {
                    if (addresses.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
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