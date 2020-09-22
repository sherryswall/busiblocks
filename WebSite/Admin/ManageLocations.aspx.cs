using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Extensions;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using System.Collections.Specialized;
using Telerik.Web.UI;
using System.Web.Services;
using System.Runtime.Serialization.Json;
using BusiBlocks.PersonLayer;
using System.Web.UI.HtmlControls;

public partial class Admin_tree : System.Web.UI.Page
{
    private const string SiteList = "siteList";
    private const string QueryStringId = "id";
    public string NewLinkUrl = "";
    private const string ErrorSiteNotEmpty = "There are persons associated with this site";
    public static string FilterExpression { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        Control feedback = Utilities.FindControlRecursive(Page.Master, "feedback");
        RadAjaxManager1.AjaxSettings.AddAjaxSetting(RadGrid1, feedback);

        NewLinkUrl = Navigation.Admin_ManageSite(string.Empty).GetAbsoluteClientUrl(true);
        // Display the sites visible to this user.
        
        if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "Name";
            expression.SortOrder = GridSortOrder.Ascending;
            RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
        }
        tree1.PopulateTreeView<Region>(SiteManager.GetAllRegions(), true, false, string.Empty);
        if (!IsPostBack)
        {            
            FilterExpression = string.Empty;
        }        
    }

    protected void Bind()
    {
        IList<Site> sites = RegionVisibilityHelper.GetSitesForUser(Page.User.Identity.Name);
        if (sites != null)
        {
            RadGrid1.DataSource = sites;
        }
        else
        {
            RadGrid1.DataSource = new List<Site>();
        }
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        var siteList = new List<Site>();
        //if there's a filter present then use that.
        if (!string.IsNullOrEmpty(FilterExpression))
            BindWithFilter(siteList);
        else
        {
            SearchDefault();
        }
    }

    protected void RadGrid1_PreRender(object sender, EventArgs e)
    {
        var radGrid = (GridTableView)sender;
        foreach (GridDataItem dataRow in radGrid.Items)
        {
            dataRow.Display = !bool.Parse(dataRow["Deleted"].Text);
        }
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            var item = e.Item.DataItem as Site;
            var gridItem = (GridDataItem)e.Item;
            //Actions
            LinkButton editButton = (LinkButton)e.Item.FindControl("lnkBtnEdit");
            LinkButton deleteButton = (LinkButton)e.Item.FindControl("lnkBtnDelete");

            if (!BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, item.Id))
            {
                editButton.Visible = false;
                deleteButton.Visible = false;
            }
            else
            {
                editButton.Visible = true;
                deleteButton.Visible = true;
            }
        }
    }

    protected void RadGrid1_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == "delete")
        {
            string id = (string)e.CommandArgument;
            Site site = SiteManager.GetSiteById(id);

            string regionId = site.Region.Id;

            // If there are users attached to the site, then do not delete.
            if (PersonManager.GetAllPersonsInSite(site).Count > 0)
            {
                ((IFeedback)Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.ShortName,
                    site.GetType().Name,
                    Feedback.Actions.Error,
                    ErrorSiteNotEmpty);
            }
            else
            {
                string siteNameBeforeDelete = site.Name;
                SiteManager.DeleteSite(site);

                ((IFeedback)Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.ShortName,
                    site.GetType().Name,
                    Feedback.Actions.Deleted,
                    siteNameBeforeDelete);

                RadGrid1.DataSource = null;
                RadGrid1.Rebind();
            }           
            tree1.RebindNode(regionId, false);
        }
        else if (e.CommandName == "edit")
        {
            string id = (string)e.CommandArgument;
            Navigation.Admin_ManageSite(id).Redirect(this);
        }
        else if (e.CommandName == RadGrid.FilterCommandName)
        {
            Pair filterPair = (Pair)e.CommandArgument;
            IList<Site> items = new List<Site>();
            switch (filterPair.Second.ToString())
            {
                case "RegionName":
                    TextBox tbPattern = (e.Item as GridFilteringItem)["RegionName"].Controls[0] as TextBox;
                    FilterExpression = tbPattern.Text;                 
                    BindWithFilter(items);
                    RadGrid1.DataBind();
                    if (!string.IsNullOrEmpty(FilterExpression))
                    {
                        string itemId = SiteManager.GetAllRegionsByName((FilterExpression.Split(',').First())).First().Id;
                        tree1.RebindNode(itemId, true);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    protected void AddRegionSites(IList<Site> sitesList, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Region region = SiteManager.GetRegionByName(name);

            if (region != null)
            {
                AddSitesToList(region, sitesList);
            }
            else//this is considering the wild card scenario. pass in any text and it will search regions containing that text.
            {
                IList<Region> regions = SiteManager.GetAllRegionsByName(name);
                foreach (Region r in regions)
                {
                    if (r != null)
                    {
                        AddSitesToList(r, sitesList);
                    }
                }
            }            
        }
    }

    private void AddSitesToList(Region region, IList<Site> sitesList)
    {
        IList<Site> sites = SiteManager.GetAllSitesByRegion(region, false);
        foreach (Site site in sites)
        {
            if (sitesList.Contains(site) == false)
                sitesList.Add(site);
        }
    }

    private void BindWithFilter(IList<Site> items)
    {
        if (!string.IsNullOrEmpty(FilterExpression))
        {
            if (FilterExpression.Contains(","))
            {
                string[] values = FilterExpression.Split(',');

                foreach (string cat in values)
                {
                    AddRegionSites(items, cat.Trim());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    AddRegionSites(items, FilterExpression.Trim());
                }
            }
            RadGrid1.DataSource = items;
        }
        else
            SearchDefault();
    }

    private void SearchDefault()
    {
        Bind();
    }

    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
        RadGrid1.Rebind();
    }

    [WebMethod]
    public static bool RegionNameAvailable(string Id, string name)
    {
        Region region = SiteManager.GetRegionByName(name);

        if (region == null)
            return true;
        else if (region.Id == Id)
            return true;

        return false;
    }

}