using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

namespace BusiBlocks.AccessLayer
{
    public class BusiBlocksAccessProvider : AccessProvider
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

        public override void CreateAccess(Access access)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);

                if (store.Find(access.ItemId, access.ItemType, access.AccessType, access.PersonTypeId, access.SiteId) ==
                    null)
                {
                    store.InsertOrUpdate(access);
                    transaction.Commit();
                }
            }
        }

        public override void DeleteAccess(string accessId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);

                if (store.FindByKey(accessId) != null)
                {
                    Access access = store.FindByKey(accessId);
                    access.Deleted = true;
                    store.Update(access);
                    transaction.Commit();
                }
            }
        }

        public override void DeleteAccess(string groupId, string itemId, ItemType itemType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);
                Access a = store.Find(itemId, itemType, AccessType.View, groupId);

                if (a != null)
                {
                    a.Deleted = true;
                    store.Update(a);
                    transaction.Commit();
                }
            }
        }

        public override IList<Access> GetItemAccess(string itemId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);
                return store.FindAllByItem(itemId);
            }
        }

        public override IList<Access> GetItemVisibilities(string itemId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);
                return store.FindVisibilityByItem(itemId);
            }
        }
        
        public override IList<Access> GetItemEdittables(string itemId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);
                return store.FindEdittableByItem(itemId);
            }
        }

        public override IList<Access> GetItemContributions(string itemId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);
                return store.FindContributableByItem(itemId);
            }
        }

        public override Folder GetFolderKey(string path)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new FolderDataStore(transaction);
                return store.FindByPath(path);
            }
        }
        
        public override void CreateFolderKey(Folder folder)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new FolderDataStore(transaction);
                Folder f = store.FindByPath(folder.Path);

                if (f == null)
                {
                    store.InsertOrUpdate(folder);
                    transaction.Commit();
                }
            }
        }

        public override void DeleteFolderKey(string path)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new FolderDataStore(transaction);

                Folder folder = store.FindByPath(path);

                if (folder != null)
                {
                    store.Delete(folder.Id);
                    transaction.Commit();
                }
            }
        }

        public override List<Access> GetUsersAccessibleItems(string userName, ItemType itemType, AccessType accessType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);

                IList<PersonType> personTypes = PersonManager.GetPersonTypesByUser(userName);

                IList<Site> sites = SiteManager.GetSitesByUser(userName, true);

                var accessList = new List<Access>();

                accessList.AddRange(store.FindForAllUsers(itemType, accessType));
                accessList.AddRange(store.FindForAllSitesPlusAllPersonTypes(itemType, accessType));
                accessList.AddRange(store.FindForUser(itemType, accessType, userName));

                foreach (PersonType personType in personTypes)
                {
                    accessList.AddRange(store.FindForAllSitesPlusPersonType(itemType, accessType, personType.Id));

                    foreach (Site site in sites)
                    {
                        accessList.AddRange(store.FindForPersonTypeSite(itemType, accessType, personType.Id, site.Id));
                    }
                }

                foreach (Site site in sites)
                {
                    accessList.AddRange(store.FindForAllPersonTypesPlusSite(itemType, accessType, site.Id));
                }

                //remove duplicates
                var cleanAccessList = new List<Access>();

                foreach (Access access in accessList)
                {
                    if (!cleanAccessList.Exists(delegate(Access a) { return a.Id == access.Id; }))
                    {
                        cleanAccessList.Add(access);
                    }
                }

                return cleanAccessList;
            }
        }

        public override IList<Access> GetItemsMatchingAccess(Access access, ItemType itemType, AccessType accessType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new AccessDataStore(transaction);

                if (access.AllUsers)
                {
                    return store.FindForAllUsers(itemType, accessType);
                }

                if (access.AllPersonTypes && access.AllSites)
                {
                    return store.FindForAllPersonTypesPlusAllSites(itemType, accessType);
                }

                if (access.UserId != null)
                {
                    return store.FindForUser(itemType, accessType, access.UserId);
                }

                if (access.AllPersonTypes && access.SiteId != null)
                {
                    return store.FindForAllPersonTypesPlusSite(itemType, accessType, access.SiteId);
                }

                if (access.AllSites && access.PersonTypeId != null)
                {
                    return store.FindForAllSitesPlusPersonType(itemType, accessType, access.PersonTypeId);
                }

                if (access.SiteId != null && access.PersonTypeId != null)
                {
                    return store.FindForPersonTypeSite(itemType, accessType, access.PersonTypeId, access.SiteId);
                }

                return new List<Access>();
            }
        }

        public override IList<User> GetAccessibleItemUsers(Access access)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                if (access.AllUsers)
                {
                    var userStore = new UserDataStore(transaction);
                    return userStore.FindAll(_applicationName);
                }

                if (access.AllPersonTypes && access.AllSites)
                {
                    var userStore = new UserDataStore(transaction);
                    return userStore.FindAll(_applicationName);
                }

                if (access.UserId != null)
                {
                    var userStore = new UserDataStore(transaction);
                    var singleUserList = new List<User>();
                    singleUserList.Add(userStore.FindByName(_applicationName, access.UserId));
                    return singleUserList;
                }

                if (access.AllPersonTypes && access.SiteId != null)
                {
                    var store = new PersonSiteDataStore(transaction);
                    IList<PersonSite> people = store.FindPersonsBySite(access.SiteId, false);
                    var userList = new List<User>();
                    var userStore = new UserDataStore(transaction);

                    foreach (PersonSite person in people)
                    {
                        userList.Add(userStore.FindByPerson(_applicationName, person.Person.Id));
                    }

                    return userList;
                }

                if (access.AllSites && access.PersonTypeId != null)
                {
                    var store = new PersonPersonTypeDataStore(transaction);
                    IList<PersonPersonType> people = store.FindPersonByPersonType(access.PersonTypeId);
                    var userList = new List<User>();
                    var userStore = new UserDataStore(transaction);

                    foreach (PersonPersonType person in people)
                    {
                        userList.Add(userStore.FindByPerson(_applicationName, person.Person.Id));
                    }

                    return userList;
                }

                if (access.SiteId != null && access.PersonTypeId != null)
                {
                    var userInPersonTypeStore = new PersonPersonTypeDataStore(transaction);
                    IList<PersonPersonType> personInPersonType =
                        userInPersonTypeStore.FindPersonByPersonType(access.PersonTypeId);

                    var userInSiteStore = new PersonSiteDataStore(transaction);
                    IList<PersonSite> userInSites = userInSiteStore.FindPersonsBySite(access.SiteId, false);

                    //convert to a generic list to perform better search
                    var userInSitesList = new List<PersonSite>(userInSites);

                    var usersInBoth = new List<User>();

                    var userStore = new UserDataStore(transaction);

                    foreach (PersonPersonType personPersonType in personInPersonType)
                    {
                        if (
                            userInSitesList.Exists(
                                delegate(PersonSite ul) { return ul.Person.Id == personPersonType.Person.Id; }))
                        {
                            usersInBoth.Add(userStore.FindByName(_applicationName, personPersonType.Person.Id));
                        }
                    }

                    return usersInBoth;
                }

                return new List<User>();
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