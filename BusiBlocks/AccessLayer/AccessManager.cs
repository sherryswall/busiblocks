using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using BusiBlocks.Membership;

namespace BusiBlocks.AccessLayer
{
    public static class AccessManager
    {
        private static readonly AccessProviderCollection providerCollection = InitialiseProviderCollection();

        static AccessManager()
        {
        }

        private static AccessProvider defaultProvider
        {
            get
            {
                if (providerCollection != null)
                {
                    //Get the feature's configuration info
                    var ac =
                        (AccessProviderConfiguration)ConfigurationManager.GetSection("accessManager");

                    if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                        throw new ProviderException("You must specify a valid default provider for accessManager.");

                    return providerCollection[ac.DefaultProvider];
                }
                return null;
            }
        }

        public static AccessProvider Provider
        {
            get { return defaultProvider; }
        }

        public static AccessProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static AccessProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (AccessProviderConfiguration)ConfigurationManager.GetSection("accessManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for accessManager.");

            var providerCollection = new AccessProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(AccessProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        public static void AddAccess(Access access)
        {
            Provider.CreateAccess(access);
        }

        public static void RemoveAccess(string accessId)
        {
            Provider.DeleteAccess(accessId);
        }

        public static void RemoveAccess(string groupId, string itemId, ItemType itemType)
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException("groupId");
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("itemId");

            Provider.DeleteAccess(groupId, itemId, itemType);
        }

        public static Folder GetFolderKey(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return Provider.GetFolderKey(path);
        }

        public static void AddFolderKey(Folder folder)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");

            Provider.CreateFolderKey(folder);
        }

        public static void RemoveFolderKey(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Provider.DeleteFolderKey(path);
        }

        public static IList<Access> GetItemAccess(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("itemId");

            return Provider.GetItemAccess(itemId);
        }

        public static IList<Access> GetItemVisibilities(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("itemId");

            return Provider.GetItemVisibilities(itemId);
        }

        public static IList<Access> GetItemEdittables(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("itemId");

            return Provider.GetItemEdittables(itemId);
        }

        public static IList<Access> GetItemContributions(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("itemId");

            return Provider.GetItemContributions(itemId);
        }

        public static List<Access> GetUsersAccessibleItems(string userName, ItemType itemType, AccessType accessType)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return Provider.GetUsersAccessibleItems(userName, itemType, accessType);
        }

        public static IList<User> GetAccessibleItemUsers(Access access)
        {
            if (access == null)
                throw new ArgumentNullException("access");

            return Provider.GetAccessibleItemUsers(access);
        }

        public static IList<Access> GetItemsMatchingAccess(Access access, ItemType itemType, AccessType accessType)
        {
            if (access == null)
                throw new ArgumentNullException("access");

            return Provider.GetItemsMatchingAccess(access, itemType, accessType);
        }
    }
}