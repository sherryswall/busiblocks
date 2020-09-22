using BusiBlocks.SiteLayer;

namespace BusiBlocks.PersonLayer
{
    public class PersonRegion
    {
        public virtual string Id { get; set; }
        public virtual Person Person { get; set; }
        public virtual Region Region { get; set; }
        public virtual bool IsView { get; set; }
        public virtual bool IsAdministrator { get; set; }
        public virtual bool IsManager { get; set; }

        public virtual bool Deleted { get; set; }
    }
}