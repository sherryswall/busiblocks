using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace BusiBlocks.AddressLayer
{
    public class AddressManager
    {
        //Public feature API
        private static readonly AddressProviderCollection providerCollection = InitialiseProviderCollection();

        static AddressManager()
        {
        }

        private static AddressProvider defaultProvider
        {
            get
            {
                if (providerCollection != null)
                {
                    //Get the feature's configuration info
                    var ac =
                        (AddressProviderConfiguration)ConfigurationManager.GetSection("addressManager");

                    if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                        throw new ProviderException("You must specify a valid default provider for addressManager.");

                    return providerCollection[ac.DefaultProvider];
                }
                return null;
            }
        }

        public static AddressProvider Provider
        {
            get { return defaultProvider; }
        }

        public static AddressProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static AddressProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (AddressProviderConfiguration)ConfigurationManager.GetSection("addressManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for addressManager.");

            var providerCollection = new AddressProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(AddressProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        public static void CreateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Provider.CreateAddress(address);
        }

        public static IList<Address> GetAllAddresss()
        {
            return Provider.GetAllAddresss();
        }

        public static IList<Address> GetAllAddresssBySuburb(string suburb)
        {
            if (suburb == null)
                throw new ArgumentNullException("suburb");
            return Provider.GetAllAddresssBySuburb(suburb);
        }

        public static IList<Address> GetAllAddresssByPostcode(string postcode)
        {
            if (postcode == null)
                throw new ArgumentNullException("postcode");
            return Provider.GetAllAddresssByPostcode(postcode);
        }

        public static Address GetAddressById(string addressId)
        {
            if (string.IsNullOrEmpty(addressId))
                throw new ArgumentNullException("addressId");
            return Provider.GetAddressById(addressId);
        }

        public static Address GetAddressByDetails(string address1, string address2, string suburb, string postcode,
                                                  string state)
        {
            if (string.IsNullOrEmpty(address1))
                throw new ArgumentNullException("address1");

            return Provider.GetAddressByDetails(address1, address2, suburb, postcode, state);
        }

        public static void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Provider.UpdateAddress(address);
        }

        public static void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            Provider.DeleteAddress(address);
        }

        public static bool AddressExists(Address address)
        {
            return Provider.AddressExists(address);
        }

        public static bool AddressExists(string address1, string address2, string suburb, string postcode, string state)
        {
            return Provider.AddressExists(address1, address2, suburb, postcode, state);
        }
    }
}