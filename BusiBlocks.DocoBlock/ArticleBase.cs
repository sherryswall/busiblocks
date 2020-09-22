using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BusiBlocks.Audit;

namespace BusiBlocks.DocoBlock
{
    public abstract class ArticleBase : IOwner
    {
        protected ArticleBase()
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public ArticleBase(ArticleBase other)
        {
            Owner = other.Owner;
            Title = other.Title;
            Description = other.Description;
            Body = other.Body;
            TOC = other.TOC;
            Version = other.Version;
            Author = other.Author;
            Tag = other.Tag;
            UpdateUser = other.UpdateUser;
            UpdateDate = other.UpdateDate;
            InsertDate = other.InsertDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOwner">The owner is the same of the update user when creating the article</param>
        /// <param name="pTitle"></param>
        public ArticleBase(string pOwner, string pTitle)
        {
            Owner = pOwner;
            Title = pTitle;
            UpdateUser = pOwner;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual int Version
        {
            get;
            set;
        }

        public virtual void IncrementVersion()
        {
            Version++;
        }

        /// <summary>
        /// Gets or sets the original user that has create the article and that has the ownership of it
        /// </summary>
        public virtual string Owner
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a free text field used to store the update user informations (last update)
        /// </summary>
        public virtual string UpdateUser
        {
            get;
            set;
        }

        public virtual string Title
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string Body
        {
            get;
            set;
        }

        public virtual bool IsUpload 
        { 
            get; 
            set; 
        }

        public virtual string DocumentType
        {
            get
            {
                return Utility.GetImageUrlType(this.FileName.Substring(this.FileName.IndexOf(".") +1), IsUpload);
            }
        }

        public virtual string FileName 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the table of contents
        /// </summary>
        public virtual string TOC
        {
            get;
            set;
        }
        

        /// <summary>
        /// Gets or sets a free text field used to store the author informations (that can be different from the owner)
        /// </summary>
        public virtual string Author
        {
            get;
            set;
        }

        public virtual DateTime InsertDate
        {
            get;
            set;
        }

        public virtual DateTime UpdateDate
        {
            get;
            set;
        }

        /// <summary>
        /// Is used to store the RequiresAck state, until we modify the schema.
        /// </summary>
        public virtual string Tag
        {
            get;
            set;
        }

        public string Groups
        {
            get;
            set;
        }

        public virtual string TrafficLight
        {
            get
            {
                return Utility.GetTrafficLight(this);
            }
        }

        public virtual bool Acknowledged
        {
            get
            {
                BusiBlocks.ConnectionParameters parameters = BusiBlocks.ConnectionParameters.Create("DefaultDB");

                using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
                {
                    AuditDataStore store = new AuditDataStore(transaction);
                    return store.AuditRecordExist(HttpContext.Current.User.Identity.Name, Id, "Acknowledged");
                }
            }
        }

        public virtual bool Viewed
        {
            get
            {
                BusiBlocks.ConnectionParameters parameters = BusiBlocks.ConnectionParameters.Create("DefaultDB");

                using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
                {
                    AuditDataStore store = new AuditDataStore(transaction);
                    return store.AuditRecordExist(HttpContext.Current.User.Identity.Name, Id, "Viewed");
                }
            }
        }

        public virtual bool HasUserActioned(string username, AuditRecord.AuditAction action)
        {
            BusiBlocks.ConnectionParameters parameters = BusiBlocks.ConnectionParameters.Create("DefaultDB");

            using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
            {
                AuditDataStore store = new AuditDataStore(transaction);
                return store.AuditRecordExist(username, Id, action.ToString());
            }
        }

        public virtual bool RequiresAck
        {
            get
            {
                if (Tag != null)
                    return bool.Parse(Tag);
                else
                    return false;
            }
            set
            {
                if (value != null)
                    Tag = value.ToString();
            }
        }
    }
}
