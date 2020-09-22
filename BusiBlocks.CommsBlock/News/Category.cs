using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.News
{
   [Serializable]
    public class Category : IAccessControl
    {
        protected Category()
        {
        }

        public Category(string name)
        {
            Name = name;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

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

        protected IList<Item> Items
        {
            get;
            set;
        }

        public virtual string Groups { get; set; }

        public virtual Category ParentCategory { get; set; }

        public virtual string Breadcrumb
        {
            get
            {
                string val = Name;
                int breaker = 0;

                Category pCategory = ParentCategory;
                while (pCategory != null && breaker < 30)
                {
                    val = pCategory.Name + @"\" + val;
                    breaker++;
                    pCategory = pCategory.ParentCategory;
                }

                return val;
            }
        }

        public virtual bool Deleted { get; set; }
    }
}
