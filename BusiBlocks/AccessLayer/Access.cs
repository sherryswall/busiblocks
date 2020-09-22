using System;
using System.Collections.Generic;

namespace BusiBlocks.AccessLayer
{
    [Serializable]
    public class Access
    {
        public virtual string Id { get; set; }
        public virtual ItemType ItemType { get; set; }
        public virtual string ItemId { get; set; }

        public virtual string PersonTypeId { get; set; }
        public virtual string SiteId { get; set; }
        public virtual string UserId { get; set; }
        public virtual bool AllSites { get; set; }
        public virtual bool AllPersonTypes { get; set; }
        public virtual bool AllUsers { get; set; }

        public virtual bool Deleted { get; set; }

        public virtual AccessType AccessType { get; set; }
    }

    public class AccessList
    {
        public AccessList()
        {
            Accesses = new List<Access>();
        }

        public List<Access> Accesses { get; set; }
    }
}