using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Configuration;
using System.Net.Mail;
using System.Linq;
using BusiBlocks.AccessLayer;
namespace BusiBlocks.DocoBlock
{
    /// <summary>
    /// Implementation of DocoProvider that use NHibernate to save and retrive informations.
    ///
    /// Configuration:
    /// connectionStringName = the name of the connection string to use
    /// 
    /// </summary>
    public class BusiBlocksDocoProvider : DocoProvider
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

        #endregion

        #region methods

        #region Category
        public override Category CreateCategory(string displayName)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);

                Category category = new Category(displayName);

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
                if (dataStore.FindByChildOfCategory(category.Id).Count > 0)
                    throw new SchemaIntegrityException("Category has child categories");

                // 2. There are no articles associated with this category.
                ArticleDataStore ads = new ArticleDataStore(transaction);
                if (ads.FindByCategory(category).Count > 0)
                    throw new SchemaIntegrityException("Articles are associated with this category");

                // Delete all access items.
                AccessLayer.AccessDataStore accessDs = new AccessLayer.AccessDataStore(transaction);
                IList<AccessLayer.Access> allAccess = accessDs.FindAllByItem(category.Id);

                foreach (AccessLayer.Access access in allAccess)
                {
                    access.Deleted = true;
                    accessDs.Update(access);
                }

                category.Deleted = true;
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
                    throw new DocoCategoryNotFoundException(id);

                return category;
            }
        }

        public override Category GetCategoryByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                Category category = dataStore.FindByName(name);
               
                return category;
            }
        }

        public override IList<Category> GetCategoryByLikeName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                IList<Category> categories = dataStore.FindByLikeName(name);
              
                return categories;
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

        public override IList<Category> GetAllCategoriesBelow(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                return dataStore.FindByChildOfCategory(id);
            }
        }

        #endregion

        #region Articles

        public override Article CreateArticle(Category category, string owner,
                                            string name, string fileName, string title,
                                            string description, string body, bool isUpload, bool isEnabled, bool isNumbChaps,
                                            bool isAckRequired)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore articleStore = new ArticleDataStore(transaction);
                if (articleStore.FindByName(name) != null)
                    throw new ArticleNameAlreadyExistsException(name);

                CategoryDataStore dataStore = new CategoryDataStore(transaction);
                dataStore.Attach(category);

                Article article = new Article(category, name, fileName, owner, title, description, body, isUpload, isEnabled, isNumbChaps);
                article.Author = owner;
                //article.Tag = isAckRequired.ToString();
                article.RequiresAck = isAckRequired;

                if (category.AutoApprove)
                    article.Approved = true;

                articleStore.Insert(article);
                transaction.Commit();

                return article;
            }
        }
        
        public override IList<Article> GetArticles(Category category, ArticleStatus status, bool recursive)
        {
            IList<Article> articles = null;
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                articles = dataStore.FindByCategoryAndOwner(category, null, status);
                if (recursive)
                {
                    // Find children of this parent category.
                    CategoryDataStore ds = new CategoryDataStore(transaction);
                    IList<Category> childCategories = ds.FindByChildOfCategory(category.Id);
                    foreach (Category subCategory in childCategories)
                    {
                        IList<Article> subArticles = dataStore.FindByCategoryAndOwner(subCategory, null, status);
                        foreach (Article subArticle in subArticles)
                        {
                            articles.Add(subArticle);
                        }
                    }
                }
            }
            return articles;
        }

        public override IList<Article> GetArticlesByOwner(Category category,
                            string owner, ArticleStatus status)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                return dataStore.FindByCategoryAndOwner(category, owner, status);
            }
        }

        public override IList<Article> GetArticlesByGroup(string groupId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                return dataStore.FindByGroup(groupId);
            }
        }

        /// <summary>
        /// Update the specified article. Increment the version if required.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="backupVersion">If true the previous article version is saved as a backup in the VersionedArticle and the current version is incremented.</param>
        public override void UpdateArticle(Article article, bool backupVersion)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);

                VersionedArticle versionedArticle = null;

                if (backupVersion)
                {
                    //Retrive the previous version (before saving the new instance) and save a versioned row

                    Article prevVersion = dataStore.FindByKey(article.Id);
                    if (prevVersion == null)
                        throw new ArticleNotFoundException(article.Id);

                    versionedArticle = new VersionedArticle(prevVersion);

                    VersionedArticleDataStore versionedStore = new VersionedArticleDataStore(transaction);
                    versionedStore.Insert(versionedArticle);

                    //Increment the current article version
                    article.IncrementVersion();
                }

                //flag the entity to be updated and attach the entity to the db
                // I must use InsertOrUpdateCopy because if backupVersion = true there is already a 
                // persistent entity in the session and I must copy the values to this instance. The Update method in this case throw an exception
                article = dataStore.InsertOrUpdateCopy(article);

                transaction.Commit();
            }
        }

        /// <summary>
        /// Deletes the specified article.
        /// </summary>
        /// <param name="article"></param>
        public override string DeleteArticle(Article article)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                article.Deleted = true;
                article.Name += DateTimeHelper.GetCurrentTimestamp();
                dataStore.Update(article);

                transaction.Commit();
                return article.Category.Id;
            }
        }

        public override void DeleteArticleVersion(VersionedArticle article)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                VersionedArticleDataStore dataStore = new VersionedArticleDataStore(transaction);
                article.Deleted = true;
                dataStore.Update(article);
                transaction.Commit();
            }
        }

        public override Article GetArticle(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);

                Article article = dataStore.FindByKey(id);
                if (article == null)
                    throw new ArticleNotFoundException(id);

                return article;
            }
        }

        public override Article GetArticleByName(string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);

                Article article = dataStore.FindByName(name);
                if (article == null && throwIfNotFound)
                    throw new ArticleNotFoundException(name);
                else if (article == null)
                    return null;

                return article;
            }
        }

        public override IList<Article> GetArticlesByTitle(string title)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                return dataStore.FindByTitle(title);
            }
        }

        /// <summary>
        /// Returns the specified version of the article. If the version is equal the article.Version then the article is returned.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public override ArticleBase GetArticleByVersion(Article article, int version)
        {
            if (article.Version == version)
                return article;

            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                VersionedArticleDataStore dataStore = new VersionedArticleDataStore(transaction);

                VersionedArticle versionedArticle = dataStore.FindByArticleVersion(article, version);
                if (versionedArticle == null)
                    throw new ArticleNotFoundException(article.Name + " " + version.ToString());

                return versionedArticle;
            }
        }

        /// <summary>
        /// Get a list of article versions (also with the latest version)
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public override IList<ArticleBase> GetArticleVersions(Article article)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                VersionedArticleDataStore dataStore = new VersionedArticleDataStore(transaction);

                IList<VersionedArticle> versionedArticles = dataStore.GetArticleVersions(article);

                List<ArticleBase> list = new List<ArticleBase>();

                //Add the latest version
                list.Add(article);

                //add all the other versions
                foreach (VersionedArticle verArticle in versionedArticles)
                    list.Add(verArticle);

                return list;
            }
        }

        public override IList<Article> FindArticles(Filter<string> categoryName,
                                           Filter<string> searchFor,
                                           Filter<string> author,
                                           Filter<string> owner,
                                           Filter<string> tag,
                                           DateTime? fromDate, DateTime? toDate,
                                           ArticleStatus status,
                                           PagingInfo paging)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);

                return dataStore.FindByFields(categoryName, searchFor,
                                                author, owner,
                                                tag,
                                                fromDate, toDate,
                                                status,
                                                paging);
            }
        }

        public override IList<Article> GetAllArticlesByOwner(string owner, ArticleStatus status)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);

                return dataStore.FindByOwner(owner, status);
            }
        }

        public override IList<Article> GetAllArticles()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dsArticle = new ArticleDataStore(transaction);
                return dsArticle.FindAllArticles();
            }
        }


        #endregion

        #region Attachments
        public override FileAttachment CreateFileAttachment(Article article, string name, string contentType, byte[] contentData)
        {
            FileAttachment attachment = new FileAttachment(article, name, contentType, contentData);

            //Check attachment
            if (attachment != null)
                Attachment.FileHelper.CheckFile(attachment, article.Category.AttachExtensions, article.Category.AttachMaxSize);

            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ArticleDataStore dataStore = new ArticleDataStore(transaction);
                dataStore.Attach(article);

                FileAttachmentDataStore attachmentStore = new FileAttachmentDataStore(transaction);
                attachmentStore.Insert(attachment);

                transaction.Commit();

                return attachment;
            }
        }

        public override string[] GetFileAttachments(Article article, EnabledStatus enabledStatus)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FileAttachmentDataStore dataStore = new FileAttachmentDataStore(transaction);
                return dataStore.GetArticleAttachments(article, enabledStatus);
            }
        }

        public override void UpdateFileAttachment(FileAttachment attachment)
        {
            //Check attachment
            if (attachment != null)
                Attachment.FileHelper.CheckFile(attachment, attachment.Article.Category.AttachExtensions, attachment.Article.Category.AttachMaxSize);

            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FileAttachmentDataStore dataStore = new FileAttachmentDataStore(transaction);
                dataStore.Update(attachment);
                transaction.Commit();
            }
        }

        public override void DeleteFileAttachment(FileAttachment attachment)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FileAttachmentDataStore dataStore = new FileAttachmentDataStore(transaction);
                attachment.Deleted = true;
                dataStore.Update(attachment);
                transaction.Commit();
            }
        }

        public override FileAttachment GetFileAttachment(string id)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FileAttachmentDataStore dataStore = new FileAttachmentDataStore(transaction);

                FileAttachment attachment = dataStore.FindByKey(id);
                if (attachment == null)
                    throw new FileAttachNotFoundException(id);

                return attachment;
            }
        }

        public override FileAttachment GetFileAttachmentByName(Article article, string name, bool throwIfNotFound)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                FileAttachmentDataStore dataStore = new FileAttachmentDataStore(transaction);

                FileAttachment attachment = dataStore.FindByArticleVersion(article, name);
                if (attachment == null && throwIfNotFound)
                    throw new FileAttachNotFoundException(article.Name + "." + name);
                else if (attachment == null)
                    return null;

                return attachment;
            }
        }
        #endregion

        #region Chapter
        public override Chapter CreateChapter(string docId, string chapterName)
        {
            if (!CheckChapterName(docId, chapterName))
            {
                using (TransactionScope transaction = new TransactionScope(mConfiguration))
                {
                    ChapterDataStore dataStore = new ChapterDataStore(transaction);
                    Chapter chapter = new Chapter(docId);

                    dataStore.Insert(chapter);
                    transaction.Commit();

                    return chapter;
                }
            }
            return new Chapter();
        }

        public override ChapterVersion CreateChapterVersion(string docId, ChapterVersion chapterVersion, VersionUpdateType versionUpdateType)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore dataStore = new ChapterVersionDataStore(transaction);


                int maxSequenceNumber = 0;

                if (versionUpdateType == VersionUpdateType.New)
                    maxSequenceNumber = GetNewSequenceNo(docId); //only generate sequence number when adding a new chapter else use existing sequence number
                else
                    maxSequenceNumber = chapterVersion.Sequence;

                chapterVersion.Version = GenerateVersionNumber(versionUpdateType, chapterVersion.Version);
                chapterVersion.VersionOrder = GetVersionOrder(docId, chapterVersion.ChapterId);

                //if (!CheckChapterName(docId, chapterVersion.Name))
                // {
                ChapterVersion chapterVer = new ChapterVersion(docId, chapterVersion.ChapterId, chapterVersion.Name, chapterVersion.Content, chapterVersion.Version, chapterVersion.VersionOrder, maxSequenceNumber);

                dataStore.Insert(chapterVer);
                transaction.Commit();

                return chapterVer;
                //}
                //else
                //  return chapterVersion;
            }
        }

        public override bool CheckChapterName(string docId, string chapterName)
        {
            return CheckChapterNameExists(docId, chapterName);
        }

        public string GenerateVersionNumber(VersionUpdateType updateType, string chapVers)
        {
            string versionNumber = string.Empty;
            int partA = 0; //this the first part indicating any changes to chapter Name or Order
            int partB = 0; //this the second part indicating any changes to chapter content;
            //if changes made to Chapter name or document's chapter collection then increment first part of version

            partA = Int32.Parse(chapVers.Substring(0, 1));
            partB = Int32.Parse(chapVers.Substring(chapVers.IndexOf(".") + 1, chapVers.Length - (chapVers.IndexOf(".") + 1)));

            if (updateType == VersionUpdateType.Content)
            {
                partB++;
            }
            else if (updateType == VersionUpdateType.ChapterName)//if changes made to chapter content then increment second part of version.
            {
                partA++;
            }

            else if (updateType == VersionUpdateType.New)
                return chapVers;

            versionNumber = partA.ToString() + "." + partB.ToString();
            return versionNumber;
        }

        public int GetNewSequenceNo(string docId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                IList<ChapterVersion> chapList = DocoManager.GetAllItemsByArticleId(docId);

                var list = from ch in chapList
                           select ch;

                List<ChapterVersion> listChapVer = list.ToList<ChapterVersion>();

                return GetMaxNumber(listChapVer, "sequence");
            }
        }

        public bool CheckChapterNameExists(string docId, string chapterName)
        {

            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                IList<ChapterVersion> chapList = DocoManager.GetAllItemsByArticleId(docId);

                var list = from ch in chapList
                           where ch.Name.ToLower() == chapterName.ToLower()
                           select ch;

                List<ChapterVersion> listChapVer = list.ToList<ChapterVersion>();

                if (listChapVer.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public int GetVersionOrder(string docId, string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                IList<ChapterVersion> chapList = DocoManager.GetAllItemsByArticleId(docId);

                var list = from ch in chapList
                           where ch.ChapterId == chapterId
                           select ch;
                List<ChapterVersion> listChapVer = list.ToList<ChapterVersion>();

                return GetMaxNumber(listChapVer, "version");

            }
        }

        public int GetMaxNumber(List<ChapterVersion> list, string maxNumberType)
        {
            int maxSeq = 0;
            int tempMaxSeq = 0;

            if (list.Count<ChapterVersion>() > 0)
            {
                foreach (ChapterVersion c in list)
                {
                    if (maxNumberType.Equals("version"))
                        tempMaxSeq = c.VersionOrder;
                    else
                        tempMaxSeq = c.Sequence;

                    if (tempMaxSeq > maxSeq)
                        maxSeq = tempMaxSeq;
                }
                return (maxSeq + 1);
            }
            else
                return 0;
        }

        public override void UpdateChapter(Chapter chapter)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterDataStore dataStore = new ChapterDataStore(transaction);
                dataStore.Update(chapter);
                transaction.Commit();
            }
        }

        public override void UpdateChapterVersion(ChapterVersion chapterVers)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore dataStore = new ChapterVersionDataStore(transaction);

                dataStore.Update(chapterVers);

                transaction.Commit();
            }
        }

        public override void DeleteChapter(string chapterId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(mConfiguration))
                {
                    ChapterDataStore dataStore = new ChapterDataStore(transaction);
                    dataStore.Delete(chapterId);

                    ChapterVersionDataStore dsChapVersion = new ChapterVersionDataStore(transaction);
                    IList<ChapterVersion> chVersionsList = dsChapVersion.FindAllItems(chapterId);

                    foreach (ChapterVersion item in chVersionsList)
                    {
                        item.Deleted = true;
                        dsChapVersion.Update(item);
                    }

                    transaction.Commit();
                }

            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public override IList<Chapter> GetAllChapters()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterDataStore chapter = new ChapterDataStore(transaction);
                return chapter.FindAllItems();
            }
        }

        public override IList<Chapter> GetAllChapters(string articleId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterDataStore chapter = new ChapterDataStore(transaction);
                return chapter.FindAllItems(articleId);
            }
        }

        public override ChapterVersion GetChapterVersion(string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                return chapVersion.FindAllItems().First(x => x.Id.Equals(chapterId));
            }
        }

        public override ChapterVersion GetNextChapter(string chapterId, string articleId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                IList<ChapterVersion> chVersList = chapVersion.FindAllItemsByArticleId(articleId);

                int seq = 0;

                if (!string.IsNullOrEmpty(chapterId))  // if this is null then the first chapter is loaded by default.
                    seq = chVersList.IndexOf(chVersList.First(x => x.Id.Equals(chapterId)));

                if (seq < chVersList.Count)
                {
                    if (seq != (chVersList.Count - 1))
                        return chVersList.ElementAt(seq + 1);
                    else
                        return new ChapterVersion();
                }
                else
                    return new ChapterVersion();

            }
        }

        public override ChapterVersion GetPreviousChapter(string chapterId, string articleId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                IList<ChapterVersion> chVersList = chapVersion.FindAllItemsByArticleId(articleId);

                int seq = 0;

                if (!string.IsNullOrEmpty(chapterId))  // if this is null then the first chapter is loaded by default.
                    seq = chVersList.IndexOf(chVersList.First(x => x.Id.Equals(chapterId)));

                if (seq == 0)
                    return new ChapterVersion();
                else
                {
                    if (seq != 0)
                        return chVersList.ElementAt(seq - 1);
                    else
                        return new ChapterVersion();
                }
            }
        }

        public override IList<ChapterVersion> GetAllChapVersions()
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                return chapVersion.FindAllItems();
            }
        }

        public override IList<string> GetSubChapters(string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                ChapterVersion cv = chapVersion.FindAllItems().First(x => x.Id.Equals(chapterId));
                XHTMLText xhtmlText = new XHTMLText();

                IList<string> subChapters = xhtmlText.GetSubChapters(cv.Content);

                return subChapters;
            }
        }

        public override IList<SubSectionChap> GetChapterSubSection(string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                ChapterVersion cv = chapVersion.FindAllItems().First(x => x.Id.Equals(chapterId));
                XHTMLText xhtmlText = new XHTMLText();

                IList<string> subChapters = xhtmlText.GetSubChapters(cv.Content);

                List<SubSectionChap> chapSections = new List<SubSectionChap>();
                foreach (string subSec in subChapters)
                {
                    chapSections.Add(new SubSectionChap { ChapId = chapterId, AnchorTag = subSec });
                }
                return chapSections;
            }
        }

        public override IList<ChapterVersion> GetAllItemsByArticleId(string articleId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);

                IList<ChapterVersion> dsChapters = chapVersion.FindAllItemsByArticleId(articleId);

                return dsChapters;
            }
        }

        public override int GetLowestSequenceNumber(string articleId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                ChapterVersionDataStore chapVersion = new ChapterVersionDataStore(transaction);
                return chapVersion.LowestSeqNumber(articleId);
            }
        }

        #endregion

        #region Draft

        public override Draft UpsertDraft(Draft draft)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(mConfiguration))
                {
                    DraftDataStore dataStore = new DraftDataStore(transaction);

                    IList<Draft> listDrafts = dataStore.FindItemByChapterId(draft.VersionId);
                    if (listDrafts.Count > 0)
                    {
                        string tempContent = draft.Content;
                        draft = listDrafts.First<Draft>();
                        draft.Content = tempContent;
                        dataStore.Update(draft);
                    }
                    else
                        dataStore.Insert(draft);

                    transaction.Commit();

                    return draft;
                }
            }
            catch (NotImplementedException ex)
            {
                throw ex;
            }
        }

        public override void DeleteDraft(string chapterVersionId, bool deleteAll, string draftId, string newChapterVersionId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(mConfiguration))
                {
                    DraftDataStore dataStore = new DraftDataStore(transaction);

                    //items are returned in sorted order - Sorted by SaveDate desc.
                    //delete only one record
                    if (!deleteAll)
                    {

                        if (!string.IsNullOrEmpty(draftId))
                        {
                            Draft draft = new Draft();
                            draft = dataStore.FindItemByDraftId(draftId).First(x => x.Id.Equals(draftId));

                            draft.Deleted = true;
                            dataStore.Update(draft);
                        }

                        //update previous drafts if any

                        IList<Draft> drafts = dataStore.FindItemByChapterId(chapterVersionId);
                        if (drafts.Count > 0)
                        {
                            foreach (Draft item in drafts)
                            {
                                item.VersionId = newChapterVersionId;
                                dataStore.Update(item);
                            }
                        }
                    }
                    //delete all previous records. 
                    else
                    {
                        IList<Draft> drafts = dataStore.FindItemByChapterId(chapterVersionId);

                        if (drafts.Count > 0)
                        {
                            foreach (Draft item in drafts)
                            {
                                if (!string.IsNullOrEmpty(newChapterVersionId))
                                {
                                    if (item.Id != draftId)
                                    {
                                        item.Deleted = true;
                                        dataStore.Update(item);
                                    }
                                }
                                else
                                {
                                    item.Deleted = true;
                                    dataStore.Update(item);
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Draft GetDraftByChapterId(string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                DraftDataStore dsDraft = new DraftDataStore(transaction);
                IList<Draft> drafts = dsDraft.FindItemByChapterId(chapterId);

                if (drafts.Count > 0)
                    return drafts.First<Draft>();
                else
                    return null;
            }
        }

        public override IList<Draft> GetDraftsByChapterId(string chapterId)
        {
            using (TransactionScope transaction = new TransactionScope(mConfiguration))
            {
                DraftDataStore dsDraft = new DraftDataStore(transaction);
                IList<Draft> drafts = dsDraft.FindItemByChapterId(chapterId);
                return drafts;
            }
        }

        #endregion

        #endregion
    }
}
