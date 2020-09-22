using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.Versioning;
using BusiBlocks.CommsBlock.News;

public partial class Communication_VersionHistory : System.Web.UI.Page
{
    public static string GroupId { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            GroupId = Request.QueryString["vGID"].ToString();
            BindMetaData();
            BindVersions();
        }
    }
    protected void BindMetaData()
    {
        //get the latest version.
        VersionItem versionItem = VersionManager.GetVersionByGroupId(GroupId);
        //pass the Item object to display the Meta Info
        Item newsItem = NewsManager.GetItem(versionItem.ItemId);
        ctrlVersionHistory.DisplayItemMeta(newsItem.Title, newsItem.Author, newsItem.Category.Name);
    }
    protected void BindVersions()
    {
        IList<VersionItem> versions = VersionManager.GetAllVersions(GroupId);
        ctrlVersionHistory.SetDataSource(versions);
    }
}