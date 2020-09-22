using System;
using System.Web.UI.WebControls;
using BusiBlocks.Audit;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void onLoggedIn(object sender, EventArgs e)
    {
        var loginControl = sender as Login;
        AuditManager.Audit(loginControl.UserName, "Logged In", AuditRecord.AuditAction.LogOn);
    }
}