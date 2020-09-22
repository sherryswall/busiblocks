using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BusiBlocks.SiteLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks;

public partial class Controls_SiteDetails : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string personId = string.Empty;
        if (Request["id"] != null)
        {
            personId = Request["id"].ToString();
        }
        tree1.PopulateTreeView<Region>(SiteManager.GetAllRegions(), false, true, personId);
    }
}