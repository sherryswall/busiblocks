using System;
using System.Collections.Generic;

public partial class Communication_ForumDefault : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lnkAddNew.HRef = Navigation.Communication_ForumNew().GetServerUrl(true);

            lnkManageForum.HRef = Navigation.Admin_ManageComm().GetServerUrl(true);
            linkSearch.HRef = Navigation.Communication_ForumSearch().GetServerUrl(true);

            LoadList();
        }
    }

    protected string GetForumLink(string name)
    {
        return Navigation.Communication_ForumView(name).GetClientUrl(this, true);
    }

    private void LoadList()
    {
        var listComplete = BusiBlocks.CommsBlock.Forums.ForumsManager.GetAllCategories();

        //Create a list with only the readable items
        var listReadable = new List<BusiBlocks.CommsBlock.Forums.Category>();

        foreach (BusiBlocks.CommsBlock.Forums.Category category in listComplete)
        {
            if (BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, category.Id))
                listReadable.Add(category);
        }

        listRepeater.DataSource = listReadable;
        listRepeater.DataBind();
    }
}