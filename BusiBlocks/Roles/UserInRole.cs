using BusiBlocks.Membership;

namespace BusiBlocks.Roles
{
    public class UserInRole
    {
        protected UserInRole()
        {
        }

        public UserInRole(string application, User user, Role role)
        {
            ApplicationName = application;
            Role = role;
            User = user;
        }

        public virtual string Id { get; protected set; }

        public virtual string ApplicationName { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }

        public virtual bool Deleted { get; set; }
    }
}