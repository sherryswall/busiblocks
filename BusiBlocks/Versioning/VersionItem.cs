using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.Versioning
{
    /// <summary>
    /// Hold information regarding a particular version
    /// </summary>
    /// 
    [Serializable]
    public class VersionItem
    {
        public virtual string Id { get; set; }
        public virtual string GroupId { get; set; }
        public virtual string ItemId { get; set; }
        public virtual string TypeId { get; set; }
        public virtual string VersionNumber { get; set; }
        public virtual string VersionTypeId { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string UserName { get; set; }
        public virtual string EditSeverity { get; set; }
    }
    public enum VersionType
    {
        New = 0,
        Draft = 1,
        Minor = 2,
        Major = 3
    }
}
