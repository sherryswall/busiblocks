using System;
using System.Collections.Generic;
using BusiBlocks.CommsBlock.Forums;

public partial class Communication_DefaultForums : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lnkAddNew.Visible = BusiBlocks.SecurityHelper.CanAddNewContainer(Parent.Page.User, "comms");
            lnkAddNew.HRef = Navigation.Admin_ForumDetailsNew().GetServerUrl(true);
            linkSearch.HRef = Navigation.Forum_Search().GetServerUrl(true);

            LoadList();
        }
    }

    protected string GetForumLink(string name)
    {
        return Navigation.Forum_ViewForum(name).GetClientUrl(this, true);
    }

    private void LoadList()
    {
        IList<Category> listComplete = ForumsManager.GetAllCategories();

        listRepeater.DataSource = listComplete;
        listRepeater.DataBind();
    }
}