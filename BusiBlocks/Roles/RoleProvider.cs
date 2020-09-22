using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.Roles
{
    public abstract class RoleProvider : ProviderBase
    {
        public abstract void CreateUserRole(User user, Role role);

        public abstract void CreateRole(string roleName);

        public abstract Role GetRole(string roleId);

        public abstract Role GetRoleByName(string roleName);

        public abstract IList<Role> GetRolesByUser(User user);

        public abstract IList<Role> GetAllRoles();

        public abstract void DeleteUserFromRole(User user, Role role);

        public abstract void DeleteRole(string roleName);
    }
}