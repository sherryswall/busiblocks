using System;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.Audit;
using BusiBlocks.Membership;
using BusiBlocks;

public partial class SiteMaster : MasterPage, IFeedback
{
    public Control FeedbackControl
    {
        get { return Utilities.FindControlRecursive(Page, feedback.ID); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string urlCurrent = HttpContext.Current.Request.Url.AbsolutePath;
        string urlChangePassword = Navigation.User_ChangePassword().GetServerUrl(true);
        string urlChangePasswordFilename = urlChangePassword.Substring(urlChangePassword.LastIndexOf("/") + 1);

        if (Page.User.Identity.IsAuthenticated && !urlCurrent.Contains(urlChangePasswordFilename))
        {
            User currentUser = MembershipManager.GetUserByName(Page.User.Identity.Name);

            LoginName loginName = HeadLoginView.FindControl("HeadLoginName") as LoginName;

            if (loginName != null)
            {
                loginName.FormatString = Utilities.GetDisplayUserFirstName(currentUser.Name);
            }
            if (currentUser.PasswordChangeRequired)
            {
                Response.Redirect(Navigation.User_ChangePassword().GetServerUrl(true));
            }
        }

        // Retrieve the client version number, add it to lblVersionNumber.
        string fileName = Path.Combine(Server.MapPath("~"), "Version.txt");
        if (File.Exists(fileName))
            lblVersionNumber.Text = File.ReadAllText(fileName);

        Page.Title = "BusiBlocks - " + Page.Title;

        DisplayAnyFeedback();
    }

    public void SetError(Type context, string message)
    {
        feedback.Show(Feedback.Actions.Error, message);
    }

    public void SetException(Type context, Exception ex)
    {
#if DEBUG
        throw new Exception(ex.Message, ex);
#endif
        //Don't log ThreadAbortException because is fired each time a Redirect is called
        if (!(ex is System.Threading.ThreadAbortException))
        {
            Log.Error(context, "Error", ex);

            Exception e = ((ex is HttpUnhandledException && ex.InnerException != null) ? ex.InnerException : ex);

            feedback.Show(Feedback.Actions.Exception, Utilities.FormatException(e));
        }
    }

    protected void onLoggedOut(object sender, EventArgs e)
    {
        AuditManager.Audit(Page.User.Identity.Name, "Logged Out", AuditRecord.AuditAction.LogOn);
    }


    #region Feedback
    public void ShowFeedback(string block, string obj, string action, string item)
    {
        feedback.Visible = true;
        feedback.Show(block, obj, action, item);
    }

    public void ShowFeedback(string action, string item)
    {
        feedback.Visible = true;
        feedback.Show(action, item);
    }

    private void DisplayAnyFeedback()
    {
        if(HttpContext.Current.Session != null)
        {
            if (Session["feedback"] != null)
            {
                Feedback fb = (Feedback)Session["feedback"];
                ShowFeedback(fb.Block, fb.Object, fb.Action, fb.Item);
                Session["feedback"] = null;
                Session.Remove("feedback");
            }
        }
    }

    public void QueueFeedback(string block, string obj, string action, string item)
    {
        Feedback fb = new Feedback(block, obj, action, item);
        QueueFeedback(fb);
    }

    public void QueueFeedback(string action, string item)
    {
        Feedback fb = new Feedback(action, item);
        QueueFeedback(fb);
    }

    private void QueueFeedback(Feedback fb)
    {
        Session.Remove("feedback");
        Session.Add("feedback", fb);
    }

    public void HideFeedback()
    {
        feedback.Hide();
    }

    #endregion

    protected void NavigationMenu_DataBound(object sender, EventArgs e)
    {
        if(NavigationMenu.Items.Count > 0)
            lnkHelp.Attributes["class"] = "helpLink";
        else
            lnkHelp.Attributes["class"] = "helpLink noMenu";
    }
}
