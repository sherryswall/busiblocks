using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Dashboard : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DashboardManager1.Visible = BusiBlocks.SecurityHelper.IsManager(User);
        DashboardNews1.Visible = User.IsInRole(BusiBlocks.BusiBlocksConstants.Blocks.Communication.BlockName);
        DashboardDoco1.Visible = User.IsInRole(BusiBlocks.BusiBlocksConstants.Blocks.Documents.BlockName);
        DashboardManager1.Visible = false;// User.IsInRole("manageblock");
        DashboardMessages1.Visible = User.IsInRole("forumsblock");
        DashboardTraining1.Visible = false;// User.IsInRole("trainingblock");
    }
}
