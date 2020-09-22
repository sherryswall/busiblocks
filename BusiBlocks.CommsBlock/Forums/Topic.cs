using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.Forums
{
    public class Topic : IAudit, IOwner
    {
        protected Topic()
        {

        }

        public Topic(Category pCategory, string pOwner, string pTitle)
        {
            Category = pCategory;
            Owner = pOwner;
            Title = pTitle;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual Category Category
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
        /// List used for cascading rules (delete)
        /// </summary>
        protected IList<Message> Messages
        {
            get;
            set;
        }

        public string Groups
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }
    }
}
