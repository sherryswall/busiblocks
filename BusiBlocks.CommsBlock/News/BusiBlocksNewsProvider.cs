using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Configuration;
using System.Net.Mail;
using BusiBlocks.AccessLayer;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.Versioning;
namespace BusiBlocks.CommsBlock.News
{
    /// <summary>
    /// Implementation of NewsProvider that use NHibernate to save and retrive informations.
    ///
    /// Configuration:
    /// connectionStringName = the name of the connection string to use
    /// 
    /// </summary>
    public class BusiBlocksNewsProvider : NewsProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksDocoProvider";

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
        public ConnectionParameters Configuration
        {
            get { return mConfiguration; }
        }

        #endregion

        #region methods
        #region Category
        public override Category CreateCategory(string name)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                Category category = new Category(name);

                dataStore.Insert(category);

                transaction.Commit();

                return category;
            }
        }

        public override void UpdateCategory(Category category)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                dataStore.Update(category);

                transaction.Commit();
            }
        }

        public override void DeleteCategory(Category category)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                // Delete should only be allowed if the following conditions are true:
                // 1. There are no child categories.
                if (dataStore.FindAllChildren(category.Id).Count > 0)
                    throw new SchemaIntegrityException("Category has child categories");

                // 2. There are no articles associated with this category.
                ItemDataStore ads = new ItemDataStore(transaction);
                if (ads.FindByCategory(category).Count > 0)
                    throw new SchemaIntegrityException("News items are associated with this category");

                // Delete all Access records.
                AccessDataStore accessDs = new AccessDataStore(transaction);
                IList<Access> allAccess = accessDs.FindAllByItem(category.Id);

                foreach (Access access in allAccess)
                {
                    access.Deleted = true;
                    accessDs.Update(access);
                }

                category.Deleted = true;
                category.Name += DateTimeHelper.GetCurrentTimestamp();
                dataStore.Update(category);

                transaction.Commit();
            }
        }

        public override Category GetCategory(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                Category category = dataStore.FindByKey(id);
                if (category == null)
                    throw new NewsCategoryNotFoundException(id);

                return category;
            }
        }

        public override IList<Category> GetCategories(string rootCategoryId, bool recursive)
        {
            List<Category> categories = new List<Category>();
            IList<Category> children = new List<Category>();
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                Category category = dataStore.FindByKey(rootCategoryId);
                if (category == null)
                    throw new NewsCategoryNotFoundException(rootCategoryId);
                categories.Add(category);
                if (recursive)
                {
                    // Find child categories.
                    children = dataStore.FindAllChildren(rootCategoryId);
                }
            }

            foreach (Category childCategory in children)
            {
                categories.AddRange(GetCategories(childCategory.Id, recursive));
            }

            return categories;
        }

        public override Category GetCategoryByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                Category category = dataStore.FindByName(name);
                if (category == null && throwIfNotFound)
                    throw new NewsCategoryNotFoundException(name);
                else if (category == null)
                    return null;

                return category;
            }
        }

        public override IList<Category> GetCategoryByLikeName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                return dataStore.FindByLikeName(name);
            }
        }

        public override IList<Category> GetAllCategories()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                return dataStore.FindAll();
            }
        }
        #endregion

        #region Items
        public override Item CreateItem(Category category, string owner,
                                        string title, string description, string url, string urlName,
                                        DateTime newsDate, bool acknowledge, bool approve, string groups,
                                        Attachment.FileInfo attachment, DateTime? expiry, ItemApprovalStatus approvalStatus)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);

                Item item = new Item(category, owner, title, description, url, urlName, newsDate, expiry, approvalStatus);
                item.Tag = acknowledge.ToString() + ":" + approve.ToString();
                item.Groups = groups;
                item.Attachment = attachment;

                dataStore.Insert(item);
                transaction.Commit();
                return item;
            }
        }

        public override IList<Item> GetItems()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                return dataStore.FindAllItems();
            }
        }

        public override IList<Item> GetItems(Category category, bool recursive)
        {
            IList<Item> items = null;
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                items = dataStore.FindByCategory(category);
                if (recursive)
                {
                    CategoryDataStore ds = new CategoryDataStore(transaction);
                    IList<Category> children = ds.FindAllChildren(category.Id);
                    foreach (Category child in children)
                    {
                        IList<Item> childItems = GetItems(child, recursive);
                        foreach (Item item in childItems)
                        {
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public override IList<Item> GetPublishedItems(Category category, bool recursive)
        {
            IList<Item> publishedItems = null;
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                publishedItems = dataStore.FindPublishedByCategory(category);
            }
            return publishedItems;
        }

        public override void UpdateItem(Item item)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                dataStore.Update(item);
                transaction.Commit();
            }
        }

        public override void DeleteItem(Item item)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                item.Deleted = true;
                dataStore.Update(item);
                transaction.Commit();
            }
        }

        public override void DeleteItemByGroup(string groupID)
        {
            IList<VersionItem> versionsToDelete = VersionManager.GetAllVersions(groupID);

            foreach (VersionItem versToDelete in versionsToDelete)
            {
                Item item = NewsManager.GetItem(versToDelete.ItemId);
                NewsManager.DeleteItem(item);
                VersionManager.DeleteVersionItem(versToDelete);
            }
        }

        public override Item GetItem(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                return dataStore.FindByKey(id);
            }
        }

        public override IList<Item> FindItems(Filter<string> categoryName,
                                              Filter<string> tag,
                                              DateTime? fromDate, DateTime? toDate,
                                              PagingInfo paging)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                return dataStore.FindByFields(categoryName, tag, fromDate, toDate, paging);
            }
        }

        public override IList<Item> GetItemsByGroup(string groupId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                return dataStore.FindByGroup(groupId);
            }
        }

        public override IList<Item> GetItemsByOwner(string owner)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ItemDataStore dataStore = new ItemDataStore(transaction);
                return dataStore.FindByOwner(owner);
            }
        }

        public override TrafficLightStatus GetTrafficLight(string userName, Item item)
        {
            return TrafficLightHelper.GetTrafficLight(userName, item.Id, item.RequiresAck);
        }

        #endregion

        #endregion
    }
}
