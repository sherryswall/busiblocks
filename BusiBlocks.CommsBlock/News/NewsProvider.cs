using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.ItemApprovalStatusLayer;

namespace BusiBlocks.CommsBlock.News
{
    /// <summary>
    /// NewsProvider abstract class. 
    /// A NewsProvider can be used to store news.
    /// 
    /// </summary>
    public abstract class NewsProvider : ProviderBase
    {
        #region Category
        public abstract Category CreateCategory(string name);

        public abstract void UpdateCategory(Category category);

        public abstract void DeleteCategory(Category category);

        public abstract void DeleteItemByGroup(string groupId);

        public abstract Category GetCategory(string id);

        public abstract IList<Category> GetCategories(string rootCategoryId, bool recursive);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwIfNotFound">True to throw an exception if the entity is not found. If false and the entity is not found return false</param>
        /// <returns></returns>
        public abstract Category GetCategoryByName(string name, bool throwIfNotFound);

        public abstract IList<Category> GetCategoryByLikeName(string name, bool throwIfNotFound);

        public abstract IList<Category> GetAllCategories();
        #endregion

        #region Items
        public abstract Item CreateItem(Category category, string owner,
                                        string title, string description,
                                        string url, string urlName,
                                        DateTime newsDate, bool acknowledge, bool approve, string groups, Attachment.FileInfo attachment, DateTime? expiry, ItemApprovalStatus approvalStatus);

        public abstract IList<Item> GetItems();

        public abstract IList<Item> GetItems(Category category, bool recursive);

        public abstract IList<Item> GetPublishedItems(Category category, bool recursive);

        public abstract void UpdateItem(Item item);

        public abstract void DeleteItem(Item item);

        public abstract Item GetItem(string id);

        public abstract IList<Item> GetItemsByOwner(string id);

        public abstract IList<Item> FindItems(Filter<string> categoryName,
                                            Filter<string> tag, 
                                           DateTime? fromDate, DateTime? toDate,
                                           PagingInfo paging);

        public abstract IList<Item> GetItemsByGroup(string groupId);

        public abstract TrafficLightStatus GetTrafficLight(string userName, Item item);

        #endregion
    }
}
