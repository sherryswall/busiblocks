using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public class FileAttachment : Attachment.FileInfo
    {
        protected FileAttachment()
        {
        }

        public FileAttachment(Article pArticle, string name, string contentType, byte[] contentData)
            :base(name, contentType, contentData)
        {
            Article = pArticle;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual Article Article
        {
            get;
            protected set;
        }

        private bool _enabled = true;
        /// <summary>
        /// Gets or sets if the attachment is enabled.
        /// A disabled attachment is visible only by an administrator/editor.
        /// When a user delete an attachment it became disabled.
        /// Only the administrator can delete attachments.
        /// </summary>
        public virtual bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public virtual bool Deleted { get; set; }
    }
}
