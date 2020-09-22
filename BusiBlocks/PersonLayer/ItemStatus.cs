using BusiBlocks.SiteLayer;

namespace BusiBlocks.PersonLayer
{
    public class ItemStatus
    {
        public virtual string Id { get; set; }
        public virtual Person Person { get; set; }
        public virtual string Version { get; set; }
        public virtual bool RequireAck { get; set; }
        public virtual string AckTime { get; set; }
        public virtual string ViewedTime { get; set; }
    }
}