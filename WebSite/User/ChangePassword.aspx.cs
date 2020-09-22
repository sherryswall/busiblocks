using System;
using System.Web.Security;
using BusiBlocks.Membership;

public partial class ChangePassword : System.Web.UI.Page
{
    private User currentUser;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            currentUser = MembershipManager.GetUserByName(Page.User.Identity.Name);
            if (currentUser.PasswordChangeRequired)
            {
                heading.InnerText = "Change password required";
                ChangePassword1.ChangePasswordTitleText = "Your password was reset recently and you are <br>required to change it before you can continue.<br>&nbsp;";
                ChangePassword1.CancelButtonText = "Log Out";
                ChangePassword1.CancelButtonClick += new EventHandler(ChangePassword1_CancelButtonClick);
            }
        }
    }

    protected void ChangePassword1_CancelButtonClick(object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();

        //Response.Resirect(FormsAuthentication.LoginUrl)
        Response.Redirect("~/");
    }
}
