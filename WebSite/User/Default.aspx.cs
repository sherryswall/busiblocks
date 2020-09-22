using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;

public partial class User_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser(User.Identity.Name);
        lnkUserInfo.HRef = "ChangeUserInfo.aspx?id=" + user.ProviderUserKey;
    }

    [WebMethod]
    public static bool wmIsAuthenticated()
    {
        return true;
    }
}
