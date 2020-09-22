using System.Web.UI;
using System;
using System.Web;
using BusiBlocks;
using System.Web.UI.WebControls;

public partial class Controls_Help : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Help help = HelpHelper.GetHelp(GetPageName());

        if (!string.IsNullOrEmpty(help.Title))
            pnlTitle.Controls.Add(new LiteralControl(help.Title));
        else
            pnlTitle.Visible = false;

        if (!string.IsNullOrEmpty(help.Purpose))
            pnlPurpose.Controls.Add(new LiteralControl(help.Purpose));
        else
            pnlPurpose.Visible = false;

        if (!string.IsNullOrEmpty(help.Works))
            pnlWorks.Controls.Add(new LiteralControl(help.Works));
        else
            pnlWorks.Visible = false;

        if (!string.IsNullOrEmpty(help.Use))
            pnlUse.Controls.Add(new LiteralControl(help.Use));
        else
            pnlUse.Visible = false;
    }


    private string GetPageName()
    {
        string url = Request.RawUrl.Split('=')[0];//caters for the querystring object - removes the value.
        return Request.RawUrl.Split('=')[0];
    }
}
