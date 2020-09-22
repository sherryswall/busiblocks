using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using BusiBlocks.Versioning;
using BusiBlocks.Audit;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.Membership;
using BusiBlocks.AccessLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using System.Globalization;
using System.Text;
using System.Web.UI.HtmlControls;

public partial class Communication_NewsListManage : System.Web.UI.UserControl
{
    private const string ImageCubeGreen = "../app_themes/default/icons/cube_green.png";
    private const string ImageCubeRed = "../app_themes/default/icons/cube_red.png";
    private const string ImageCubeYellow = "../app_themes/default/icons/cube_yellow.png";

    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png' />";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png' />";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png' />";
    private const string HtmlSpace = "&nbsp;";

    private const string CheckOutString = "(Checked out to ";//append username.

    public static string FilterExpression { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        //RadAjaxManager1.AjaxSettings.AddAjaxSetting(RadGrid1, feedback);

        if (!Page.IsPostBack)
            FilterExpression = string.Empty;
    }

    public void Bind()
    {
        string NewsItemCategoryId = Request.QueryString["ncid"];

        if (!string.IsNullOrEmpty(NewsItemCategoryId))
        {
            Bind(NewsManager.GetCategories(NewsItemCategoryId, true));
        }
        else
        {
            //Bind View grid
            Bind(NewsManager.GetViewableCategories(Page.User.Identity.Name));
            //Bind Edit grid
        }
    }

