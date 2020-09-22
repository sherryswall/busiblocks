using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using BusiBlocks.Audit;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using Telerik.Web.UI;

public partial class Controls_NewsList : System.Web.UI.UserControl
{
    public string IdPlaceholder = "IDPLACEHOLDER";
    private const string ItemList = "itemList";

    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png'/>";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png'/>";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png'/>";

    public string CategoryId
    {
        
        get
        {
            if (ViewState["categoryId"] != null)
            {
                return ViewState["categoryId"].ToString();
            }
            return string.Empty;
        }
        set
        {
            ViewState["categoryId"] = value;
        }
    }

    public struct BindingItem
    {
        public string Username { get; set; }
        public bool ViewedOrAcked { get; set; }
        public bool RequiresAck { get; set; }
    }

    public void Bind(string categoryName)
    {
        Category category = NewsManager.GetCategoryByName(categoryName, true);

        ViewState[ItemList] = string.Join(",", NewsManager.GetItems(category, false).Select(x => x.Id).ToArray());
        RadGrid1.Rebind();
    }

    public void Bind(IList<Category> categories)
    {
        var items = new List<Item>();
        foreach (Category category in categories)
        {
            IList<Item> childItems = NewsManager.GetItems(category, false);
            items.AddRange(childItems);
        }
        ViewState[ItemList] = string.Join(",", items.Select(x => x.Id).ToArray());

        RadGrid1.DataSource = null;
        RadGrid1.Rebind();

    }

    public void Bind()
    {
        RadGrid1.Rebind();
    }

    public class RepeaterItem
    {
        public Item Item { get; set; }
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
        if (requiresAck)
        {
            if (viewedOrAcked)
                return HtmlElementImageCubeGreen;
            return HtmlElementImageCubeRed;
        }
        if (viewedOrAcked)
            return HtmlElementImageCubeGreen;
        return HtmlElementImageCubeYellow;
    }

    private string GetUserStatus(Item item)
    {
        return GetImageUrl(item.RequiresAck, item.HasUserActioned(Page.User.Identity.Name, AuditRecord.AuditAction.Acknowledged));
    }

    protected void LnkTitleClick(object source, EventArgs e)
    {
        string itemId = ((LinkButton)source).CommandArgument;
        Navigation.Communication_NewsViewItem(itemId).Redirect(this);
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        BusiBlocksTreeView categories = NewsManager.GetCategoriesItemsTree(Page.User.Identity.Name);

        object itemList = ViewState[ItemList];

        if (itemList != null)
        {
            IList<Item> items = new List<Item>();
            if (itemList.ToString().Equals("All"))
            {
                items = NewsManager.GetItems();
            }
            else
            {
                if (!string.IsNullOrEmpty(itemList.ToString()))
                {
                    string[] arr = itemList.ToString().Split(',');
                    foreach (string itemId in arr)
                    {
                        Item item = NewsManager.GetItem(itemId);
                        if (categories.Contains(item.Category.Id))
                            items.Add(NewsManager.GetItem(itemId));
                    }
                }
            }
            if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
            {
                GridSortExpression expression = new GridSortExpression();
                expression.FieldName = "Item.UpdateDate";
                expression.SortOrder = GridSortOrder.Descending;
                RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
            }
            var allowedList =
                (from item in items where SecurityHelper.CanUserView(Page.User.Identity.Name, item.Category.Id) select new RepeaterItem { Item = item, TrafficLightUrl = GetUserStatus(item) }).ToList();
            RadGrid1.DataSource = allowedList;
        }
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            var gridItem = (GridDataItem)e.Item;
         
            DateTime modifiedDate = DateTime.Parse(gridItem["Date"].Text);
            gridItem["Date"].Text = modifiedDate.ToString("dd/MM/yy");

            gridItem["Author"].Text = Utilities.GetDisplayUserName(((Item)e.Item.DataItem).Author);
        }
    }

    protected void RadGrid1ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "view")
        {
            var lnkbtnView = (LinkButton)e.Item.FindControl("imgBtnView");
            Navigation.Communication_NewsViewItem(lnkbtnView.CommandArgument.ToString(CultureInfo.InvariantCulture)).Redirect(this);
        }
    }
    
    protected void RadGrid1PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        
    }

    public string NavNewsItemView()
    {
        return Navigation.Communication_NewsViewItem(IdPlaceholder).GetAbsoluteClientUrl(false);
    }
}
