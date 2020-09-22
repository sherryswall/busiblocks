using BusiBlocks.Roles;

namespace BusiBlocks.PersonLayer
{
    public class PersonTypeRole
    {
        public virtual string Id { get; set; }
        public virtual PersonType PersonType { get; set; }
        public virtual Role Role { get; set; }

        public virtual bool Deleted { get; set; }
    }
}