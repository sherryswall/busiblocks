using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.Membership;
using BusiBlocks.Audit;
using BusiBlocks.Versioning;
using BusiBlocks.PersonLayer;
using BusiBlocks.AccessLayer;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.SiteLayer;
using BusiBlocks;
using BusiBlocks.ItemApprovalStatusLayer;

public partial class Communication_UserViewStatus : System.Web.UI.Page
{
    private const string Nothing = "Nothing";
    private const string Acknowledged = "Acknowledged";
    private const string NotAcknowledged = "NotAcknowledged";
    private const string Viewed = "Viewed";
    private const string NotViewed = "NotViewed";

    private string ItemId
    {
        get { return (Request.QueryString["itemId"] != null) ? Request.QueryString["itemId"].ToString() : string.Empty; }
    }
    private string VersionId
    {
        get { return (Request.QueryString["versionId"] != null) ? Request.QueryString["versionId"].ToString() : string.Empty; }
    }
    private VersionItem PublishedVersion
    {
        get { return GetPublishedVersion(VersionId); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Item item = NewsManager.GetItem(ItemId);
            if (item == null)
                throw new ArgumentNullException("item");

            lblAnnouncementName.Text = item.Title;

            VersionItem versionItem = PublishedVersion;

            // Are we acking or viewing?
            if (!item.RequiresAck)
            {
                lblAck.Text = "Viewed:";
                lblNotAck.Text = "Not Viewed:";
            }

            lblAuthor.Text = item.Author;
            lblDate.Text = Utilities.GetDateTimeForDisplay(item.NewsDate);


            if (versionItem == null)
                throw new ArgumentNullException("publishedVersion");

            int index = versionItem.VersionNumber.IndexOf(".", versionItem.VersionNumber.IndexOf(".") + 1);
            lblVersionNumber.Text = versionItem.VersionNumber.Substring(0, index);

            BindViewCount(versionItem.GroupId, versionItem.VersionNumber, item.Category.Id);
        }
    }

    #region Grid Events

    protected void RadGrid1_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        IList<ViewStatusGridItem> items = new List<ViewStatusGridItem>();
        IList<User> totalUsers = GetTotalUsers(NewsManager.GetItem(ItemId).Category.Id);

        foreach (User user in totalUsers)
        {
            ViewStatusGridItem item = new ViewStatusGridItem();
            item.User = user;
            item.DisplayName = Utilities.GetDisplayUserName(user.Name);
            VersionItem publishedVersionItem = PublishedVersion;
            Item newsItem = NewsManager.GetItem(publishedVersionItem.ItemId);
            item.ViewStatus = Nothing;

            TrafficLightStatus tflStatus = NewsManager.GetTrafficLight(user.Name, newsItem);

            item.TrafficLight = Utilities.GetTrafficLightImageTag(tflStatus.RequiresAck, (tflStatus.Acknowledged || tflStatus.Viewed));
            items.Add(item);
        }

