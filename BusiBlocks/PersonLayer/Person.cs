using BusiBlocks.AddressLayer;

namespace BusiBlocks.PersonLayer
{
    public class Person
    {
        public virtual string Id { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Position { get; set; }
        public virtual string WorkPhone { get; set; }
        public virtual string WorkFax { get; set; }
        public virtual string WorkEmail { get; set; }
        public virtual string WorkMobile { get; set; }
        public virtual Address Address { get; set; }

        public virtual bool Deleted { get; set; }
    }
}