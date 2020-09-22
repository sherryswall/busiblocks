using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    
    public class Article : ArticleBase, ISearchResult, IAudit
    {
        protected Article()
        {

        }

        public Article(Category pCategory, string pName, string pFileName, string pOwner,
                        string pTitle, string pDescription, string pBody, bool isUpload, bool isEnabled, bool isNumbChaps)
            : base(pOwner, pTitle)
        {
            Category = pCategory;
            Name = pName;
            Description = pDescription;
            Body = pBody;
            IsUpload = isUpload;
            Enabled = isEnabled;
            FileName = pFileName;
            NumberedChaps = isNumbChaps;
        }

        private string _name;
        /// <summary>
        /// The unique name of the article item. Must be unique for all the categories
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual Category Category
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if the article is enabled.
        /// A disabled article is visible only by an administrator/editor.
        /// When a user delete an article it became disable.
        /// Only the administrator can delete articles.
        /// </summary>
        public virtual bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if the article is approved.
        /// A not approved article is visible only by an administrator/editor or by the owner.
        /// Only the administrator/editor can approve an article.
        /// When a user submit a new article it must be approved.
        /// </summary>
        public virtual bool Approved
        {
            get;
            set;
        }

        /// <summary>
        /// List used for cascading rules
        /// </summary>
        protected IList<FileAttachment> Attachments
        {
            get;
            set;
        }

        /// <summary>
        /// List used for cascading rules
        /// </summary>
        protected IList<VersionedArticle> Versions
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }

        public virtual bool NumberedChaps
        {
            get;
            set;
        }

        #region ISearchResult Members

        string ISearchResult.Title
        {
            get { return this.Title; }
        }

        string ISearchResult.Owner
        {
            get { return this.Owner; }
        }

        string ISearchResult.Description
        {
            get { return this.Description; }
        }

        DateTime ISearchResult.Date
        {
            get { return this.UpdateDate; }
        }

        string ISearchResult.Category
        {
            get { return this.Category.DisplayName; }
        }

        #endregion
    }
}
