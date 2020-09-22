using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace BusiBlocks.ItemApprovalStatusLayer
{
    public static class ItemApprovalStatusManager
    {
        private static readonly ItemApprovalStatusProviderCollection providerCollection = InitialiseProviderCollection();

        static ItemApprovalStatusManager()
        {
        }

        private static ItemApprovalStatusProvider defaultProvider
        {
            get
            {
                if (providerCollection != null)
                {
                    //Get the feature's configuration info
                    var ac =
                        (ItemApprovalStatusProviderConfiguration)ConfigurationManager.GetSection("ItemApprovalStatusManager");

                    if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                        throw new ProviderException("You must specify a valid default provider for itemApprovalStatusManager.");

                    return providerCollection[ac.DefaultProvider];
                }
                return null;
            }
        }

        public static ItemApprovalStatusProvider Provider
        {
            get { return defaultProvider; }
        }

        public static ItemApprovalStatusProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static ItemApprovalStatusProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (ItemApprovalStatusProviderConfiguration)ConfigurationManager.GetSection("ItemApprovalStatusManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for itemApprovalStatusManager.");

            var providerCollection = new ItemApprovalStatusProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(ItemApprovalStatusProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        public static ItemApprovalStatus GetStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            return Provider.GetItemApprovalStatus(id);
        }

        public static ItemApprovalStatus GetStatusByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Provider.GetItemApprovalStatusByName(name);
        }

        public static ItemApprovalStatus GetDraftStatus()
        {
            return GetStatusByName("Draft");
        }

        public static ItemApprovalStatus GetForApprovalStatus()
        {
            return GetStatusByName("For Approval");
        }

        public static ItemApprovalStatus GetApprovalStatus()
        {
            return GetStatusByName("Published");
        }

        public static ItemApprovalStatus GetForEditApprovalStatus()
        {
            return GetStatusByName("For Edit Approval");
        }

    }
}