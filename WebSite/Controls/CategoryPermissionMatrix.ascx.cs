using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.AccessLayer;
using BusiBlocks.SiteLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks;

public partial class Controls_CategoryPermissionMatrix : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void LoadPermissionsView(string categoryId, string categoryName)
    {
        lblCategoryName.Text = categoryName;

        const string allGroupsLabel = "All Groups";
        const string allLocationsLabel = "All Sites";

        IList<string> viewingItems = new List<string>();
        IList<string> editingItems = new List<string>();
        IList<string> contributingItems = new List<string>();

        if (!string.IsNullOrEmpty(categoryId))
        {
            foreach (Access access in AccessManager.GetItemAccess(categoryId))
            {
                string personType = string.Empty;
                string site = string.Empty;

                if (access.AllPersonTypes)
                    personType = allGroupsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.PersonTypeId))
                        personType = PersonManager.GetPersonTypeById(access.PersonTypeId).Name;
                }

                if (access.AllSites)
                    site = allLocationsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.SiteId))
                        site = SiteManager.GetSiteById(access.SiteId).Name;
                }

                if (!string.IsNullOrEmpty(personType) && !string.IsNullOrEmpty(site))
                {
                    string listItem = string.Format("{0} from {1}", personType, site);
                    if (access.AccessType == AccessType.View)
                    {
                        viewingItems.Add(listItem);
                    }
                    else if (access.AccessType == AccessType.Edit)
                    {
                        editingItems.Add(listItem);
                    }
                    else if (access.AccessType == AccessType.Contribute)
                    {
                        contributingItems.Add(listItem);
                    }
                }
            }
        }

        lstSummaryViewing.DataSource = viewingItems;
        lstSummaryViewing.DataBind();
        lstSummaryEditing.DataSource = editingItems;
        lstSummaryEditing.DataBind();
        lstSummaryContributing.DataSource = contributingItems;
        lstSummaryContributing.DataBind();
    }
}