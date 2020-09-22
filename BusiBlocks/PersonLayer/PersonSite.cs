using BusiBlocks.SiteLayer;

namespace BusiBlocks.PersonLayer
{
    public class PersonSite
    {
        public virtual string Id { get; set; }
        public virtual Site Site { get; set; }
        public virtual Person Person { get; set; }
        public virtual bool IsAdministrator { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual bool IsAssigned { get; set; }
        public virtual bool Deleted { get; set; }
    }
}