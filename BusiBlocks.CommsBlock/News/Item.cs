using System;
using System.Collections.Generic;
using System.Web;
using BusiBlocks.Audit;
using BusiBlocks;
using BusiBlocks.Versioning;
using BusiBlocks.ItemApprovalStatusLayer;

namespace BusiBlocks.CommsBlock.News
{
    [Serializable]
    public class Item : IOwner, IAudit
    {
        public Item()
        {
        }

        public Item(Category category, string owner,
                        string title, string description,
                        string url, string urlName, DateTime newsDate, DateTime? expiry,ItemApprovalStatus approvalStatus)
        {
            Owner = owner;
            Title = title;
            Author = owner;
            Category = category;
            Description = description;
            URL = url;
            URLName = urlName;
            NewsDate = newsDate;
            Expiry = expiry;
            ApprovalStatus = approvalStatus;
            
        }

        public virtual string Id
        {
            get;
            protected set;
        }
        public virtual Category Category
        {
            get;
            set;
        }
        public virtual VersionItem Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original user that has create the article and that has the ownership of it
        /// </summary>
        public virtual string Owner
        {
            get;
            protected set;
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

        public virtual string URL
        {
            get;
            set;
        }

        public virtual string URLName
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

        public virtual DateTime NewsDate
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

        public virtual Attachment.FileInfo Attachment
        {
            get;
            set;
        }

        /// <summary>
        /// Field that can be used for user defined extensions.
        /// </summary>
        public virtual string Tag
        {
            get;
            set;
        }

        public virtual string Groups
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }

        public virtual bool Acknowledged
        {
            get
            {
                BusiBlocks.ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

                using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
                {
                    AuditDataStore store = new AuditDataStore(transaction);
                    return store.AuditRecordExist(HttpContext.Current.User.Identity.Name, Id, AuditRecord.AuditAction.Acknowledged.ToString());
                }
            }
        }

        public virtual bool HasUserActioned(string username, AuditRecord.AuditAction action)
        {
            BusiBlocks.ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
            {
                AuditDataStore store = new AuditDataStore(transaction);
                return store.AuditRecordExist(username, Id, action.ToString());
            }
        }

        public virtual bool Viewed
        {
            get
            {
                BusiBlocks.ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

                using (BusiBlocks.TransactionScope transaction = new BusiBlocks.TransactionScope(parameters))
                {
                    AuditDataStore store = new AuditDataStore(transaction);
                    return store.AuditRecordExist(HttpContext.Current.User.Identity.Name, Id, AuditRecord.AuditAction.Viewed.ToString());
                }
            }
        }

        public virtual bool RequiresAck
        {
            get
            {
                if (string.IsNullOrEmpty(Tag))
                    return false;
                return Convert.ToBoolean(Tag.Split(':')[0]);
            }
        }

        public virtual bool RequiresApproval
        {
            get
            {
                if (string.IsNullOrEmpty(Tag))
                    return false;
                string[] split = Tag.Split(':');
                if (split.Length > 1)
                    return Convert.ToBoolean(split[1]);
                else return false;
            }
        }

        public virtual DateTime? Expiry
        {
            get;
            set;
        }

        private ItemApprovalStatus _approvalStatus;
        public virtual ItemApprovalStatus ApprovalStatus
        {
            get
            {
                if (_approvalStatus == null)
                    return ItemApprovalStatusManager.GetDraftStatus();
                else
                    return _approvalStatus;
            }
            set { _approvalStatus = value; }
        }

        public virtual string ActionedByPersonId
        {
            get;
            set;
        }

        public virtual DateTime? ActionedOnDate
        {
            get;
            set;
        }

        public virtual string ActionedNotes
        {
            get;
            set;
        }
    }
    [Serializable]
    public class NewsGridItem
    {
        public VersionItem Draft { get; set; }
        public Item NewsItem { get; set; }

        // todo store only the attributes the grid uses.
        //public string Id { get; set; }
        //public string CategoryId { get; set; }
        //public string CategoryName { get; set; }
        //public string Title { get; set; }
        //public string Deleted { get; set; }
        //public string ApprovalStatusName { get; set; }
        //public string ApprovalStatusId { get; set; }
        //public string UpdateDate { get; set; }
        //public string Owner { get; set; }
        //public string Author { get; set; }
        //public string DraftVersion { get; set; }
        //public string DraftDateCreated { get; set; }
        public string TrafficLightUrl { get; set; }
    }
}
