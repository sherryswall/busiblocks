using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using NHibernate.Id;
using NHibernate.Engine;
using NHibernate.Util;

namespace BusiBlocks.Versioning
{
    public class BusiBlocksVersionProvider : VersionProvider
    {
        #region Properties

        private string _applicationName;
        private string _providerName;

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        #endregion

        private ConnectionParameters _configuration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksSiteProvider";

            base.Initialize(name, config);

            _providerName = name;
            _applicationName = ExtractConfigValue(config, "applicationName", ConnectionParameters.DefaultApp);
            //System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath

            string connName = ExtractConfigValue(config, "connectionStringName", null);
            _configuration = ConnectionParameters.Create(connName);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " +
                                                attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private static string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;

            config.Remove(key);
            return val;
        }

        public override void CreateVersionItem(VersionItem item)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                vDTS.Insert(item);
                transaction.Commit();
            }
        }

        public override void UpdateVersionItem(VersionItem item)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                vDTS.Update(item);
                transaction.Commit();
            }
        }

        public override void DeleteVersionItem(VersionItem item)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                VersionItemDataStore vDTS = new VersionItemDataStore(transaction);
                item.Deleted = true;
                vDTS.Update(item);
                transaction.Commit();
            }
        }

        public override IList<VersionItem> GetAllVersions(string groupId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindAllByGroupId(groupId);
            }
        }

        public override VersionItem GetVersionByItemId(string itemId)
        {
            //find the 
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindGroupIdByItemId(itemId);
            }
        }

        public override VersionItem GetVersionItemById(string versionId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindGroupIdByVersionId(versionId);
            }
        }

        /// <summary>
        /// Returns the latest version.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public override VersionItem GetVersionByGroupId(string groupId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindVersionByGroupId(groupId);
            }
        }

        public override bool IsLatestVersion(string versionId)
        {
            VersionItem item = GetVersionItemById(versionId);
            if (item != null)
            {
                VersionItem latestVersion = GetAllVersions(item.GroupId).First<VersionItem>();
                if (latestVersion.Id.Equals(item.Id))
                    return true;
                else
                    return false;
            }
            return true;
        }

        public override string GetVersionNumber(VersionType versionType, string itemId)
        {
            if (versionType == VersionType.New)
                return "0.0.1";
            else
                return GetNextVersionNumber(versionType, itemId);
        }

        public string GetNextVersionNumber(VersionType versionType, string itemId)
        {
            string versionNumber = string.Empty;
            //find the version item with the id 
            // pass in the versionnumber

            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);

                //VersionItem item = vDTS.FindAll().First<VersionItem>(x => x.ItemId.Equals(ItemId));
                VersionItem item = vDTS.FindGroupIdByItemId(itemId);
                VersionItem latestVersion = vDTS.FindVersionByGroupId(item.GroupId);
                return GenerateVersionNumber(versionType, latestVersion.VersionNumber);
            }
        }

        public string GenerateVersionNumber(VersionType versionType, string versionNumber)
        {
            string newVersionNumber = string.Empty;
            string[] versionParts = versionNumber.Split('.');
            //first index = Major;
            //Second index = Minor;
            //Third index =draft;

            if (versionType == VersionType.Draft)
            {
                int draftPart = Convert.ToInt32(versionParts[2]);
                draftPart = draftPart + 1;
                versionParts[2] = draftPart.ToString();
            }
            else if (versionType == VersionType.Minor)
            {
                int minorPart = Convert.ToInt32(versionParts[1]);
                minorPart = minorPart + 1;
                versionParts[1] = minorPart.ToString();
                //clear the draft part
                versionParts[2] = "0";
            }
            else if (versionType == VersionType.Major)
            {
                int majorPart = Convert.ToInt32(versionParts[0]);
                majorPart = majorPart + 1;
                versionParts[0] = majorPart.ToString();
                //clear the minor and draft part;
                versionParts[1] = "0";
                versionParts[2] = "0";
            }
            newVersionNumber = string.Join(".", versionParts);
            return newVersionNumber;
        }

        /// <summary>
        /// Returns list of all the latest drafts
        /// </summary>
        /// <returns></returns>
        public override List<VersionItem> GetAllLatestDrafts()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                IList<VersionItem> allVersions = vDTS.FindAll();
                List<VersionItem> A = new List<VersionItem>();//get only unique group ids
                List<VersionItem> B = new List<VersionItem>();//get all group IDs
                List<VersionItem> C = new List<VersionItem>();// the list with latest versions only

                //iterate through all version items.
                //put unique groupIds in one list
                //use that list to iterate through the versionItems again and retrieve the latest of all version for the particular groupID
                foreach (VersionItem item in allVersions)
                {
                    if (B.Count > 0 && B.Exists(x => x.GroupId.Equals(item.GroupId)) == true)
                    {
                        B.Add(item);
                    }
                    else
                    {
                        A.Add(item);
                        B.Add(item);
                    }
                    //get groupID
                    //add to list
                }

                foreach (VersionItem itemA in A)
                {
                    C.Add(vDTS.FindVersionByGroupId(itemA.GroupId));
                }
                //return the list which has the version items with only latest draft.
                return C;
            }
        }

        /// <summary>
        /// Returns the Version Item that was last published
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public override VersionItem GetPublishedVersion(string groupId)
        {
            //pass in the group ID.
            // return the version item list with status as published in desc
            // return the first item only.
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindPublishedVersion(groupId);
            }
        }
        /// <summary>
        /// Finds all published versions of the item
        /// </summary>
        /// <param name="versionId"></param>
        public override IList<VersionItem> GetPublishedVersions(string groupId)
        {
            //pass in the group ID.
            // return the version item list with status as published in desc
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                return vDTS.FindPublishedVersions(groupId);
            }
        }

        public override IList<VersionItem> GetRespectivePublishedVersions(string groupId,string versionNumber)
        {
            //pass in the group ID.
            // return the version item list with status as published in desc
            string versionMajorSuffix = versionNumber.Split('.').First();
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                IList<VersionItem> pubVersions= vDTS.FindPublishedVersions(groupId);
                foreach (VersionItem version in pubVersions)
                {
                    string tempMajorSuffix = version.VersionNumber.Split('.').First();
                    if (!tempMajorSuffix.Equals(versionMajorSuffix))
                    {
                        pubVersions.RemoveAt(pubVersions.IndexOf(version));
                    }
                }
                return pubVersions;
            }
        }

        public override void CheckInVersion(string versionId)
        {
            if (string.IsNullOrEmpty(versionId))
                throw new ArgumentNullException("versionId");

            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                VersionItem version = vDTS.FindGroupIdByVersionId(versionId);
                IList<VersionItem> versions = vDTS.FindAllByGroupId(version.GroupId);
                foreach (VersionItem vers in versions)
                {
                    vers.UserName = null;
                    vDTS.Update(vers);
                }                
                transaction.Commit();
            }
        }

        public override void CheckOutVersion(string versionId, string userName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var vDTS = new VersionItemDataStore(transaction);
                VersionItem version = vDTS.FindGroupIdByVersionId(versionId);
                version.UserName = userName;
                vDTS.Update(version);
                transaction.Commit();
            }
        }
        /// <summary>
        /// Accepts groupId and Returns username of the person who has the item checked out.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public override string GetCheckedOutUser(string groupId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                string userName = string.Empty;
                var vDTS = new VersionItemDataStore(transaction);

                IList<VersionItem> versions = vDTS.FindAllByGroupId(groupId);
                foreach (VersionItem version in versions)
                {
                    if (!string.IsNullOrEmpty(version.UserName))
                    {
                        userName = version.UserName;
                        break;
                    }
                }
                return userName;
            }
        }
    }
}
