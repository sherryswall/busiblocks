using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.AccessLayer
{
    public abstract class AccessProvider : ProviderBase
    {
        public abstract void CreateAccess(Access access);

        public abstract void CreateFolderKey(Folder folder);

        public abstract Folder GetFolderKey(string path);

        public abstract IList<Access> GetItemAccess(string itemId);

        public abstract IList<Access> GetItemEdittables(string itemId);

        public abstract IList<Access> GetItemVisibilities(string itemId);

        public abstract IList<Access> GetItemContributions(string itemId);

        public abstract List<Access> GetUsersAccessibleItems(string userName, ItemType itemType, AccessType accessType);

        public abstract IList<User> GetAccessibleItemUsers(Access access);

        public abstract IList<Access> GetItemsMatchingAccess(Access access, ItemType itemType, AccessType accessType);

        public abstract void DeleteAccess(string accessId);

        public abstract void DeleteAccess(string groupId, string itemId, ItemType itemType);

        public abstract void DeleteFolderKey(string path);
    }
}