        RadGrid1.DataSource = items;
    }

    protected void RadGrid1_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {        
    }

    #endregion

    private class ViewStatusGridItem
    {
        public User User { get; set; }
        public string DisplayName { get; set; }
        public string DateViewed { get; set; }
        public string ViewStatus { get; set; }
        public string TrafficLight { get; set; }
    }

    #region View Status

    private void BindViewCount(string groupId, string versionNumber, string categoryId)
    {
        string versionGroupId = groupId;// item["GroupId"].Text;
        //string versionNumber = item["VersionNumber"].Text;
        //string categoryId = item["categoryId"].Text;

        IList<User> totalUsers = GetTotalUsers(categoryId);
        List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        List<User> usersNotViewed = new List<BusiBlocks.Membership.User>();

        //get the published id.
        List<string> publishedIds = new List<string>();
        publishedIds = GetRespectivePublishedVersions(versionGroupId, versionNumber);

        //bind figures only when theres a published id.
        if (publishedIds.Count > 0)
        {
            Item item = NewsManager.GetItem(ItemId);
            if (item.RequiresAck)
            {
                usersViewed = GetViewUsers(AuditRecord.AuditAction.Acknowledged, totalUsers, publishedIds);

                foreach (User user in totalUsers)
                    if (!usersViewed.Contains(user))
                        usersNotViewed.Add(user);

                lblNotAckNumber.Text = usersNotViewed.Count > 0 ? "(" + usersNotViewed.Count().ToString() + ")" : "(0)";
                lblAckNumber.Text = usersViewed.Count > 0 ? "(" + usersViewed.Count().ToString() + ")" : "(0)";
            }
            else
            {
                usersViewed = GetViewUsers(AuditRecord.AuditAction.Viewed, totalUsers, publishedIds);

                foreach (User user in totalUsers)
                    if (!usersViewed.Contains(user))
                        usersNotViewed.Add(user);

                lblNotAckNumber.Text = (usersNotViewed.Count > 0 ? "(" + usersNotViewed.Count().ToString() + ")" : "(0)");
                lblAckNumber.Text = (usersViewed.Count > 0 ? "(" + usersViewed.Count().ToString() + ")" : "(0)");
            }
        }
    }

    private IList<User> GetTotalUsers(string categoryId)
    {
        IList<User> totalUsers = new List<BusiBlocks.Membership.User>();//total number of users who have viewed the announcement
        List<User> viewUsers = new List<BusiBlocks.Membership.User>();

        IList<Access> accesses = AccessManager.GetItemAccess(categoryId);
        const string allGroupsLabel = "All Groups";
        const string allLocationsLabel = "All Sites";

        foreach (Access access in accesses)
        {
            PersonType personType = null;
            Site site = null;

            if (!string.IsNullOrEmpty(access.PersonTypeId))
                personType = PersonManager.GetPersonTypeById(access.PersonTypeId);
            if (!string.IsNullOrEmpty(access.SiteId))
                site = SiteManager.GetSiteById(access.SiteId);

            if (personType != null || site != null || access.AllSites || access.AllPersonTypes || access.AllUsers)
            {
                IList<Person> persons = PersonManager.GetAllPersons();
                foreach (Person person in persons)
                {
                    User user = MembershipManager.GetUserByPerson(person);
                    bool add = false;
                    if (SecurityHelper.CanUserView(user.Name, categoryId))
                    {
                        if (access.AllUsers || access.AllPersonTypes || access.AllSites)
                        {
                            add = true;
                        }
                        else if (personType != null)
                        {
                            if (PersonManager.IsPersonInPersonType(person, personType))
                                add = true;
                        }
                        else if (site != null)
                        {
                            if (PersonManager.IsPersonInPersonSite(person, site))
                                add = true;
                        }
                        if (add && totalUsers.Contains(user) == false)
                        {
                            totalUsers.Add(user);
                        }
                    }
                }
            }
        }

        return totalUsers;
    }

    private List<User> GetViewUsers(AuditRecord.AuditAction auditType, IList<User> totalUsers, List<string> publishedIds)
    {
        List<User> viewUsers = new List<BusiBlocks.Membership.User>();
        foreach (string publishedId in publishedIds)
        {
            List<AuditRecord> records = (List<AuditRecord>)AuditManager.GetAuditItems(publishedId, auditType);

            foreach (User user in totalUsers)
            {
                AuditRecord record = records.Find(x => x.UserName.Equals(user.Name));
                if (record != null)
                {
                    //add unique record only
                    if (viewUsers.Exists(x => x.Name.Equals(user.Name)) == false)
                        viewUsers.Add(user);
                }
            }
        }
        return viewUsers;
    }

    private List<string> GetRespectivePublishedVersions(string versionGroupId, string versionNumber)
    {
        IList<VersionItem> versions = VersionManager.GetAllVersions(versionGroupId);
        string versionMajorSuffix = versionNumber.Split('.').First();

        List<string> publishedNewsItems = new List<string>();
        foreach (VersionItem version in versions)
        {
            string tempVerMajorSuffix = version.VersionNumber.Split('.').First();
            if (versionMajorSuffix.Equals(tempVerMajorSuffix))
            {
                string pubId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
                Item itemX = NewsManager.GetItem(version.ItemId);

                if (itemX != null)
                {
                    if (itemX.ApprovalStatus != null && itemX.ApprovalStatus.Id.Equals(pubId))
                    {
                        //add only published items
                        publishedNewsItems.Add(itemX.Id);
                    }
                }
            }
        }
        return publishedNewsItems;
    }

    #endregion

    private VersionItem GetPublishedVersion(string VersionId)
    {
        string groupId = VersionManager.GetVersionById(VersionId).GroupId;
        string aprrovalPubId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
        IList<VersionItem> versions = VersionManager.GetPublishedVersions(groupId);
        VersionItem publishedVersion = new VersionItem();
        foreach (VersionItem version in versions)
        {
            Item newsItem = NewsManager.GetItem(version.ItemId);
            if (newsItem.ApprovalStatus.Id.Equals(aprrovalPubId))
            {
                publishedVersion = version;
                break;
            }
        }
        return publishedVersion;
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Navigation.Communication_News().Redirect(this);
    }
}