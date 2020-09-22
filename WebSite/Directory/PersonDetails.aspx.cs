using System;
using BusiBlocks.PersonLayer;
using BusiBlocks.Membership;
using System.Web.Security;

public partial class Directory_PersonDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Navigation.Directory_Search().Redirect(this);
    }
}