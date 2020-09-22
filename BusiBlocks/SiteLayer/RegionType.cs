using System;

namespace BusiBlocks.SiteLayer
{
    public class RegionType
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Int32 Seq { get; set; }

        public virtual bool Deleted { get; set; }
    }
}