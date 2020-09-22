using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Configuration;
using System.Linq;

namespace BusiBlocks.DocoBlock
{
    public class DocoManager
    {
        static DocoManager()
        {
            //Get the feature's configuration info
            DocoProviderConfiguration qc =
                (DocoProviderConfiguration)ConfigurationManager.GetSection("DocoManager");

            if (qc == null || qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for DocoManager.");

            //Instantiate the providers
            providerCollection = new DocoProviderCollection();
            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(DocoProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[qc.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the DocoManager.",
                    qc.ElementInformation.Properties["defaultProvider"].Source,
                    qc.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        //Public feature API
        private static DocoProvider defaultProvider;
        private static DocoProviderCollection providerCollection;

        public static DocoProvider Provider
        {
            get { return defaultProvider; }
        }

        public static DocoProviderCollection Providers
        {
            get { return providerCollection; }
        }


        #region Static methods

        #region Category
        public static Category CreateCategory(string displayName)
        {
            return Provider.CreateCategory(displayName);
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
            IList<Category> categories = new List<Category>();

            foreach (Category category in Provider.GetAllCategories())
            {
                if (BusiBlocks.SecurityHelper.CanUserView(username, category.Id) || BusiBlocks.SecurityHelper.CheckWriteAccess(username, category.Id))
                {
                    categories.Add(category);
                }
            }
            return categories;
        }
        public static IList<Category> GetAllCategoriesBelow(string id)
        {
            return Provider.GetAllCategoriesBelow(id);
        }
        #endregion

        #region Articles
        public static Article CreateArticle(Category category, string owner,
                                            string name, string fileName, string title, string description, string body,
                                            bool isUpload, bool isEnabled, bool isNumbChaps, bool isAckRequired)
        {
            return Provider.CreateArticle(category, owner, name, fileName, title, description, body, isUpload, isEnabled, isNumbChaps, isAckRequired);
        }

        public static IList<Article> GetArticles(Category category,
                            ArticleStatus status, bool recursive)
        {
            return Provider.GetArticles(category, status, recursive);
        }

        public static IList<Article> GetArticlesByOwner(Category category,
                            string owner, ArticleStatus status)
        {
            return Provider.GetArticlesByOwner(category, owner, status);
        }

        public static IList<Article> GetAllArticlesByOwner(string owner, ArticleStatus status)
        {
            return Provider.GetAllArticlesByOwner(owner, status);
        }

        public static IList<Article> GetArticlesByGroup(string groupId)
        {
            return Provider.GetArticlesByGroup(groupId);
        }

        public static IList<Article> GetAllArticles()
        {
            return Provider.GetAllArticles();
        }

        /// <summary>
        /// Update the specified article. The current version is incremented if required.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="backupVersion">If true the previous article version is saved as a backup in the VersionedArticle and the current version is incremented.</param>
        public static void UpdateArticle(Article article, bool backupVersion)
        {
            Provider.UpdateArticle(article, backupVersion);
        }

        public static string DeleteArticle(Article article)
        {
            return Provider.DeleteArticle(article);
        }

        public static void DeleteArticleVersion(VersionedArticle article)
        {
            Provider.DeleteArticleVersion(article);
        }

        public static Article GetArticle(string id)
        {
            return Provider.GetArticle(id);
        }

        public static Article GetArticleByName(string name, bool throwIfNotFound)
        {
            return Provider.GetArticleByName(name, throwIfNotFound);
        }

        public static IList<Article> GetArticlesByTitle(string title)
        {
            return Provider.GetArticlesByTitle(title);
        }

        /// <summary>
        /// Returns the specified version of the article. If the version is equal the article.Version then the article is returned.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ArticleBase GetArticleByVersion(Article article, int version)
        {
            return Provider.GetArticleByVersion(article, version);
        }

        /// <summary>
        /// Get a list of article versions (also with the latest version)
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static IList<ArticleBase> GetArticleVersions(Article article)
        {
            return Provider.GetArticleVersions(article);
        }

        public static IList<Article> FindArticles(Filter<string> categoryName,
                                           Filter<string> searchFor,
                                           Filter<string> author,
                                           Filter<string> owner,
                                           Filter<string> tag,
                                           DateTime? fromDate, DateTime? toDate,
                                           ArticleStatus status,
                                           PagingInfo paging)
        {
            return Provider.FindArticles(categoryName, searchFor,
                                        author, owner,
                                        tag,
                                         fromDate, toDate,
                                         status,
                                         paging);
        }

        /// <summary>
        /// Returns the count of articles in the category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageIdentity"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int CountItems(string categoryId, string pageIdentity, string username)
        {            
            int itemCount = 0;
            Category cat = DocoManager.GetCategory(categoryId);

            if (SecurityHelper.CanUserEdit(pageIdentity, categoryId))
            {
                itemCount = DocoManager.GetArticles(cat, ArticleStatus.All, false).Count;
            }
            else if (SecurityHelper.CanUserContribute(pageIdentity, categoryId))
            {
                IList<Article> allAritcles = DocoManager.GetArticles(cat, ArticleStatus.All, false);
                foreach (Article article in allAritcles)
                {
                    if (article.Owner.Equals(pageIdentity))
                        itemCount++;
                }
            }

            else if (SecurityHelper.CanUserView(pageIdentity, categoryId))
            {
                //change this in future to see only the published ones!.
                itemCount = itemCount + DocoManager.GetArticles(cat, ArticleStatus.Approved, false).Count;
            }
            return itemCount;
        }
        #endregion

        #region Attachments
        public static FileAttachment CreateFileAttachment(Article article, string name, string contentType, byte[] contentData)
        {
            return Provider.CreateFileAttachment(article, name, contentType, contentData);
        }

        public static string[] GetFileAttachments(Article article, EnabledStatus enabledStatus)
        {
            return Provider.GetFileAttachments(article, enabledStatus);
        }

        public static void UpdateFileAttachment(FileAttachment attachment)
        {
            Provider.UpdateFileAttachment(attachment);
        }

        public static void DeleteFileAttachment(FileAttachment attachment)
        {
            Provider.DeleteFileAttachment(attachment);
        }

        public static FileAttachment GetFileAttachment(string id)
        {
            return Provider.GetFileAttachment(id);
        }

        public static FileAttachment GetFileAttachmentByName(Article article, string name, bool throwIfNotFound)
        {
            return Provider.GetFileAttachmentByName(article, name, throwIfNotFound);
        }
        #endregion

        #region Chapters
        public static Chapter CreateChapter(string docId, string chapterName)
        {
            return Provider.CreateChapter(docId, chapterName);
        }

        public static void UpdateChapter(Chapter chapter)
        {
            Provider.UpdateChapter(chapter);
        }
        public static ChapterVersion CreateChapterVersion(string docId, ChapterVersion chapterVersion, VersionUpdateType versionUpdateType)
        {
            return Provider.CreateChapterVersion(docId, chapterVersion, versionUpdateType);
        }
        public static void UpdateChapterVersion(ChapterVersion chapterVers)
        {
            Provider.UpdateChapterVersion(chapterVers);
        }
        public static bool CheckChapterName(string docId, string chapterName)
        {
            return Provider.CheckChapterName(docId, chapterName);
        }
        public static int GetLowestSequenceNumber(string articleId)
        {
            return Provider.GetLowestSequenceNumber(articleId);
        }
        public static void DeleteChapter(string chapterId)
        {
            Provider.DeleteChapter(chapterId);
        }
        public static IList<Chapter> GetAllChapters()
        {
            return Provider.GetAllChapters();
        }
        public static IList<Chapter> GetAllChapters(string articleId)
        {
            return Provider.GetAllChapters(articleId);
        }
        public static IList<ChapterVersion> GetAllItemsByArticleId(string articleId)
        {
            return Provider.GetAllItemsByArticleId(articleId);
        }
        public static IList<ChapterVersion> GetAllChapterVersion()
        {
            return Provider.GetAllChapVersions();
        }
        public static ChapterVersion GetChapterVersion(string chapterId)
        {
            return Provider.GetChapterVersion(chapterId);
        }
        public static ChapterVersion GetNextChapter(string chapterId, string articleId)
        {
            return Provider.GetNextChapter(chapterId, articleId);
        }
        public static ChapterVersion GetPreviousChapter(string chapterId, string articleId)
        {
            return Provider.GetPreviousChapter(chapterId, articleId);
        }
        public static IList<string> GetSubChapters(string chapterId)
        {
            return Provider.GetSubChapters(chapterId);
        }
        public static IList<SubSectionChap> GetChapterSubSection(string chapterId)
        {
            return Provider.GetChapterSubSection(chapterId);
        }
        #endregion

        #region Drafts
        public static Draft UpsertDraft(Draft draft)
        {
            return Provider.UpsertDraft(draft);
        }
        public static void DeleteDraft(string chapterVersionId, bool deleteAll, string draftId, string newChapterVersionId)
        {
            Provider.DeleteDraft(chapterVersionId, deleteAll, draftId, newChapterVersionId);
        }
        public static Draft GetDraftByChapterId(string chapterId)
        {
            return Provider.GetDraftByChapterId(chapterId);
        }
        public static IList<Draft> GetDraftsByChapterId(string chapterId)
        {
            return Provider.GetDraftsByChapterId(chapterId);
        }

        #endregion

        #region TreeView

        public static BusiBlocksTreeView GetDocoTree(string username)
        {
            return PopulateTreeView(username);
        }

        private static BusiBlocksTreeView PopulateTreeView(string username)
        {
            var catTreeView = new BusiBlocksTreeView();

            IList<Category> cats = GetAllCategories();

            IList<Category> toRemove = new List<Category>();
            foreach (Category cat in cats)
            {
                // Remove this category from the list if it is not viewable by this user.
                if (!SecurityHelper.CanUserView(username, cat.Id))
                {
                    toRemove.Add(cat);
                }
            }
            foreach (Category cat in toRemove)
            {
                cats.Remove(cat);
            }

            // Need to form the hierarchical structure by selecting the categories with no parent
            // and then adding sub collections of categories with the chosen parent.

            var noParent =
                from x in cats
                where x.ParentCategory == null
                select x;

            int maxLevel = 20;
            if (!noParent.Any())
            {
                // Try to pick the "all docs" category.
                noParent =
                    from x in cats
                    where x.DisplayName.Equals("All Documents")
                    select x;
            }
            foreach (Category cat in noParent)
            {
                var node = new BusiBlocksTreeNode { Id = cat.Id, Name = cat.DisplayName, IsFolder = true };
                IList<Article> items = GetArticles(cat, ArticleStatus.All, false);
                foreach (Article item in items)
                {
                    node.ChildNodes.Add(new BusiBlocksTreeNode { Id = item.Id, Name = item.Name, IsFolder = false });
                }
                catTreeView.Nodes.Add(node);
                PopulateSub(cat, cats, node, maxLevel, 0);
            }
            // todo Remove this commented out block when we're sure it isn't doing anything.
            //// Set the selected category.
            //if (noParent.Any())
            //{
            //    Category cat = noParent.First();
            //}
            return catTreeView;
        }

        private static void PopulateSub(Category cat, IEnumerable<Category> cats, BusiBlocksTreeNode docoCat, int maxLevel, int level)
        {
            if (level > maxLevel)
                return;

            var subCats =
                from x in cats
                where (x.ParentCategory == null) ? 1 == 0 : x.ParentCategory.Equals(cat)
                select x;

            foreach (Category subCat in subCats)
            {
                var node = new BusiBlocksTreeNode { Id = subCat.Id, Name = subCat.DisplayName, IsFolder = true };

                IList<Article> items = GetArticles(subCat, ArticleStatus.All, false);
                foreach (Article item in items)
                {
                    node.ChildNodes.Add(new BusiBlocksTreeNode { Id = item.Id, Name = item.Name, IsFolder = false });
                }
                docoCat.ChildNodes.Add(node);
                level = level + 1;
                PopulateSub(subCat, cats, node, maxLevel, level);
            }
        }
        #endregion

        #endregion
    }
}
