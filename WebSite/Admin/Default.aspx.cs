using System;
using BusiBlocks.PersonLayer;

public partial class Admin_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Disable Manage Groups unless the user is in the Administrators group.
        Person person = PersonManager.GetPersonByUserName(Page.User.Identity.Name);
        if (!PersonManager.IsPersonInPersonTypeAdmin(person))
        {
            listManageRolesGroups.Visible = false;
            //this.divManageBlocks.Visible = false;
        }
    }
}
