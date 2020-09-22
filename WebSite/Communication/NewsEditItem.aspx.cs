using System;
using System.Linq;
using System.Collections.Generic;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.Versioning;
using Telerik.Web.UI;
using BusiBlocks.AccessLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks.Membership;
using BusiBlocks.ApproverLayer;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.Audit;
using BusiBlocks.SiteLayer;
using System.Web.Security;

public partial class Communication_NewsEditItem : Page
{
    private const string SessionKeyItem = "item";
    private const string SessionKeyCategory = "category";

    private const string TitleEdit = "Edit Announcement";
    private const string TitleNew = "Create Announcement";

    private const string ErrorSelectCategory = "Select a category";
    private const string ErrorCategoryPermission = "Permission to category denied!";

    private string currentSelectedNode { set; get; }

    //[Deprecated]
    //News item is now loaded from the verisonId
    private string ItemId
    {
        get { return Request[SessionKeyItem]; }
    }

    private string VersionId
    {
        get { return (Request.QueryString["item"] != null) ? Request.QueryString["item"].ToString() : string.Empty; }
    }

    private string NewVersionId
    {
        get;
        set;
    }

    private string CategoryId
    {
        get { return Request[SessionKeyCategory]; }
    }

    private Item _item;

    private Item Item
    {
        get
        {
            if (_item == null)
            {
                //find the version in version table and load its respective content from the newsItem table.
                VersionItem version = VersionManager.GetVersionById(VersionId);
                if (version != null)
                {
                    Item item = NewsManager.GetItem(version.ItemId);
                    if ((!string.IsNullOrEmpty(item.Id)) && (item.Id.Length > 0))
                    {
                        _item = NewsManager.GetItem(item.Id);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ItemId))
                        _item = NewsManager.GetItem(ItemId);
                }
            }
            return _item;
        }
        set
        {
            _item = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tv.OnNodeClicked += category_Clicked;

        if (!IsPostBack)
        {
            BindPage();
        }
        else
            CurrentSelNodeVal.Value = currentSelectedNode = tv.Selected;
    }

    protected void category_Clicked(object sender, EventArgs e)
    {
        currentSelectedNode = tv.Selected;

        if (!string.IsNullOrEmpty(currentSelectedNode))
        {
            CurrentSelNodeVal.Value = currentSelectedNode;
            string categoryName = NewsManager.GetCategory(currentSelectedNode).Name;
            lblTitle.Text = txtTitle.Text;
            //repopulate the matrix;
            pmm.LoadPermissionsView(currentSelectedNode, categoryName);
            BindApprovers();
        }
    }

    private void BindApprovers()
    {
        //get the groups under the category
        //get the users in that group
        //bind the users to the drop down

        //clear the dropdown list
        ddlApprovers.Items.Clear();
        ddlApprovers.Enabled = false;

        ddlOwner.Items.Clear();
        ddlOwner.Enabled = true;

        if (!string.IsNullOrEmpty(currentSelectedNode))
        {
            IList<Access> accessList = AccessManager.GetItemEdittables(currentSelectedNode);
            bool canEdit = false;

            if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, currentSelectedNode))
            {
                canEdit = true;
            }
            else if (SecurityHelper.CanUserContribute(Page.User.Identity.Name, currentSelectedNode))
            {
                User currentUser = MembershipManager.GetUserByName(Page.User.Identity.Name);
                ddlOwner.Items.Add(new ListItem(currentUser.Name, currentUser.Id));
            }
            if (canEdit)
            {
                foreach (Access access in accessList)
                {
                    IList<Person> persons = PersonManager.GetAllPersons();

                    foreach (Person person in persons)
                    {
                        User user = MembershipManager.GetUserByPerson(person);
                        if (!string.IsNullOrEmpty(user.Name))
                        {
                            if (ddlOwner.Items.FindByText(user.Name) == null)
                            {
                                if (SecurityHelper.CanUserEdit(user.Name, currentSelectedNode))
                                    ddlOwner.Items.Add(new ListItem(user.Name, user.Id));
                            }

                            //APPROVERS NOT REQUIRED AT THIS RELEASE - 19/06/2012
                            //if (ddlApprovers.Items.FindByText(user.Name) == null)
                            //{
                            //    ddlApprovers.Items.Add(new ListItem(user.Name, user.Id));
                            //}                        
                        }
                    }
                }
            }
            //ddlApprovers.Items.Insert(0, new ListItem("--All Approvers--", ""));
            if (ddlOwner.Items.FindByText("admin") == null)
            {
                User admin = MembershipManager.GetUserByName("admin");
                ddlOwner.Items.Insert(0, new ListItem(admin.Name, admin.Id));
            }

