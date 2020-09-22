using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace BusiBlocks.ItemStatusLayer
{
    public static class ItemStatusManager
    {
        //Public feature API
        private static ItemStatusProvider _defaultProvider;
        private static ItemStatusProviderCollection _providerCollection;

        public static ItemStatusProvider Provider
        {
            get { return ReturnDefaultProvider(); }
        }

        public static ItemStatusProviderCollection Providers
        {
            get { return _providerCollection; }
        }

        public static ItemStatusProvider ReturnDefaultProvider()
        {
            //Get the feature's configuration info
            var ac =
                (ItemStatusProviderConfiguration)ConfigurationManager.GetSection("itemStatusManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider itemStatusManager.");
             
            //Instantiate the providers
            _providerCollection = new ItemStatusProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, _providerCollection, typeof(ItemStatusProvider));
            _providerCollection.SetReadOnly();
            _defaultProvider = _providerCollection[ac.DefaultProvider];
            if (_defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the itemStatusManager.",
                    ac.ElementInformation.Properties["defaultProvider"].Source,
                    ac.ElementInformation.Properties["defaultProvider"].LineNumber);
            }

            return _defaultProvider;
        }

        public static ItemStatus GetStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            return Provider.GetItemStatus(id);
        }

        public static ItemStatus GetStatusByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Provider.GetItemStatusByName(name);
        }

        public static ItemStatus GetDraftStatus()
        {
            return GetStatusByName("Draft");
        }

        public static ItemStatus GetForApprovalStatus()
        {
            return GetStatusByName("For Approval");
        }

        public static ItemStatus GetApprovalStatus()
        {
            return GetStatusByName("Approved");
        }

    }
}