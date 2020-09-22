using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.Membership
{
    public abstract class MembershipProvider : ProviderBase
    {
        public abstract User GetUser(string userId);

        public abstract User GetUserByName(string userName);

        public abstract User GetUserByPersonId(string personId);

        public abstract void UpdateUser(User user);

        public abstract IList<User> GetAllUsers();
    }
}