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
using BusiBlocks.PersonLayer;

public partial class Communication_NewsListRead : System.Web.UI.UserControl
{
    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png' class='center' />";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png' class='center' />";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png' class='center' />";
    public static string FilterExpression { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            FilterExpression = string.Empty;
    }

    #region GridEvents


    protected void RadGrid1ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == RadGrid.FilterCommandName)
        {
            Pair filterPair = (Pair)e.CommandArgument;
            List<NewsGridItem> items = new List<NewsGridItem>();

            switch (filterPair.Second.ToString())
            {
                case "Category":
                    TextBox tbPattern = (e.Item as GridFilteringItem)["Category"].Controls[0] as TextBox;
                    FilterExpression = tbPattern.Text;
                    BindWithFilter(items);
                    RadGridRead.DataBind();
                    break;
                default:
                    break;
            }
        }
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item.ItemType != GridItemType.Footer && e.Item.ItemType != GridItemType.Header)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                item["Owner"].Text = Utilities.GetDisplayUserName(item["Owner"].Text);

                var ngi = (NewsGridItem)item.DataItem;
                HyperLink linkTitle = (HyperLink)item["Title"].Controls[0];
                linkTitle.NavigateUrl = GetViewLink(ngi.NewsItem.Id);
            }
        }
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        List<NewsGridItem> items = new List<NewsGridItem>();

        if (!string.IsNullOrEmpty(FilterExpression))
            BindWithFilter(items);
        else
            SearchDefault(items);
    }

    #endregion

    public void Bind()
    {
        string NewsItemCategoryId = Request.QueryString["ncid"];

        if (!string.IsNullOrEmpty(NewsItemCategoryId))
        {
            Bind(NewsManager.GetCategories(NewsItemCategoryId, true));
        }
        else
        {
            Bind(NewsManager.GetAllCategories());
        }
    }

    public void Bind(IList<Category> categories)
    {
        List<Item> items = new List<Item>();
        List<NewsGridItem> gridItems = new List<NewsGridItem>();
        string approvalPublishedId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
        List<string> uniqueGroupIds = new List<string>();
       
        foreach (Category category in categories)
        {
            IList<Item> itms = (IList<Item>)NewsManager.GetItems(category, false);
            foreach (Item item in itms)
            {
                if (item.ApprovalStatus.Id.Equals(approvalPublishedId))
                {
                    VersionItem version = VersionManager.GetVersionByItemId(item.Id);
                    if (!uniqueGroupIds.Contains(version.GroupId))
                    {
                        gridItems.Add(new NewsGridItem() { Draft = version, NewsItem = item, TrafficLightUrl = GetUserStatus(item, version.GroupId) });
                        uniqueGroupIds.Add(version.GroupId);
                    }
                }
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
                if (SecurityHelper.CanUserView(Page.User.Identity.Name, gi.NewsItem.Category.Id))
                    gridItemsPermission.Add(gi);
        }

        RadGridRead.DataSource = gridItemsPermission;
    }

    private string GetUserStatus(Item item, string groupId)
    {
        TrafficLightStatus tlStatus = NewsManager.GetTrafficLight(Page.User.Identity.Name, item);
        return Utilities.GetTrafficLightImageTag(tlStatus.RequiresAck, (tlStatus.Acknowledged || tlStatus.Viewed));
    }

    protected string GetViewLink(string newsItemId)
    {
        return Navigation.Communication_NewsViewItem(newsItemId).GetAbsoluteClientUrl(true) + "&mode=view";
    }

    protected void AddNewsToList(List<NewsGridItem> newsGridItems, string categoryName, IList<NewsGridItem> itemList)
    {
        if (!string.IsNullOrEmpty(categoryName))
        {
            //get all grid items which match the filter. 
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
                            newsGridItems.Add(new NewsGridItem() { Draft = gItem.Draft, NewsItem = newsItem, TrafficLightUrl = GetUserStatus(newsItem, gItem.Draft.GroupId) });
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
            if (FilterExpression.Contains(","))
            {
                string[] values = FilterExpression.Split(',');

                Bind();
                List<NewsGridItem> itemList = (List<NewsGridItem>)RadGridRead.DataSource;
                foreach (string cat in values)
                {
                    AddNewsToList(items, cat.Trim(), itemList);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    Bind();
                    List<NewsGridItem> itemList = (List<NewsGridItem>)RadGridRead.DataSource;
                    AddNewsToList(items, FilterExpression.Trim(), itemList);
                }
            }
            RadGridRead.DataSource = items;
        }
        else
        {
            SearchDefault(items);
        }
    }

    private void SearchDefault(List<NewsGridItem> gridItems)
    {
        Bind();
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
}