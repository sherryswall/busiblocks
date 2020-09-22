using System;
using System.Configuration.Provider;

namespace BusiBlocks.AccessLayer
{
    public static class Exceptions
    {
        #region Nested type: GroupNotFoundException

        [Serializable]
        public class GroupNotFoundException : ProviderException
        {
            public GroupNotFoundException(string group)
                : base("Group " + group + " not found")
            {
            }
        }

        #endregion

        #region Nested type: UserInGroupNotFoundException

        [Serializable]
        public class UserInGroupNotFoundException : ProviderException
        {
            public UserInGroupNotFoundException(string user, string group)
                : base("User " + user + " in group " + group + " not found")
            {
            }
        }

        #endregion

        #region Nested type: UserInLocationNotFoundException

        [Serializable]
        public class UserInLocationNotFoundException : ProviderException
        {
            public UserInLocationNotFoundException(string user, string group)
                : base("User " + user + " in location " + group + " not found")
            {
            }
        }

        #endregion
    }
}