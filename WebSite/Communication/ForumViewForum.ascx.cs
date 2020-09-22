public partial class Communication_ForumViewForum : System.Web.UI.UserControl
{

    public void BindForum(string forumName)
    {
        BusiBlocks.CommsBlock.Forums.Category category = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategoryByName(forumName, true);
        
        if (BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, category.Id) == false)
            throw new BusiBlocks.InvalidPermissionException("read forum");

        linkNewTopic.HRef = Navigation.Communication_ForumNewTopic(forumName).GetServerUrl(true);
        //linkRss.HRef = //Navigation.Forum_ForumRss(ForumName).GetServerUrl(true);
        linkRss.Visible = false;

        lblForumName.InnerText = category.DisplayName;
        lblDescription.InnerText = category.Description;

        linkNewTopic.Visible = BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, category.Id);

        linkSearch.HRef = Navigation.Communication_ForumSearch(forumName).GetServerUrl(true);
        lnkBack.HRef = Navigation.Communication_Default(2).GetServerUrl(true);

        topicList.CategoryName = forumName;
        topicList.LoadList();
    }
}