    public void Bind(IList<Category> categories)
    {
        var items = new List<Item>();
        List<NewsGridItem> gridItems = new List<NewsGridItem>();

        //change this to include ItemTypeId(news,Doco,Forum) so it only search selected versions.
        List<VersionItem> latestDrafts = VersionManager.GetAllLatestDrafts();

        //Binding all Published news with their corresponding latest version.

        foreach (VersionItem draft in latestDrafts)
        {
            List<Item> newsItems = new List<Item>();
            //get all versions by groupID
            IList<VersionItem> versionedItems = VersionManager.GetAllVersions(draft.GroupId);

            foreach (VersionItem item in versionedItems)
            {
                //add all published ones to newsItems list
                string pubId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
                Item itemX = NewsManager.GetItem(item.ItemId);
                if (itemX != null)
                {
                    if (itemX.ApprovalStatus != null)
                    {
                        if (itemX.ApprovalStatus.Id.Equals(pubId) && item.Deleted == false)
                        {
                            newsItems.Add(itemX);
                        }
                    }
                }
            }
            //sort them by date inserted- DESC and get the first one.
            if (newsItems.Count > 0)
            {
                Item newsItem = newsItems.OrderByDescending(x => x.InsertDate).First<Item>();
                gridItems.Add(new NewsGridItem() { Draft = draft, NewsItem = newsItem });
            }
            else//no published item found - get the latest draft and its corresponding newsItem
            {
                VersionItem latestDraft = VersionManager.GetVersionByGroupId(draft.GroupId);
                Item newsItem = NewsManager.GetItem(latestDraft.ItemId);
                gridItems.Add(new NewsGridItem() { Draft = draft, NewsItem = newsItem });
            }
        }

        //do a 1-1 news & draft comparison to check whether all news have corresponding draft or not if not then add it to the grid. Pressing edit will create its first version.
        foreach (Category category in categories)
        {
            IList<Item> childItems = NewsManager.GetItems(category, false);

            foreach (Item newsItem in childItems)
            {
                VersionItem versionItem = VersionManager.GetVersionByItemId(newsItem.Id);
                if (versionItem == null && newsItem.ApprovalStatus == null)
                {
                    versionItem = new VersionItem();
                    versionItem.ItemId = newsItem.Id;
                    versionItem.GroupId = newsItem.Id;
                    gridItems.Add(new NewsGridItem() { Draft = versionItem, NewsItem = newsItem });
                }
            }
        }

        List<NewsGridItem> gridItemsPermission = new List<NewsGridItem>();
        foreach (var gi in gridItems)
        {
            if (gi.NewsItem != null)
            {
                if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, gi.NewsItem.Category.Id)
                || SecurityHelper.CanUserContribute(Page.User.Identity.Name, gi.NewsItem.Category.Id))
                    gridItemsPermission.Add(gi);
            }
        }

        RadGridManage.DataSource = gridItemsPermission;
    }

    public class RepeaterItem
    {
        public NewsGridItem NewsGridItem { get; set; }
        public string TrafficLightUrl { get; set; }
    }

    protected static string GetShortDescription(Item item)
    {
        if (item.Description == null)
            return string.Empty;

        if (item.Description.Length > 100)
            return item.Description.Substring(0, 100) + "...";
        return item.Description;
    }

    private static string GetImageUrl(bool requiresAck, bool viewedOrAcked)
    {
        Image image = new Image { CssClass = "center" };

        if (requiresAck)
        {
            if (viewedOrAcked)
                image.ImageUrl = ImageCubeGreen.ToString();
            else
                image.ImageUrl = ImageCubeRed.ToString();
        }
        else if (viewedOrAcked)
            image.ImageUrl = ImageCubeGreen.ToString();
        else
            image.ImageUrl = ImageCubeYellow.ToString();

        return image.ToString();
    }

    private string GetUserStatus(Item item, string groupId)
    {
        bool viewed = false;
        bool acknowledged = false;

        IList<VersionItem> publishedVersions = VersionManager.GetPublishedVersions(groupId);
        int currentVersPubMajPart = Int32.Parse(publishedVersions.First<VersionItem>(x => x.ItemId.Equals(item.Id)).VersionNumber.Split('.').First());
        //for a minor -> minor change

        if (publishedVersions.First<VersionItem>(x => x.ItemId.Equals(item.Id)).EditSeverity.Equals("minor"))
        {
            foreach (VersionItem version in publishedVersions)
            {
                int temp = Int32.Parse(version.VersionNumber.Split('.').First());
                if (temp == currentVersPubMajPart)
                {
                    //reset traffic light if ack required else check audit table(reset if no record exists)
                    if (!item.RequiresAck)
                    {
                        if (AuditManager.GetAuditItems(Page.User.Identity.Name, version.ItemId, AuditRecord.AuditAction.Viewed).Count > 0)
                        {
                            viewed = true;
                            break;
                        }
                    }
                    else
                    {
                        if (AuditManager.GetAuditItems(Page.User.Identity.Name, item.Id, AuditRecord.AuditAction.Acknowledged).Count > 0)
                        {
                            acknowledged = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (!item.RequiresAck)
            {
                if (AuditManager.GetAuditItems(Page.User.Identity.Name, item.Id, AuditRecord.AuditAction.Viewed).Count > 0)
                {
                    viewed = true;
                }
            }
            else
            {
                //the scenario for traffic lights when its a major publish and acknowledgement is not required is not covered here since we are not capturing that in Audit table.            
                if (AuditManager.GetAuditItems(Page.User.Identity.Name, item.Id, AuditRecord.AuditAction.Acknowledged).Count > 0)
                {
                    acknowledged = true;
                }
            }
        }
        return GetImageUrl(item.RequiresAck, (acknowledged || viewed));
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (RadGridManage.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "NewsGridItem.NewsItem.UpdateDate";
            expression.SortOrder = GridSortOrder.Descending;
            RadGridManage.MasterTableView.SortExpressions.AddSortExpression(expression);
        }

        List<NewsGridItem> items = new List<NewsGridItem>();

        if (!string.IsNullOrEmpty(FilterExpression))
            BindWithFilter(items);
        else
            SearchDefault(items);
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item.ItemType != GridItemType.Footer && e.Item.ItemType != GridItemType.Header)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                LinkButton latestVersionId = (LinkButton)item.FindControl("lnkEditItem");
                LinkButton lnkBtnDelete = (LinkButton)item.FindControl("lnkDeleteClient");
                LinkButton lnkBtnCheckIn = (LinkButton)item.FindControl("lnkBtnCheckIn");

                Label lblCheckedOut = (Label)item.FindControl("lblCheckedOut");
                VersionItem versionByItem = VersionManager.GetVersionByItemId(latestVersionId.CommandArgument.ToString());

                lnkBtnCheckIn.Visible = false;
                //if the Version/item is not checked out - check using the groupId of those versions and return user who has checkedout.
                if (versionByItem != null)
                {
                    Item NewsItem = NewsManager.GetItem(versionByItem.ItemId);
                    string checkedOutUsername = VersionManager.GetCheckedOutUser(versionByItem.GroupId); //username for comparison puposes
                    string checkedOutName = (!string.IsNullOrEmpty(checkedOutUsername)) ? Utilities.GetDisplayUserName(checkedOutUsername) : string.Empty; ;//full name for display purpose.

                    HyperLink lnkVersion = (HyperLink)item["Version"].Controls[0];//accessing link
                    lnkVersion.Text = versionByItem.VersionNumber; //add version number here

                    //display check out to string to other users if item is checked out by the user.
                    string username = Page.User.Identity.Name;

                    if (!string.IsNullOrEmpty(checkedOutUsername) && (username.ToLower() != checkedOutUsername.ToLower() && username != "admin"))
                    {
                        latestVersionId.Visible = false;
                        lnkBtnDelete.Visible = false;
                        lblCheckedOut.Text = CheckOutString + checkedOutName + ")";
                        lnkVersion.NavigateUrl = null;
                    }
                    else
                    {
                        latestVersionId.Visible = true;
                        lnkBtnDelete.Visible = true;
                        lnkVersion.NavigateUrl = Navigation.Communication_NewsVersionHistory(versionByItem.GroupId).GetServerUrl(true); //link to version history page 
                        //if user is same as owner and item is not checked out.
                        if (!string.IsNullOrEmpty(checkedOutUsername))
                        {
                            lnkBtnCheckIn.Visible = true;
                            if (checkedOutUsername.ToLower() != "admin" && username == "admin")
                            {
                                latestVersionId.Visible = false;
                                lnkBtnDelete.Visible = false;
                                lblCheckedOut.Text = CheckOutString + checkedOutName + ")";
                            }
                        }
                    }

                    // todo fix this defect

                    if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, item["CategoryId"].Text) == true)
                    {
                        if (string.IsNullOrEmpty(checkedOutUsername))
                            AddApproverButton(versionByItem.Id, versionByItem.ItemId, item);
                    }
                    else if (SecurityHelper.CanUserContribute(Page.User.Identity.Name, item["CategoryId"].Text) == true)
                    {


                        if (Page.User.Identity.Name.ToLower().Equals(item["Owner"].Text.ToLower()))
                        {
                            if (string.IsNullOrEmpty(checkedOutUsername))
                                AddApproverButton(versionByItem.Id, versionByItem.ItemId, item);
                        }
                        else
                        {
                            latestVersionId.Visible = false;
                            lnkBtnDelete.Visible = false;
                        }
                    }
                    else
                    {
                        latestVersionId.Visible = false;
                        lnkBtnDelete.Visible = false;
                    }
                }
                //disabling the view link and columns text to empty if the item hasn't been published once.
                if (item["PubStatus"].Text != "Published")
                {
                    CheckPublished(item);
                }
            }
        }
    }

    protected void AddApproverButton(string versionId, string versionItemId, GridDataItem item)
    {
        //get news item for the latest version.
        //checked the status of the item and add approve button.
        Item newsLatestEditVersion = NewsManager.GetItem(versionItemId);

        //to be approved by owner when sent by editor
        if (newsLatestEditVersion.ApprovalStatus.Name == ItemApprovalStatusManager.GetForEditApprovalStatus().Name)
        {
            if (Page.User.Identity.Name.ToLower().Equals(item["Owner"].Text.ToLower()))
            {
                ((LinkButton)item["Actions"].FindControl("lnkApproveItem")).CommandArgument = versionItemId;
                item["Actions"].FindControl("lnkApproveItem").Visible = true;
            }
        }
        //to be approved by approver
        else if (newsLatestEditVersion.ApprovalStatus.Name == ItemApprovalStatusManager.GetForApprovalStatus().Name)
        {
            if (BusiBlocks.ApproverLayer.ApproverManager.IsApprover(Utilities.GetUserName(Page.User.Identity.Name), versionId))
            {
                ((LinkButton)item["Actions"].FindControl("lnkApproveItem")).CommandArgument = versionItemId;
                item["Actions"].FindControl("lnkApproveItem").Visible = true;
            }
        }
    }

    protected void CheckPublished(GridDataItem item)
    {
        HyperLink lnkBtnTitle = (HyperLink)item.FindControl("lnkBtnTitle");
        string title = ((RepeaterItem)item.DataItem).NewsGridItem.NewsItem.Title;
        item["Title"].Controls.Add(new Label() { Text = title, ID = "lblTitle", CssClass = "test" });
        //item["Published"].Text = "-";
    }

    protected static string EditLink
    {
        get { return Navigation.Communication_NewsEditItem("IDPLACEHOLDER").GetAbsoluteClientUrl(true); }
    }

    protected string GetViewLink(string newsItemId)
    {
        return Navigation.Communication_NewsViewItem(newsItemId).GetAbsoluteClientUrl(true) + "&mode=view";
    }

    protected void RadGrid1ItemCommand(object sender, GridCommandEventArgs e)
    {
        ((IFeedback)Page.Master).HideFeedback();

        if (e.CommandName == "view")
        {
            GridDataItem item = (GridDataItem)e.Item;
            var lnkbtnView = (LinkButton)e.Item.FindControl("imgBtnView");
            AuditManager.Audit(Page.User.Identity.Name, item["Id"].Text, AuditRecord.AuditAction.Viewed);
            Navigation.Communication_NewsViewItem(lnkbtnView.CommandArgument.ToString(CultureInfo.InvariantCulture)).Redirect(this);
        }
        else if (e.CommandName == "editVersion")
        {
            //edit link to be be the VersionID
            VersionItem versionByItem = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());
            string groupId = string.Empty;
            if (versionByItem != null)
            {
                groupId = versionByItem.GroupId;
                string versionId = (VersionManager.GetVersionByGroupId(groupId)).Id;

                string checkedoutUser = VersionManager.GetCheckedOutUser(groupId);
                if ((!string.IsNullOrEmpty(checkedoutUser)) && (checkedoutUser != Page.User.Identity.Name))
                {
                    ((IFeedback)Page.Master).SetError(GetType(), "Unable to edit: News Item is checked out to " + checkedoutUser);
                }
                else
                {
                    //perform checkout by the user here
                    VersionManager.CheckOutVersion(versionId, Page.User.Identity.Name);
                    Navigation.Communication_NewsEditItem(versionId).Redirect(this);
                }
            }
            else //for existing announcements which dont have a version
            {
                //create a version out of the existing announcement

                string itemId = e.CommandArgument.ToString();
                string VersionNumber = VersionManager.GetVersionNumber(VersionType.New, string.Empty);

                VersionManager.CreateVersionItem(new VersionItem()
                {
                    ItemId = itemId,
                    GroupId = itemId,
                    VersionNumber = VersionNumber,
                    DateCreated = DateTime.Now,
                    Comments = string.Empty,
                    ModifiedBy = Utilities.GetUserName(Page.User.Identity.Name)
                });

                //get the version by itemId and then check out.
                string versionId = VersionManager.GetVersionByItemId(itemId).Id;


                VersionManager.CheckOutVersion(versionId, Utilities.GetUserName(Page.User.Identity.Name));

                Navigation.Communication_NewsEditItem(versionId).Redirect(this);
            }
        }
        else if (e.CommandName == "delete")
        {
            Item itemToDelete = NewsManager.GetItem(e.CommandArgument.ToString());
            string itemNameBeforeDelete = itemToDelete.Title;
            string groupID = VersionManager.GetVersionByItemId(itemToDelete.Id).GroupId;
            
            string checkedoutUser = VersionManager.GetCheckedOutUser(groupID);
            if ((!string.IsNullOrEmpty(checkedoutUser)) && (checkedoutUser != Page.User.Identity.Name))
            {
                ((IFeedback)Page.Master).SetError(GetType(), "Unable to delete: News Item is checked out to " + checkedoutUser);
            }
            else
            {
                string itemIdBeforeDelete = itemToDelete.Category.Id;
                NewsManager.DeleteItemByGroup(groupID);

                //Controls_TreeView tree = (Controls_TreeView)this.Parent.FindControl("tree1");
                //tree.PopulateTreeView<Category>(NewsManager.GetViewableCategories(Page.User.Identity.Name), false, false, string.Empty);
                //tree.RebindNode(itemIdBeforeDelete, false);

                ((IFeedback)Page.Master).ShowFeedback(BusiBlocksConstants.Blocks.Administration.ShortName, itemToDelete.GetType().Name, Feedback.Actions.Deleted, itemNameBeforeDelete);
            }
        }
        else if (e.CommandName == RadGrid.FilterCommandName)
        {
            Pair filterPair = (Pair)e.CommandArgument;
            List<NewsGridItem> items = new List<NewsGridItem>();

            switch (filterPair.Second.ToString())
            {
                case "Category":
                    TextBox tbPattern = (e.Item as GridFilteringItem)["Category"].Controls[0] as TextBox;
                    FilterExpression = tbPattern.Text;
                    BindWithFilter(items);
                    RadGridManage.DataBind();
                    if (!string.IsNullOrEmpty(FilterExpression))
                    {
                        var foundNews = NewsManager.GetCategoryByLikeName(FilterExpression.Split(',').First(), true);
                        int foundCount = foundNews.Count;
                        if (foundCount > 0)
                        {
                            //Controls_TreeView tree = (Controls_TreeView)this.Parent.FindControl("tree1");
                            //tree.PopulateTreeView<Category>(NewsManager.GetViewableCategories(Page.User.Identity.Name), false, false, string.Empty);
                            //string selectedNode = (foundNews.FirstOrDefault()).Id;
                            //tree.RebindNode(selectedNode, true);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else if (e.CommandName == "approveItem")
        {
            VersionItem versionByItem = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());
            string groupId = string.Empty;
            if (versionByItem != null)
            {
                groupId = versionByItem.GroupId;

                string checkedoutUser = VersionManager.GetCheckedOutUser(groupId);
                if ((!string.IsNullOrEmpty(checkedoutUser)) && (checkedoutUser != Page.User.Identity.Name))
                {
                    ((IFeedback)Page.Master).SetError(GetType(), "Unable to approve: News Item is checked out to " + checkedoutUser);
                }
                else
                {
                    string versionId = (VersionManager.GetVersionByGroupId(groupId)).Id;
                    Navigation.Communication_NewsViewItemApproval(versionId).Redirect(this);
                }
            }
            else
            {
                Navigation.Communication_NewsViewItemApproval(e.CommandArgument.ToString()).Redirect(this);
            }
        }
        else if (e.CommandName == "checkinItem")
        {
            VersionManager.CheckInVersion(e.CommandArgument.ToString());
            Navigation.Communication_News().Redirect(this);
        }
        else if (e.CommandName == "viewStatus")
        {
            // Navigate to the user view status page and pass the item id
            VersionItem versionByItem = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());
            if (versionByItem != null)
            {
                Navigation.Communication_UserViewStatus(versionByItem.ItemId, versionByItem.Id).Redirect(this);
            }
        }
        //else if (e.CommandName == "UsersViewed")
        //{
        //    Item item = NewsManager.GetItem(e.CommandArgument.ToString());
        //    VersionItem version = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());

        //    IList<User> totalUsers = GetTotalUsers(item.Category.Id);
        //    List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        //    IList<string> personsViewed = new List<string>();

        //    //get the published id.
        //    List<string> publishedIds = new List<string>();
        //    publishedIds = GetRespectivePublishedVersions(version.GroupId, version.VersionNumber);

        //    usersViewed = GetViewUsers(AuditRecord.AuditAction.Viewed, totalUsers, publishedIds);

        //    foreach (User user in usersViewed)
        //        personsViewed.Add(user.Person.Id);

        //    Session["personList"] = personsViewed;

        //    Navigation.Directory_Search().Redirect(this);
        //}
        //else if (e.CommandName == "UsersNotViewed")
        //{
        //    Item item = NewsManager.GetItem(e.CommandArgument.ToString());
        //    VersionItem version = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());

        //    IList<User> totalUsers = GetTotalUsers(item.Category.Id);
        //    List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        //    IList<string> personsNotViewed = new List<string>();

        //    //get the published id.
        //    List<string> publishedIds = new List<string>();
        //    publishedIds = GetRespectivePublishedVersions(version.GroupId, version.VersionNumber);

        //    usersViewed = GetViewUsers(AuditRecord.AuditAction.Viewed, totalUsers, publishedIds);

        //    foreach (User user in totalUsers)
        //        if (!usersViewed.Contains(user))
        //            personsNotViewed.Add(user.Person.Id);

        //    Session["personList"] = personsNotViewed;

        //    Navigation.Directory_Search().Redirect(this);
        //}
        //else if (e.CommandName == "UsersAcked")
        //{
        //    Item item = NewsManager.GetItem(e.CommandArgument.ToString());
        //    VersionItem version = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());

        //    IList<User> totalUsers = GetTotalUsers(item.Category.Id);
        //    List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        //    IList<string> personsViewed = new List<string>();

        //    //get the published id.
        //    List<string> publishedIds = new List<string>();
        //    publishedIds = GetRespectivePublishedVersions(version.GroupId, version.VersionNumber);

        //    usersViewed = GetViewUsers(AuditRecord.AuditAction.Acknowledged, totalUsers, publishedIds);

        //    foreach (User user in usersViewed)
        //        personsViewed.Add(user.Person.Id);

        //    Session["personList"] = personsViewed;

        //    Navigation.Directory_Search().Redirect(this);
        //}
        //else if (e.CommandName == "UsersNotAcked")
        //{
        //    Item item = NewsManager.GetItem(e.CommandArgument.ToString());
        //    VersionItem version = VersionManager.GetVersionByItemId(e.CommandArgument.ToString());

        //    IList<User> totalUsers = GetTotalUsers(item.Category.Id);
        //    List<User> usersViewed = new List<BusiBlocks.Membership.User>();
        //    IList<string> personsNotViewed = new List<string>();

        //    //get the published id.
        //    List<string> publishedIds = new List<string>();
        //    publishedIds = GetRespectivePublishedVersions(version.GroupId, version.VersionNumber);

        //    usersViewed = GetViewUsers(AuditRecord.AuditAction.Acknowledged, totalUsers, publishedIds);

        //    foreach (User user in totalUsers)
        //        if (!usersViewed.Contains(user))
        //            personsNotViewed.Add(user.Person.Id);

        //    Session["personList"] = personsNotViewed;

        //    Navigation.Directory_Search().Redirect(this);
        //}
    }

    /// <summary>
    /// Return all news based on category
    /// </summary>
    /// <param name="newsGridItems"></param>
    /// <param name="categoryName"></param>
    protected void AddNewsToList(List<NewsGridItem> newsGridItems, string categoryName, IList<NewsGridItem> itemList)
    {
        if (!string.IsNullOrEmpty(categoryName))
        {
            //get all grid items from viewstate which match the filter. 
            IList<NewsGridItem> gridItems = GetCategoryByLikeName(categoryName, itemList);

            foreach (NewsGridItem gItem in gridItems)
            {
                List<Item> newsItems = new List<Item>();
                //get all versions by groupID
                IList<VersionItem> versionedItems = VersionManager.GetAllVersions(gItem.Draft.GroupId);

                //get the published news item for the version and add it to the list.
                foreach (VersionItem item in versionedItems)
                {
                    //add all published ones to newsItems list
                    string pubId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
                    Item itemX = NewsManager.GetItem(item.ItemId);
                    if (itemX != null)
                    {
                        if (itemX.Category.Id.Equals(gItem.NewsItem.Category.Id))
                        {
                            if (itemX.ApprovalStatus.Id.Equals(pubId))
                            {
                                newsItems.Add(itemX);
                            }
                        }
                    }
                }
                //sort the published versions by date inserted- DESC and get the first one.
                if (newsItems.Count > 0)
                {
                    //if the grid item does not exist
                    Item newsItem = newsItems.OrderByDescending(x => x.InsertDate).First<Item>();
                    if (newsItem.Category.Id.Equals(gItem.NewsItem.Category.Id))
                    {
                        if (newsGridItems.Exists(x => x.Draft.GroupId.Equals(gItem.Draft.GroupId)) == false)
                            newsGridItems.Add(new NewsGridItem() { Draft = gItem.Draft, NewsItem = newsItem });
                    }
                }
                else//no published item found - get the latest draft and its corresponding newsItem
                {
                    VersionItem latestDraft = VersionManager.GetVersionByGroupId(gItem.Draft.GroupId);
                    Item newsItem = NewsManager.GetItem(latestDraft.ItemId);
                    if (newsItem.Category.Id.Equals(gItem.NewsItem.Category.Id))
                    {
                        if (newsGridItems.Exists(x => x.Draft.GroupId.Equals(gItem.Draft.GroupId)) == false)
                            newsGridItems.Add(new NewsGridItem() { Draft = gItem.Draft, NewsItem = newsItem });
                    }
                }
            }
        }
    }

    private void BindWithFilter(List<NewsGridItem> items)
    {
        //search for filter expression else search default which is all items.
        if (!string.IsNullOrEmpty(FilterExpression))
        {
            Bind();
            List<NewsGridItem> itemList = (List<NewsGridItem>)RadGridManage.DataSource;
            if (FilterExpression.Contains(","))
            {
                string[] values = FilterExpression.Split(',');

                foreach (string cat in values)
                {
                    AddNewsToList(items, cat.Trim(), itemList);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    AddNewsToList(items, FilterExpression.Trim(), itemList);
                }
            }
            RadGridManage.DataSource = GetAccessibleList(items);
        }
        else
        {
            SearchDefault(items);
        }
    }

    private void SearchDefault(List<NewsGridItem> gridItems)
    {
        Bind();
        List<NewsGridItem> allNews = (List<NewsGridItem>)RadGridManage.DataSource;
        if (allNews != null)
        {
            foreach (NewsGridItem item in allNews)
            {
                gridItems.Add(item);
            }
            RadGridManage.DataSource = GetAccessibleList(gridItems);
        }
        else
            RadGridManage.DataSource = new List<NewsGridItem>();
    }

    private List<NewsGridItem> GetCategoryByLikeName(string name, IList<NewsGridItem> itemList)
    {
        List<NewsGridItem> matchingCategories = new List<NewsGridItem>();

        foreach (NewsGridItem item in itemList)
        {
            if (item.NewsItem != null)
            {
                if (item.NewsItem.Category.Name.ToLower().Contains(name.ToLower()))
                {
                    matchingCategories.Add(item);
                }
            }
        }

        return matchingCategories;
    }

    protected List<RepeaterItem> GetAccessibleList(List<NewsGridItem> gridItems)
    {
        List<RepeaterItem> repeaterItemList = new List<RepeaterItem>();
        foreach (NewsGridItem gridItem in gridItems)
        {
            if (gridItem.NewsItem != null)
            {
                if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, gridItem.NewsItem.Category.Id))
                    repeaterItemList.Add(new RepeaterItem { NewsGridItem = gridItem, TrafficLightUrl = GetUserStatus(gridItem.NewsItem, gridItem.Draft.GroupId) });
                else
                {
                    if (SecurityHelper.CanUserContribute(Page.User.Identity.Name, gridItem.NewsItem.Category.Id))
                    {
                        //only add if the user is the owner
                        if (gridItem.NewsItem.Owner.Equals(Utilities.GetUserName(Page.User.Identity.Name)))
                            repeaterItemList.Add(new RepeaterItem { NewsGridItem = gridItem, TrafficLightUrl = GetUserStatus(gridItem.NewsItem, gridItem.Draft.GroupId) });
                        else if (SecurityHelper.CanUserView(Page.User.Identity.Name, gridItem.NewsItem.Category.Id))
                        {
                            if (gridItem.NewsItem.ApprovalStatus.Name.Equals("Published"))
                                repeaterItemList.Add(new RepeaterItem { NewsGridItem = gridItem, TrafficLightUrl = GetUserStatus(gridItem.NewsItem, gridItem.Draft.GroupId) });
                        }
                    }
                    else if (SecurityHelper.CanUserView(Page.User.Identity.Name, gridItem.NewsItem.Category.Id))
                    {
                        if (gridItem.NewsItem.ApprovalStatus.Name.Equals("Published"))
                            repeaterItemList.Add(new RepeaterItem { NewsGridItem = gridItem, TrafficLightUrl = GetUserStatus(gridItem.NewsItem, gridItem.Draft.GroupId) });
                    }
                }
            }
        }
        return repeaterItemList;
    }

    protected void RadGrid1_PreRender(object sender, EventArgs e)
    {
        var radGrid = (GridTableView)sender;

        foreach (GridDataItem dataRow in radGrid.Items)
            dataRow.Display = !bool.Parse(dataRow["Deleted"].Text);
    }
}