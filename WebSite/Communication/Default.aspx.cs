using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.CommsBlock.News;
using System.Web;
using System.Web.Services;

public partial class Communication_Announcements : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string NewsItemCategoryId = Request.QueryString["ncid"];
        string NewsItemId = Request.QueryString["niid"];
        string action = Request.QueryString["action"];
    }
}