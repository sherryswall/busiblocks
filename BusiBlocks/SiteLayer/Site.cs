using BusiBlocks.AddressLayer;

namespace BusiBlocks.SiteLayer
{
    public class Site
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Region Region { get; set; }
        public virtual SiteType SiteType { get; set; }
        public virtual Address PhysicalAddress { get; set; }
        public virtual Address PostalAddress { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string AltPhoneNumber { get; set; }
        public virtual string Email { get; set; }

        public virtual bool Deleted { get; set; }
    }
}