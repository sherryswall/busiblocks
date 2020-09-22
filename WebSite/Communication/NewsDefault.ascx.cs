using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Services;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.Membership;
using BusiBlocks.Audit;
using BusiBlocks;
using Telerik.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using BusiBlocks.Versioning;
using BusiBlocks.ItemApprovalStatusLayer;

public partial class Communication_NewsDefault : UserControl
{
    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png' class='center' />";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png' class='center' />";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png' class='center' />";
    
    public struct BindingItem
    {
        public string Username { get; set; }
        public bool ViewedOrAcked { get; set; }
        public bool RequiresAck { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Control feedback = Utilities.FindControlRecursive(Page.Master, "feedback");
    }    
}
