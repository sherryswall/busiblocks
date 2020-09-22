using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.Forums
{
    public class Message : IAudit, IOwner, ISearchResult
    {
        protected Message()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTopic"></param>
        /// <param name="pIdParentMessage">Null for the first message</param>
        /// <param name="pOwner"></param>
        /// <param name="pTitle"></param>
        /// <param name="pBody"></param>
        /// <param name="pAttachment"></param>
        public Message(Topic pTopic, string pIdParentMessage,
                    string pOwner, string pTitle,
                    string pBody, Attachment.FileInfo pAttachment)
        {
            Topic = pTopic;
            Owner = pOwner;
            Title = pTitle;
            Body = pBody;
            IdParentMessage = pIdParentMessage;
            Attachment = pAttachment;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual Topic Topic
        {
            get;
            protected set;
        }

        public virtual string Owner
        {
            get;
            protected set;
        }

        public virtual string Title
        {
            get;
            protected set;
        }

        public virtual string Body { get; set; }

        /// <summary>
        /// Field that can be used for user defined extensions.
        /// </summary>
        public virtual string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Null for the first message
        /// </summary>
        public virtual string IdParentMessage
        {
            get;
            protected set;
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
            protected set;
        }

        public string Groups
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }

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
            get 
            {
                XHTMLText xhtml = new XHTMLText();
                xhtml.Load(this.Body);

                return xhtml.GetShortText();
            }
        }

        DateTime ISearchResult.Date
        {
            get { return this.UpdateDate; }
        }

        string ISearchResult.Category
        {
            get { return this.Topic.Category.DisplayName + "\\" + this.Topic.Title; }
        }

       

        #endregion
    }
}
