using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Configuration.Provider;

namespace BusiBlocks.Versioning
{
    public static class VersionManager
    {
        //Public feature API
        private static VersionProviderCollection providerCollection = InitialiseProviderCollection();

        private static VersionProvider defaultProvider
        {
            get
            {
                var ac =
                    (VersionProviderConfiguration)ConfigurationManager.GetSection("versionManager");

                if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                    throw new ProviderException("You must specify a valid default provider for siteManager.");

                return providerCollection[ac.DefaultProvider];
            }
        }

        public static VersionProvider Provider
        {
            get { return defaultProvider; }
        }

        public static VersionProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static VersionProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (VersionProviderConfiguration)ConfigurationManager.GetSection("versionManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for versionManager.");

            //Instantiate the providers
            providerCollection = new VersionProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(VersionProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }
        
        public static void CreateVersionItem(VersionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Provider.CreateVersionItem(item);
        }

        public static void UpdateVersionItem(VersionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Provider.UpdateVersionItem(item);
        }

        public static void DeleteVersionItem(VersionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Provider.DeleteVersionItem(item);
        }

        public static IList<VersionItem> GetAllVersions(string groupId)
        {
            if (groupId == null)
                throw new ArgumentNullException("groupId");

            return Provider.GetAllVersions(groupId);
        }

        public static string GetVersionNumber(VersionType versionType, string id)
        {
            return Provider.GetVersionNumber(versionType, id);
        }

        public static VersionItem GetVersionByItemId(string itemId)
        {
            return Provider.GetVersionByItemId(itemId);
        }
        public static VersionItem GetVersionById(string id)
        {
            return Provider.GetVersionItemById(id);
        }
        public static VersionItem GetVersionByGroupId(string groupId)
        {
            return Provider.GetVersionByGroupId(groupId);
        }
        public static bool IsLatestVersion(string versionId)
        {
            return Provider.IsLatestVersion(versionId);
        }
        public static List<VersionItem> GetAllLatestDrafts()
        {
            return Provider.GetAllLatestDrafts();
        }
        public static VersionItem GetPublishedVersion(string groupId)
        {
            return Provider.GetPublishedVersion(groupId);
        }

        public static IList<VersionItem> GetPublishedVersions(string groupId)
        {
            return Provider.GetPublishedVersions(groupId);
        }
        public static IList<VersionItem> GetRespectivePublishedVersions(string groupId,string versionNumber)
        {
            return Provider.GetRespectivePublishedVersions(groupId,versionNumber);
        }

        public static void CheckInVersion(string versionId)
        {
            Provider.CheckInVersion(versionId);
        }
        public static void CheckOutVersion(string versionId,string userName)
        {
            Provider.CheckOutVersion(versionId, userName);
        }

        public static string GetCheckedOutUser(string groupId)
        {
            return Provider.GetCheckedOutUser(groupId);
        }

    }
}
