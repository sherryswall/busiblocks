using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.DocoBlock
{
    /// <summary>
    /// DocoProvider abstract class. 
    /// A DocoProvider can be used to store articles, attachments and the relative informations (versions, ...).
    /// 
    /// </summary>
    public abstract class DocoProvider : ProviderBase
    {
        #region Category
        public abstract Category CreateCategory(string displayName);

        public abstract void UpdateCategory(Category category);

        public abstract void DeleteCategory(Category category);

        public abstract Category GetCategory(string id);

        public abstract Category GetCategoryByName(string name, bool throwIfNotFound);

        public abstract IList<Category> GetCategoryByLikeName(string name, bool throwIfNotFound);

        public abstract IList<Category> GetAllCategories();

        public abstract IList<Category> GetAllCategoriesBelow(string id);
        #endregion

        #region Articles
        public abstract Article CreateArticle(Category category, string owner,
                                            string name, string fileName, string title, string description, string body,
                                            bool isUpload, bool isEnabled,bool isNumbChaps, bool isAckRequired);

        public abstract IList<Article> GetArticles(Category category,
                            ArticleStatus status, bool recursive);

        public abstract IList<Article> GetArticlesByOwner(Category category,
                            string owner, ArticleStatus status);

        public abstract IList<Article> GetAllArticlesByOwner(string owner, ArticleStatus status);

        /// <summary>
        /// Update the specified article. The current version is incremented if required.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="backupVersion">If true the previous article version is saved as a backup in the VersionedArticle and the current version is incremented.</param>
        public abstract void UpdateArticle(Article article, bool backupVersion);

        public abstract string DeleteArticle(Article article);

        public abstract void DeleteArticleVersion(VersionedArticle article);

        public abstract Article GetArticle(string id);

        public abstract Article GetArticleByName(string name, bool throwIfNotFound);

        public abstract IList<Article> GetArticlesByTitle(string title);

        public abstract IList<Article> GetArticlesByGroup(string groupId);

        public abstract IList<Article> GetAllArticles();
        

        /// <summary>
        /// Returns the specified version of the article. If the version is equal the article.Version then the article is returned.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public abstract ArticleBase GetArticleByVersion(Article article, int version);

        /// <summary>
        /// Get a list of article versions (also with the latest version)
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public abstract IList<ArticleBase> GetArticleVersions(Article article);

        public abstract IList<Article> FindArticles(Filter<string> categoryName,
                                           Filter<string> searchFor,
                                           Filter<string> author,
                                           Filter<string> owner,
                                           Filter<string> tag, 
                                           DateTime? fromDate, DateTime? toDate,
                                           ArticleStatus status,
                                           PagingInfo paging);
        #endregion

        #region Attachments
        public abstract FileAttachment CreateFileAttachment(Article article, string name, string contentType, byte[] contentData);

        public abstract string[] GetFileAttachments(Article article, EnabledStatus enabledStatus);

        public abstract void UpdateFileAttachment(FileAttachment attachment);

        public abstract void DeleteFileAttachment(FileAttachment attachment);

        public abstract FileAttachment GetFileAttachment(string id);

        public abstract FileAttachment GetFileAttachmentByName(Article article, string name, bool throwIfNotFound);
        #endregion

        #region Chapters
        public abstract Chapter CreateChapter(string docId,string chapterName);

        public abstract ChapterVersion CreateChapterVersion(string docId, ChapterVersion chapterVersion, VersionUpdateType versionUpdateType);

        public abstract void UpdateChapter(Chapter chapter);

        public abstract void UpdateChapterVersion(ChapterVersion chapterVers);

        public abstract bool CheckChapterName(string docId, string chapterName);

        public abstract int GetLowestSequenceNumber(string articleId);

        public abstract void DeleteChapter(string chapterId);

        public abstract IList<Chapter> GetAllChapters();

        public abstract IList<Chapter> GetAllChapters(string articleId);

        public abstract ChapterVersion GetChapterVersion(string chapterId);

        public abstract ChapterVersion GetNextChapter(string chapterId,string articleId);

        public abstract ChapterVersion GetPreviousChapter(string chapterId, string articleId);

        public abstract IList<string> GetSubChapters(string chapterId);

        public abstract IList<SubSectionChap> GetChapterSubSection(string chapterId);

        public abstract IList<ChapterVersion> GetAllItemsByArticleId(string articleId);

        public abstract IList<ChapterVersion> GetAllChapVersions();

        #endregion

        #region Drafts
        public abstract Draft UpsertDraft(Draft draft);
        public abstract void DeleteDraft(string chapterVersionId, bool deleteAll, string draftId, string newChapterVersionId);
        public abstract Draft GetDraftByChapterId(string chapterId);
        public abstract IList<Draft> GetDraftsByChapterId(string chapterId); 

        #endregion
    }
}
