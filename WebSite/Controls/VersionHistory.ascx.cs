using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using BusiBlocks.Versioning;
using BusiBlocks.ItemApprovalStatusLayer;
using BusiBlocks.CommsBlock.News;

public partial class Controls_VersionHistory : System.Web.UI.UserControl
{
    protected string GroupId { get { return Request.QueryString["vGID"].ToString(); } }
    protected List<VersionItem> publishedVersions;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
    }
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item.ItemType != GridItemType.Footer && e.Item.ItemType != GridItemType.Header)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                string itemId = item["ItemId"].Text;

                //styling the published rows 
                if (publishedVersions.Count != 0)
                {
                    if (publishedVersions.Exists(x => x.ItemId.Equals(itemId)))
                    {
                        if (publishedVersions[0].ItemId.Equals(itemId))
                            item.CssClass = "publishedCurrent";
                        else
                            item.CssClass = "published";
                    }
                }
                item["ModifiedBy"].Text = Utilities.GetDisplayUserName(item["ModifiedBy"].Text);
            }
        }
    }
    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
        RadGrid1.DataBind();
    }

    protected string GetViewVersionUrl(string Id)
    {
        return Navigation.Communication_NewsViewItem(Id).GetServerUrl(true);
    }
    public void DisplayItemMeta(string title, string author, string categoryName)
    {
        lblTitle.Text = title;
        lblAuthor.Text = Utilities.GetDisplayUserName(author);
        lblCategory.Text = categoryName;
    }
    public void SetDataSource(IList<VersionItem> versions)
    {
        publishedVersions = new List<VersionItem>();
        //add only published versions to the list so it can style the published rows
        foreach (VersionItem version in versions)
        {
            string pubId = ItemApprovalStatusManager.GetStatusByName("Published").Id;
            Item itemX = NewsManager.GetItem(version.ItemId);
            if (itemX != null && itemX.ApprovalStatus.Id.Equals(pubId))
            {
                publishedVersions.Add(version);
            }
        }
        RadGrid1.DataSource = versions;
        RadGrid1.DataBind();
    }
}