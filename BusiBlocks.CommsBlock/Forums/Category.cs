using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.Forums
{
    public class Category : IAccessControl
    {
        public Category()
        {

        }

        public Category(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }


        public virtual string Id
        {
            get;
            protected set;
        }

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set 
            {
                BusiBlocks.EntityHelper.ValidateCode("Name", value);
                _name = value;
            }
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

        /// <summary>
        /// List used for cascading rules (delete)
        /// </summary>
        protected IList<Topic> Topics { get; set; }

        public virtual string Groups { get; set; }

        public virtual bool Deleted { get; set; }
    }
}