            if ((Item != null) && (Item.Owner != null) && (ddlOwner.Items.FindByValue(Item.Owner) == null))
            {
                User currentUser = MembershipManager.GetUserByName(Item.Owner);
                ddlOwner.Items.Insert(0, new ListItem(currentUser.Name, currentUser.Id));
                ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText(Item.Owner));
            }
        }
    }

    private void BindPage()
    {
        if (!string.IsNullOrEmpty(CategoryId))
        {
            Category cat = NewsManager.GetCategory(CategoryId);
            pmm.LoadPermissionsView(cat.Id, cat.Name);
        }
        BindCategory();

        //Bind Approvers
        //NOTE: NOT BINDING THE LIST SINCE THE TV.SELECTED IS EMPTY.
        currentSelectedNode = tv.Selected;
        BindApprovers();

        if (Item != null)
            BindPageEditItem();
        else
            BindPageNewItem();

        if (Request.QueryString.Get("mode") == "approve")
        {
            divActionFunctions.Visible = false;
            divSaveFunctions.Visible = false;

            ddlApprovers.Enabled = false;
            rblAcknowledge.Enabled = false;
            //rdpExpiry.Enabled = false;
        }
        else
        {
            divApprove.Visible = false;
        }
    }

    private void BindPageEditItem()
    {
        divSaveFunctions.Visible = true;
        btnDelete.Visible = true;

        VersionItem version = VersionManager.GetVersionById(VersionId);
        Item item = (version == null) ? NewsManager.GetItem(ItemId) : NewsManager.GetItem(version.ItemId);

        txtDescription.Content = item.Description;
        txtTitle.Text = item.Title;
        lblTitle.Text = item.Title;
        ddlOwner.SelectedIndex = ddlOwner.Items.IndexOf(ddlOwner.Items.FindByText(item.Owner));

        //rdpExpiry.SelectedDate = item.Expiry;
        lnkVersionNumber.Text = "Version&nbsp;" + ((version == null) ? VersionManager.GetVersionNumber(VersionType.New, string.Empty) : VersionManager.GetVersionNumber(VersionType.Draft, version.ItemId));
        lnkVersionNumber.NavigateUrl = (version == null) ? null : Navigation.Communication_NewsVersionHistory(version.GroupId).GetServerUrl(true);

        // Set the acknowledge radios.
        if (item.RequiresAck)
        {
            rblAcknowledge.Items.FindByValue("required").Selected = true;
            ackReqd.CssClass = "";
            rblAcknowledge.Items.FindByValue("notRequired").Selected = false;
            ackNotReqd.CssClass = "hideElement";
        }
        else
        {
            rblAcknowledge.Items.FindByValue("required").Selected = false;
            ackReqd.CssClass = "hideElement";
            rblAcknowledge.Items.FindByValue("notRequired").Selected = true;
            ackNotReqd.CssClass = "";
        }
        // Set the approval radios.
        if (item.RequiresApproval)
        {
            rblApproval.Items.FindByValue("required").Selected = true;
            rblApproval.Items.FindByValue("notRequired").Selected = false;
        }
        else
        {
            rblApproval.Items.FindByValue("required").Selected = false;
            rblApproval.Items.FindByValue("notRequired").Selected = true;
        }

        //pre selecting the major/minor
        if (version == null)
        {
            //rblEditDetails.Items.FindByText("minor").Selected = true;
            trEditDetails.Visible = false;
        }
        else
        {
            //check if there's any published version - true =use the previous severity value else set it to major as first published will always be null. 
            Item tempItem = NewsManager.GetPublishedItem(version.GroupId);
            if (tempItem.Id != null)
            {
                if (!string.IsNullOrEmpty(version.EditSeverity))
                    rblEditDetails.Items.FindByValue(version.EditSeverity).Selected = true;
                else
                    rblEditDetails.Items.FindByValue("minor").Selected = true;
            }
            else
                trEditDetails.Visible = false;
        }
        //Load permissions
        pmm.LoadPermissionsView(item.Category.Id, item.Category.Name);

        //if user is approver then show the minor/major option.
        lblPageHeading.Text = TitleEdit;

        //select the approver item only if theres a version existing.
        if (version != null)
        {
            IList<Approver> approvers = ApproverManager.GetApproversByItem(version.Id);
            //count more than one means its a category approval
            if (approvers.Count != 0 && ddlApprovers.Enabled != false)
            {
                if (!string.IsNullOrEmpty(approvers[0].UserId))
                {
                    string ddlValue = MembershipManager.GetUser(approvers[0].UserId).Name;
                    ddlApprovers.Items.FindByText(ddlValue).Selected = true;
                }
                else if (ddlApprovers.Items.Count > 0)
                    ddlApprovers.Items[0].Selected = true;
            }

            bool isAckReq = NewsManager.GetItem(version.ItemId).RequiresAck;
            if (isAckReq)
            {
                BindAcknowledgeCount(version.ItemId, version.GroupId, version.VersionNumber);
                BindViewerCount(null, null);
            }
            else
            {
                BindViewerCount(version.GroupId, version.VersionNumber);
                BindAcknowledgeCount(null, null, null);
            }
        }
    }

    private void BindPageNewItem()
    {
        lblPageHeading.Text = TitleNew;

        if (ddlOwner.Items.FindByText(Utilities.GetUserName(Page.User.Identity.Name)) != null)
            ddlOwner.Items.FindByText(Utilities.GetUserName(Page.User.Identity.Name)).Selected = true;

        lblTitle.Text = txtTitle.Text;
        btnCreatePublish.Visible = true;
        btnCreate.Visible = true;
        lnkVersionNumber.Text = "Version&nbsp;" + VersionManager.GetVersionNumber(VersionType.New, string.Empty);
        divSaveFunctions.Visible = false;
        trEditDetails.Visible = false;
        rblAcknowledge.Items.FindByValue("required").Selected = false;
        ackReqd.CssClass = "hideElement";
        rblAcknowledge.Items.FindByValue("notRequired").Selected = true;
        ackNotReqd.CssClass = "";
    }

    private void BindCategory()
    {
        if (Item != null)
        {
            //treeCategory.Selected = CurrentSelNodeVal.Value = Item.Category.Id;
            tv.Selected = CurrentSelNodeVal.Value = Item.Category.Id;
        }
        else if ((!string.IsNullOrEmpty(CategoryId)) && (CategoryId.Length > 0))
        {
            tv.Selected = CurrentSelNodeVal.Value = CategoryId;
        }
    }

    protected void SaveChanges(SaveType saveType)
    {
        Category category = (CurrentSelNodeVal.Value == null ? null : NewsManager.GetCategory(CurrentSelNodeVal.Value));
        string catId = (category == null) ? string.Empty : category.Id;
        if (category == null)
            ((IFeedback)Master).SetError(GetType(), ErrorSelectCategory);
        else if (!SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, category.Id))
            ((IFeedback)Master).SetError(GetType(), ErrorCategoryPermission);
        else
        {
            SaveItem(category, saveType);

            ((IFeedback)Page.Master).QueueFeedback(
                BusiBlocksConstants.Blocks.Communication.LongName,
                Item.GetType().Name,
                (saveType == SaveType.Publish ? "publish" : Feedback.Actions.Saved),
                Item.Title
            );
        }
    }

    /// <summary>
    /// Checks in the current version which is been edited.
    /// </summary>
    protected void CheckInVersion()
    {
        string versionId = VersionId;
        if (string.IsNullOrEmpty(versionId))
            versionId = NewVersionId;
        VersionItem versionItem = VersionManager.GetVersionById(versionId);
        VersionManager.CheckInVersion(versionItem.Id);
    }

    private void SaveItem(Category category, SaveType saveType)
    {
        if (txtTitle.Text.Length >= 100)
        {
            ((IFeedback)Page.Master).SetError(GetType(), "The announcement title must be less than 100 characters long");
            return;
        }
        Item.Category = category;
        Item.Description = txtDescription.Content;
        Item.Title = txtTitle.Text;
        Item.Tag = rblAcknowledge.Items.FindByValue("required").Selected.ToString() + ":" +
        rblApproval.Items.FindByValue("required").Selected.ToString();
        //Item.Expiry = rdpExpiry.SelectedDate;

        string versionId = VersionId;
        if (string.IsNullOrEmpty(versionId))
            versionId = NewVersionId;

        VersionItem versionItem = VersionManager.GetVersionById(versionId);
        VersionType versionType = VersionType.Draft;

        string GroupID = Item.Id;
        string VersionNumber = VersionManager.GetVersionNumber(VersionType.New, string.Empty);

        ItemApprovalStatus approvalStatus = new ItemApprovalStatus();

        if (rblApproval.SelectedValue.Equals("required"))
        {
            if (saveType == SaveType.CheckIn)
                approvalStatus = ItemApprovalStatusManager.GetDraftStatus();
            else if (saveType == SaveType.Publish)
                approvalStatus = (Item.Author.Equals(Utilities.GetUserName(Page.User.Identity.Name)) == true) ?
                    ItemApprovalStatusManager.GetForApprovalStatus() : Item.ApprovalStatus = ItemApprovalStatusManager.GetForEditApprovalStatus();
            else
                approvalStatus = new ItemApprovalStatus() { Id = string.Empty, Name = string.Empty };
        }
        else
        {
            if (saveType == SaveType.CheckIn)
            {
                approvalStatus = ItemApprovalStatusManager.GetDraftStatus();
            }
            else if (saveType == SaveType.Publish)
            {
                approvalStatus = ItemApprovalStatusManager.GetStatusByName("Published");
                versionType = (rblEditDetails.SelectedIndex == 0) ? VersionType.Minor : VersionType.Major;
            }
        }

        if (versionItem != null)
        {
            //Create a new record only when saving after editing an existing version
            //if it's being put on hold then don't create a new record 
            if (saveType == SaveType.Hold)
            {
                NewsManager.UpdateItem(Item);
            }
            else
            {
                Item = CreateItem(category, ddlOwner.SelectedItem.Text, txtTitle.Text, txtDescription.Content, approvalStatus);
            }
            GroupID = versionItem.GroupId;
            VersionNumber = VersionManager.GetVersionNumber(versionType, versionItem.ItemId);
        }
        //this applies to announcements when they don't have any versions previously.
        else
        {
            Item.ApprovalStatus = approvalStatus;
            NewsManager.UpdateItem(Item);
        }
        //save version item only when checkin or publish.
        if (saveType != SaveType.Hold)
            NewVersionId = SaveVersionItem(GroupID, VersionNumber, saveType);
    }

    private Item CreateItem(Category category, string owner, string title, string content, ItemApprovalStatus approvalStatus)
    {
        return NewsManager.CreateItem(
                category,
                owner,
                title,
                content,
                "",
                "",
                DateTime.Now,
                rblAcknowledge.Items.FindByValue("required").Selected,
                rblApproval.Items.FindByValue("required").Selected,
                "",
                null,
                null, approvalStatus
            );
    }

    private string SaveVersionItem(string groupId, string versionNumber, SaveType saveType)
    {
        // todo : there is a hack there with the 2 seconds added to the current date time. Fix this.
        // The reason is that if two versions are created within one second of each other, then the
        // 'current' version will be the first one that was created. Usually the second one is the
        // latest/current one. This is a defect whereby we determine that the latest timestamped 
        // version is the current one. There should be a flag on a version saying that it is the latest.
        VersionManager.CreateVersionItem(new VersionItem()
        {
            ItemId = Item.Id,
            GroupId = groupId,
            VersionNumber = versionNumber,
            DateCreated = (saveType == SaveType.Publish) ? DateTime.Now.AddSeconds(2.0) : DateTime.Now,
            Comments = ctrlEditComments.GetComments(),
            ModifiedBy = Utilities.GetUserName(Page.User.Identity.Name),
            EditSeverity = (trEditDetails.Visible == true) ? rblEditDetails.SelectedValue : rblEditDetails.Items[1].Value
        });

        string approverItemId = VersionManager.GetVersionByItemId(Item.Id).Id;
        //start the approval process by adding approvers.
        ApproverManager.AddApprover(new BusiBlocks.ApproverLayer.Approver() { UserId = ddlApprovers.SelectedValue, CategoryId = currentSelectedNode, ItemId = approverItemId });
        return VersionManager.GetVersionByGroupId(groupId).Id;
    }

    protected void BindViewerCount(string versionGroupId, string versionNumber)
    {
        if (string.IsNullOrEmpty(versionGroupId) || string.IsNullOrEmpty(versionNumber))
        {
            lblNotViewed.Text = "(0)";
            lblViewed.Text = "(0)";
            return;
        }

        /*get all users meant to view that announcement
         * get the published versions by group id
         * order them by insertion date and select the first one.
         */
        IList<User> totalUsers = GetTotalUsers();
        List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        List<User> usersNotViewed = new List<BusiBlocks.Membership.User>();

        //get the published id.
        List<string> publishedIds = new List<string>();
        publishedIds = GetRespectivePublishedVersions(versionGroupId, versionNumber);

        //bind figures only when theres a published id.
        if (publishedIds.Count > 0)
        {
            //get users who have viewed the item.
            usersViewed = GetViewUsers(AuditRecord.AuditAction.Viewed, totalUsers, publishedIds);

            foreach (User user in totalUsers)
                if (!usersViewed.Contains(user))
                    usersNotViewed.Add(user);

            //displaying the numbers
            if (usersNotViewed.Count > 0)
                lblNotViewed.Text = "(" + usersNotViewed.Count.ToString() + ")";
            else
                lblNotViewed.Text = "(0)";

            if (usersViewed.Count > 0)
                lblViewed.Text = "(" + usersViewed.Count.ToString() + ")";
            else
                lblViewed.Text = "(0)";
        }
    }

    protected void BindAcknowledgeCount(string itemId, string versionGroupId, string versionNumber)
    {
        if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(versionGroupId) || string.IsNullOrEmpty(versionNumber))
        {
            lblNotAcked.Text = "(0)";
            lblAcked.Text = "(0)";
            return;
        }
        //check for acknowledgement required?
        IList<User> totalUsers = GetTotalUsers();
        List<User> usersAcked = new List<BusiBlocks.Membership.User>();
        List<User> usersNotAcked = new List<BusiBlocks.Membership.User>();

        int totalNoUsers = totalUsers.Count;

        //get the published id.
        List<string> publishedIds = new List<string>();
        publishedIds = GetRespectivePublishedVersions(versionGroupId, versionNumber);

        //bind figures only when theres a published id.
        if (publishedIds.Count > 0)
        {
            //get users who have viewed the item.
            usersAcked = GetViewUsers(AuditRecord.AuditAction.Acknowledged, totalUsers, publishedIds);

            foreach (User user in totalUsers)
                if (!usersAcked.Contains(user))
                    usersNotAcked.Add(user);

            //displaying the numbers
            if (usersNotAcked.Count > 0)
                lblNotAcked.Text = "(" + usersNotAcked.Count.ToString() + ")";
            else
                lblNotAcked.Text = "(0)";

            if (usersAcked.Count > 0)
                lblAcked.Text = "(" + usersAcked.Count.ToString() + ")";
            else
                lblAcked.Text = "(0)";
        }
    }

    private IList<User> GetTotalUsers()
    {
        IList<User> totalUsers = new List<BusiBlocks.Membership.User>();//total number of users who have viewed the announcement
        List<User> viewUsers = new List<BusiBlocks.Membership.User>();

        IList<Access> accesses = AccessManager.GetItemAccess(Item.Category.Id);
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
                    if (SecurityHelper.CanUserView(user.Name, Item.Category.Id))
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

    protected void btnCreatePublish_Click(object sender, EventArgs e)
    {
        Category category = (currentSelectedNode == null ? null : NewsManager.GetCategory(currentSelectedNode));

        string catId = (category == null) ? string.Empty : category.Id;

        if (category == null)
            ((IFeedback)Master).SetError(GetType(), ErrorSelectCategory);
        else if (!SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, category.Id))
            ((IFeedback)Master).SetError(GetType(), ErrorCategoryPermission);
        else
        {
            Item = CreateItem(category, ddlOwner.SelectedItem.Text, txtTitle.Text, txtDescription.Content, ItemApprovalStatusManager.GetStatusByName("Published"));
            Item.UpdateDate = DateTime.Now.AddSeconds(4.0);
            NewVersionId = SaveVersionItem(Item.Id, VersionManager.GetVersionNumber(VersionType.New, string.Empty), SaveType.CheckIn);
            VersionItem vi = VersionManager.GetVersionById(NewVersionId);
            vi.VersionNumber = VersionManager.GetVersionNumber(VersionType.Major, vi.ItemId);
            VersionManager.UpdateVersionItem(vi);
            NewsManager.UpdateItem(Item);
            CheckInVersion();
            Navigation.Communication_News().Redirect(this);
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Category category = (currentSelectedNode == null ? null : NewsManager.GetCategory(currentSelectedNode));

        string catId = (category == null) ? string.Empty : category.Id;

        if (category == null)
            ((IFeedback)Master).SetError(GetType(), ErrorSelectCategory);
        else if (!SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, category.Id))
            ((IFeedback)Master).SetError(GetType(), ErrorCategoryPermission);
        else
        {
            Item = CreateItem(category, ddlOwner.SelectedItem.Text, txtTitle.Text, txtDescription.Content, ItemApprovalStatusManager.GetDraftStatus());

            SaveVersionItem(Item.Id, VersionManager.GetVersionNumber(VersionType.New, string.Empty), SaveType.CheckIn);

            ((IFeedback)Page.Master).QueueFeedback(
                BusiBlocksConstants.Blocks.Communication.LongName,
                Item.GetType().Name,
                Feedback.Actions.Created,
                Item.Title
            );

            Navigation.Communication_News().Redirect(this);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(VersionId))
            VersionManager.CheckInVersion(VersionId);
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnHold_Click(object sender, EventArgs e)
    {
        //Just save the announcement and don't checkin.
        SaveChanges(SaveType.Hold);
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnCheckin_Click(object sender, EventArgs e)
    {
        //save the announcement
        //checkin the announcement.
        //others will be able to edit it. 
        SaveChanges(SaveType.CheckIn);
        CheckInVersion();
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnPublish_Click(object sender, EventArgs e)
    {
        // Publish the announcement.
        SaveChanges(SaveType.Publish);
        CheckInVersion();

        Navigation.Communication_News().Redirect(this);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        NewsManager.DeleteItem(Item);

        ((IFeedback)Page.Master).QueueFeedback(
                  BusiBlocksConstants.Blocks.Communication.LongName,
                  Item.GetType().Name,
                  Feedback.Actions.Deleted,
                  Item.Title
              );
        //remove approval rights.
        ApproverManager.RemoveApproversbyItem(Item.Id);

        Navigation.Communication_News().Redirect(this);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Navigation.Communication_NewsEditItem(VersionId).Redirect(this);
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        if (Item.ApprovalStatus.Id == ItemApprovalStatusManager.GetForEditApprovalStatus().Id)
        {
            Item.ApprovalStatus = ItemApprovalStatusManager.GetForApprovalStatus();
        }
        else if (Item.ApprovalStatus.Id == ItemApprovalStatusManager.GetForApprovalStatus().Id)
        {
            Item.ApprovalStatus = ItemApprovalStatusManager.GetApprovalStatus();
        }

        Item.ApprovalStatus = ItemApprovalStatusManager.GetApprovalStatus();
        Item.UpdateDate = DateTime.Now;
        //Versioning
        //update the version item as well.
        //if person is approver only then change the major/minor version for publishing.
        if (Request.QueryString.Get("mode") == "approve")
        {
            VersionType versionType = VersionType.Draft;

            //if trEditDetails not visible then this is the first publish!
            if (trEditDetails.Visible == true)
            {
                if (!string.IsNullOrEmpty(rblEditDetails.SelectedValue))
                    versionType = (rblEditDetails.SelectedValue == "major") ? VersionType.Major : VersionType.Minor;
            }
            else
                versionType = VersionType.Major;

            VersionItem approveVersion = VersionManager.GetVersionById(VersionId);
            approveVersion.VersionNumber = VersionManager.GetVersionNumber(versionType, approveVersion.ItemId);

            VersionManager.UpdateVersionItem(approveVersion);
        }
        NewsManager.UpdateItem(Item);
        Navigation.Communication_News().Redirect(this);
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        Item.ApprovalStatus = ItemApprovalStatusManager.GetDraftStatus();
        Item.ActionedByPersonId = MembershipManager.GetUserByName(Page.User.Identity.Name).Id;
        Item.ActionedNotes = popReject.Value;
        Item.ActionedOnDate = DateTime.Now;

        NewsManager.UpdateItem(Item);
        Navigation.Communication_News().Redirect(this);
    }
    
    protected void RadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            User user = (User)e.Item.DataItem;

            string personId = ((HiddenField)item.FindControl("hfPersonId")).Value;
            bool isPrimaryExisting = false;
            string userName = string.Empty;
            if (user != null)
                userName = (string.IsNullOrEmpty(user.Name)) ? string.Empty : user.Name;
            //add primary site if any for the person
            Label primarySite = new Label();
            primarySite = (Label)item.FindControl("lblPrimarySite");
            primarySite.CssClass = "bold";
            primarySite.Text = GetPrimarySite(personId);

            if (!string.IsNullOrEmpty(primarySite.Text))
            {
                isPrimaryExisting = true;
                primarySite.Text += "<img src=../App_Themes/default/icons/star_16.png />";
            }
            else
                isPrimaryExisting = false;

            //add secondary sites if any for the person.
            Label sites = new Label();
            sites = (Label)item.FindControl("lblSecondarySites");
            sites.Text = GetSecondarySites(personId, isPrimaryExisting);
        }
    }

    protected string GetPrimarySite(string Id)
    {
        //get user and person
        Person queryPerson = PersonManager.GetPersonById(Id);
        User user1 = MembershipManager.GetUserByPerson(queryPerson);

        //get primary site to avoid duplicate site names 
        Site primarySite = PersonManager.GetDefaultSiteByPerson(queryPerson);

        if (primarySite != null && !string.IsNullOrEmpty(primarySite.Name))
            return primarySite.Name;
        else
            return string.Empty;
    }

    /// <summary>
    /// Gets secondary sites if any for a person
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isPrimaryExisting"></param>
    /// <returns>string of secondary sites</returns>     
    protected string GetSecondarySites(string id, bool isPrimaryExisting)
    {
        string secondarySites = string.Empty;
        string seperator = string.Empty;

        seperator = (isPrimaryExisting == true) ? ", " : string.Empty;
        //get user and person
        Person queryPerson = PersonManager.GetPersonById(id);
        User user1 = MembershipManager.GetUserByPerson(queryPerson);

        if (user1 != null && !string.IsNullOrEmpty(user1.Name))
        {
            //get primary site to avoid duplicate site names 
            Site primarySite = PersonManager.GetDefaultSiteByPerson(queryPerson);
            MembershipUser membershipUser = Membership.GetUser(user1.Name);

            if (!string.IsNullOrEmpty(membershipUser.UserName))
            {
                IList<Site> sites = SiteManager.GetSitesByUser(membershipUser.UserName, true);

                for (int i = 0; i < sites.Count; i++)
                {
                    if (primarySite != null)
                    {
                        //check if not primary site
                        if ((primarySite.Id != sites[i].Id) && !string.IsNullOrEmpty(sites[i].Name))
                        {
                            secondarySites = CombineSites(seperator + sites[i].Name, secondarySites);
                        }
                    }
                    else
                    {
                        secondarySites = CombineSites(sites[i].Name + ((i == (sites.Count - 1)) ? string.Empty : ", "), secondarySites);
                    }
                }
            }
        }
        return secondarySites;
    }


    /// <summary>
    /// Combines the primary site and secondary site into a string
    /// </summary>
    /// <param name="siteName"></param>
    /// <param name="sites"></param>
    /// <returns>Concatenated string</returns>
    private string CombineSites(string siteName, string sites)
    {
        if (!string.IsNullOrEmpty(siteName))
        {
            sites += siteName;
        }
        return sites;
    }

}
