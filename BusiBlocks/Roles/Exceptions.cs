using System;
using System.Configuration.Provider;

namespace BusiBlocks.Roles
{
    [Serializable]
    public class RoleNotFoundException : ProviderException
        //BusiBlocksException (MSDN reccomand to use the Provider exception in this case)
    {
        public RoleNotFoundException(string role)
            : base("Role " + role + " not found")
        {
        }
    }

    [Serializable]
    public class UserInRoleNotFoundException : ProviderException
        //BusiBlocksException (MSDN reccomand to use the Provider exception in this case)
    {
        public UserInRoleNotFoundException(string user, string role)
            : base("User " + user + " in role " + role + " not found")
        {
        }
    }
}