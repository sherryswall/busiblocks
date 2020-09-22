using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public class Category : IAccessControl
    {
        protected Category()
        {

        }

        public Category(string displayName)
        {
            DisplayName = displayName;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual string DisplayName { get; set; }


        public virtual string Description { get; set; }
        

        private string _readPermissions = SecurityHelper.ALL_USERS;
        public virtual string ReadPermissions
        {
            get { return _readPermissions; }
            set { _readPermissions = value; }
        }

        private string _editPermissions = SecurityHelper.NONE;
        public virtual string EditPermissions
        {
            get { return _editPermissions; }
            set { _editPermissions = value; }
        }

        private string _insertPermissions = SecurityHelper.AUTHENTICATED_USERS;
        public virtual string InsertPermissions
        {
            get { return _insertPermissions; }
            set { _insertPermissions = value; }
        }

        private string _deletePermissions = SecurityHelper.NONE;
        public virtual string DeletePermissions
        {
            get { return _deletePermissions; }
            set { _deletePermissions = value; }
        }

        private string _approvePermissions = SecurityHelper.NONE;
        /// <summary>
        /// Gets or sets the roles that are enabled to approve and enable the articles
        /// </summary>
        public virtual string ApprovePermissions
        {
            get { return _approvePermissions; }
            set { _approvePermissions = value; }
        }

        private bool _autoApprove = true;
        /// <summary>
        /// Gets or sets if automatically approve the article when adding it.
        /// If set to true the editor don't need to approve the articles, their are automatically approved.
        /// </summary>
        public virtual bool AutoApprove
        {
            get { return _autoApprove; }
            set { _autoApprove = value; }
        }

        private bool _attachEnabled = true;
        public virtual bool AttachEnabled
        {
            get { return _attachEnabled; }
            set { _attachEnabled = value; }
        }

        private string _attachExtensions = Attachment.FileHelper.EXTENSIONS_ALL;
        /// <summary>
        /// Accepted file name extensions.
        /// </summary>
        public virtual string AttachExtensions
        {
            get { return _attachExtensions; }
            set { _attachExtensions = value; }
        }

        private int _attachMaxSize = 500;
        /// <summary>
        /// Maximum size of the attachment file expressed in kb
        /// </summary>
        public virtual int AttachMaxSize
        {
            get { return _attachMaxSize; }
            set { _attachMaxSize = value; }
        }

        private XHtmlMode _XHtmlMode = XHtmlMode.StrictValidation;
        /// <summary>
        /// Gets or sets the xhtml validation mode
        /// </summary>
        public virtual XHtmlMode XHtmlMode
        {
            get { return _XHtmlMode; }
            set { _XHtmlMode = value; }
        }

        private DocoBackupMode _backupMode = DocoBackupMode.Always;
        /// <summary>
        /// Gets or sets the backup mode
        /// </summary>
        public virtual DocoBackupMode BackupMode
        {
            get { return _backupMode; }
            set { _backupMode = value; }
        }

        public virtual Category ParentCategory { get; set; }

        protected IList<Article> Articles { get; set; }

        public virtual string Groups { get; set; }

        public virtual string Breadcrumb
        {
            get
            {
                string val = DisplayName;
                int breaker = 0;

                Category pCategory = ParentCategory;
                while (pCategory != null && breaker < 30)
                {
                    val = pCategory.DisplayName + @"\" + val;
                    breaker++;
                    pCategory = pCategory.ParentCategory;
                }

                return val;
            }
        }

        public virtual bool Deleted { get; set; }
    }
}
