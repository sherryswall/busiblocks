using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Configuration;
using System.Linq;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.Versioning;

namespace BusiBlocks.CommsBlock.News
{
    public class NewsManager
    {
        static NewsManager()
        {
            //Get the feature's configuration info
            NewsProviderConfiguration qc = (NewsProviderConfiguration)ConfigurationManager.GetSection("newsManager");

            if (qc == null || qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for newsManager.");

            //Instantiate the providers
            providerCollection = new NewsProviderCollection();
            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(NewsProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[qc.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the newsManager.",
                    qc.ElementInformation.Properties["defaultProvider"].Source,
                    qc.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        //Public feature API
        private static NewsProvider defaultProvider;
        private static NewsProviderCollection providerCollection;

        public static NewsProvider Provider
        {
            get { return defaultProvider; }
        }

        public static NewsProviderCollection Providers
        {
            get { return providerCollection; }
        }

        #region Static methods

        #region Category
        public static Category CreateCategory(string name)
        {
            return Provider.CreateCategory(name);
        }

        public static void UpdateCategory(Category category)
        {
            Provider.UpdateCategory(category);
        }

        public static void DeleteCategory(Category category)
        {
            Provider.DeleteCategory(category);
        }

        public static Category GetCategory(string id)
        {
            return Provider.GetCategory(id);
        }

        public static IList<Category> GetCategories(string rootCategoryId, bool recursive)
        {
            return Provider.GetCategories(rootCategoryId, recursive);
        }

        public static Category GetCategoryByName(string name, bool throwIfNotFound)
        {
            return Provider.GetCategoryByName(name, throwIfNotFound);
        }
        public static IList<Category> GetCategoryByLikeName(string name, bool throwIfNotFound)
        {
            return Provider.GetCategoryByLikeName(name, throwIfNotFound);
        }

        public static IList<Category> GetAllCategories()
        {
            return Provider.GetAllCategories();
        }

        public static IList<Category> GetViewableCategories(string username)
        {
            List<Category> categories = new List<Category>();
            foreach (Category category in Provider.GetAllCategories())
            {
                if (BusiBlocks.SecurityHelper.CanUserView(username, category.Id))
                {
                    categories.Add(category);
                }
            }
            return categories;
        }
        public static IList<Category> GetEditableCategories(string username)
        {
            List<Category> categories = new List<Category>();
            foreach (Category category in Provider.GetAllCategories())
            {
                if (SecurityHelper.CheckWriteAccess(username, category.Id))
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        public static Item GetPublishedItem(string groupId)
        {
            IList<VersionItem> versions = VersionManager.GetAllVersions(groupId);
            List<Item> publishedItems = new List<Item>();
            foreach (VersionItem version in versions)
            {
                Item newsItem = GetItem(version.ItemId);
                ItemApprovalStatus approvalStatus = ItemApprovalStatusManager.GetStatusByName("Published");
                if (newsItem.ApprovalStatus.Id.Equals(approvalStatus.Id))
                    publishedItems.Add(newsItem);
            }
            if (publishedItems.Count > 0)
                return publishedItems.OrderByDescending(x => x.InsertDate).First<Item>();
            else
                return new Item();
        }

        public static IList<Item> GetPublishedItems(Category category, bool recursive)
        {
            return Provider.GetPublishedItems(category, recursive);
        }
        #endregion

        #region News
        public static Item CreateItem(Category category, string owner,
                                        string title, string description,
                                        string url, string urlName,
                                        DateTime newsDate, bool acknowledge, bool approve, string groups, Attachment.FileInfo attachment, DateTime? expiry, ItemApprovalStatus approvalStatus)
        {
            return Provider.CreateItem(category, owner, title, description, url, urlName, newsDate, acknowledge, approve, groups, attachment, expiry, approvalStatus);
        }

        public static IList<Item> GetItems()
        {
            return Provider.GetItems();
        }

        public static IList<Item> GetItems(Category category, bool recursive)
        {
            return Provider.GetItems(category, recursive);
        }

        public static int CountItems(IList<Item> items, string username)
        {
            if (items != null && items.Count > 0)
            {
                IList<VersionItem> vItems = new List<VersionItem>();
                foreach (Item item1 in items)
                {
                    VersionItem v = VersionManager.GetVersionByItemId(item1.Id);
                    if (v != null)
                    {
                        VersionItem latest = new VersionItem();
                        IList<VersionItem> versions = VersionManager.GetAllVersions(v.GroupId);

                        foreach (VersionItem version in versions)
                        {
                            if (VersionManager.IsLatestVersion(version.Id))
                                latest = version;
                        }
                        if (latest == null)
                        {
                            if (SecurityHelper.CanUserEdit(username, item1.Category.Id) || SecurityHelper.CanUserContribute(username, item1.Category.Id))
                            {
                                vItems.Add(v);
                            }
                        }
                        else
                        {
                            Item item2 = NewsManager.GetItem(latest.ItemId);
                            if ((SecurityHelper.CanUserView(username, item2.Category.Id) && item2.ApprovalStatus.Name == "Published")
                            || (SecurityHelper.CanUserContribute(username, item2.Category.Id) && item2.Owner.ToLower() == username.ToLower())
                            || (SecurityHelper.CanUserEdit(username, item2.Category.Id)))
                            {
                                if (item1.Category.Id == item2.Category.Id)
                                    vItems.Add(v);
                            }
                        }
                    }
                }
                IEnumerable<VersionItem> disItems = vItems.Distinct(new KeyEqualityComparer<VersionItem>(x => x.GroupId));

                return disItems.Count();
            }
            return 0;
        }

        private static bool UserCanView(Item item, string username)
        {
            bool canView = false;

            if (SecurityHelper.CanUserEdit(username, item.Category.Id))
            {
                canView = true;
            }
            else
            {
                if (SecurityHelper.CanUserContribute(username, item.Category.Id))
                {
                    if (item.Owner.Equals(username))
                    {
                        canView = true;
                    }
                    else if (SecurityHelper.CanUserView(username, item.Category.Id))
                    {
                        if (item.ApprovalStatus.Name.Equals("Published"))
                        {
                            canView = true;
                        }
                    }
                }
                else if (SecurityHelper.CanUserView(username, item.Category.Id))
                {
                    if (item.ApprovalStatus.Name.Equals("Published"))
                    {
                        canView = true;
                    }
                }
            }
            
            return canView;
        }

        
        public static void UpdateItem(Item item)
        {
            Provider.UpdateItem(item);
        }

        public static void DeleteItem(Item item)
        {
            Provider.DeleteItem(item);
        }

        public static void DeleteItemByGroup(string groupId)
        {
            Provider.DeleteItemByGroup(groupId);
        }
        public static Item GetItem(string id)
        {
            return Provider.GetItem(id);
        }
        public static IList<Item> FindItems(Filter<string> categoryName,
                                    Filter<string> tag,
                                   DateTime? fromDate, DateTime? toDate,
                                   PagingInfo paging)
        {
            return Provider.FindItems(categoryName, tag, fromDate, toDate, paging);
        }

        public static IList<Item> GetItemsByGroup(string groupId)
        {
            return Provider.GetItemsByGroup(groupId);
        }

        public static IList<Item> GetItemsByOwner(string owner)
        {
            return Provider.GetItemsByOwner(owner);
        }

        #endregion

        #endregion

        public static BusiBlocksTreeView GetCategoriesItemsTree(string username)
        {
            return GetCategoriesItemsTree(true, username);
        }

        public static BusiBlocksTreeView GetCategoriesTree(string username)
        {
            return GetCategoriesItemsTree(false, username);
        }

        public static BusiBlocksTreeView GetCategoriesItemsTree(bool includeItems, string username)
        {
            BusiBlocksTreeView tree = new BusiBlocksTreeView { Nodes = new List<BusiBlocksTreeNode>() };
            IList<Category> categories = NewsManager.GetAllCategories();

            var rootCategories = from x in categories where x.ParentCategory == null select x;

            int maxLevel = 100;
            foreach (Category category in rootCategories)
            {
                BusiBlocksTreeNode node = new BusiBlocksTreeNode { Name = category.Name, Id = category.Id, IsFolder = true, ChildNodes = new List<BusiBlocksTreeNode>() };
                if (BusiBlocks.SecurityHelper.CanUserView(username, node.Id))
                {
                    tree.Nodes.Add(node);
                    IList<Item> items = NewsManager.GetItems(category, false);
                    if (items.Count > 0 && includeItems)
                    {
                        foreach (Item item in items)
                        {
                            if (BusiBlocks.SecurityHelper.CanUserView(username, category.Id))
                            {
                                node.ChildNodes.Add(new BusiBlocksTreeNode { Id = item.Id, Name = item.Title, IsFolder = false });
                            }
                        }
                    }
                    CreateCategoryStructure(node, categories, category, maxLevel, 0, includeItems, username);
                }
            }
            return tree;
        }

        private static void CreateCategoryStructure(BusiBlocksTreeNode rootNode, IList<Category> categories, Category category, int maxLevel, int level, bool includeItems, string username)
        {
            if (level > maxLevel)
                return;

            var subCategories = from x in categories where (x.ParentCategory == null) ? 1 == 0 : x.ParentCategory.Equals(category) select x;

            foreach (Category subCategory in subCategories)
            {
                IList<Item> items = NewsManager.GetItems(subCategory, false);

                BusiBlocksTreeNode categoryNode = new BusiBlocksTreeNode { Name = subCategory.Name, Id = subCategory.Id, IsFolder = true, ChildNodes = new List<BusiBlocksTreeNode>() };

                if (items.Count > 0 && includeItems)
                {
                    foreach (Item item in items)
                    {
                        if (BusiBlocks.SecurityHelper.CanUserView(username, subCategory.Id))
                        {
                            categoryNode.ChildNodes.Add(new BusiBlocksTreeNode { Id = item.Id, Name = item.Title, IsFolder = false });
                        }
                    }
                }

                if (BusiBlocks.SecurityHelper.CanUserView(username, categoryNode.Id))
                {
                    rootNode.ChildNodes.Add(categoryNode);
                    CreateCategoryStructure(categoryNode, categories, subCategory, maxLevel, level++, includeItems, username);
                }
            }
        }

        public static TrafficLightStatus GetTrafficLight(string userName, Item item)
        {
            return Provider.GetTrafficLight(userName, item);
        }
    }
}
