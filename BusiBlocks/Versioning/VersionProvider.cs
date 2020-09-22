using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.Versioning
{
    public abstract class VersionProvider: ProviderBase
    {

        public abstract void CreateVersionItem(VersionItem Item);
        public abstract void UpdateVersionItem(VersionItem Item);
        public abstract void DeleteVersionItem(VersionItem Item);

        public abstract IList<VersionItem> GetAllVersions(string GroupId);
        public abstract List<VersionItem> GetAllLatestDrafts();
        public abstract VersionItem GetPublishedVersion(string GroupId);        
        public abstract IList<VersionItem> GetPublishedVersions(string GroupId);
        public abstract IList<VersionItem> GetRespectivePublishedVersions(string GroupId,string VersionNumber);
        public abstract VersionItem GetVersionItemById(string VersionId);
        public abstract VersionItem GetVersionByItemId(string ItemId);
        public abstract VersionItem GetVersionByGroupId(string GroupId);
        public abstract string GetVersionNumber(VersionType VersionType, string Id);

        public abstract void CheckInVersion(string VersionId);
        public abstract void CheckOutVersion(string VersionId,string UserName);
        public abstract string GetCheckedOutUser(string GroupId);

        public abstract bool IsLatestVersion(string VersionId);
    }
}
