using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;

public partial class Controls_DashboardMessages : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadMessages();

        sectionError.Visible = false;
    }

    private void LoadMessages()
    {
        List<Access> accessibleList = AccessManager.GetUsersAccessibleItems(Page.User.Identity.Name, BusiBlocks.ItemType.ForumTopic, BusiBlocks.AccessType.View);
        //IList<PersonType> personTypes = PersonManager.GetPersonTypesByUser(Page.User.Identity.Name);
        var itemsToList = new List<BusiBlocks.CommsBlock.Forums.Topic>();
        
        // todo MASSIVE problem here. When we create a forum topic, we store the permissions for it as the forum Category.
        // Then we retrieve all 'topics' by looking at the categories in the access table.
        // Then we are expected to display those 'topics', even though we have now lost that information (since we
        // stored them as categories.
        // We should have stored them as topicIds not categoryIds in the access table, then retrieved those topics
          
        // For the moment, filter the duplicate category items in the accessible list.
        var cleanAccessList = new List<Access>();
        foreach (Access access in accessibleList)
        {
            if (!cleanAccessList.Exists(delegate(Access a) { return a.ItemId == access.ItemId; }))
            {
                cleanAccessList.Add(access);
            }
        }

        foreach (Access access in cleanAccessList)
        {
            BusiBlocks.CommsBlock.Forums.Category category = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategory(access.ItemId);
            IList<BusiBlocks.CommsBlock.Forums.Topic> items = BusiBlocks.CommsBlock.Forums.ForumsManager.GetTopics(category, new BusiBlocks.PagingInfo(0, 1));

            foreach (BusiBlocks.CommsBlock.Forums.Topic item in items)
            {
                if (!itemsToList.Exists(delegate(BusiBlocks.CommsBlock.Forums.Topic i) { return i.Id == item.Id; }))
                {
                    //if (item.RequiresAck)
                    //{
                    //    if (item.Acknowledged)
                    //        continue;
                    //}
                    //else
                    //{
                    //    if (item.Viewed)
                    //        continue;
                    //}
                    itemsToList.Add(item);
                }
            }
        }

        lblNoResults.Visible = itemsToList.Count == 0;

        listRepeater.DataSource = itemsToList;
        listRepeater.DataBind();
    }

    protected string GetViewUrl(BusiBlocks.CommsBlock.Forums.Topic topic)
    {
        return Navigation.Communication_ForumViewTopic(topic.Id).GetClientUrl(Page, true);
    }

    protected void listRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string imageUrl = string.Empty;

        //BusiBlocks.CommsBlock.Forums.Topic item = e.Item.DataItem as BusiBlocks.CommsBlock.Forums.Topic;

        //if (item.RequiresAck)
        //{
        //    if (item.Acknowledged)
        //        imageUrl = @"~\app_themes\default\images\cube_green.png";
        //    else
        //        imageUrl = @"~\app_themes\default\images\cube_red.png";
        //}
        //else
        //{
        //    if (item.Viewed)
        //        imageUrl = @"~\app_themes\default\images\cube_green.png";
        //    else
        //        imageUrl = @"~\app_themes\default\images\cube_yellow.png";
        //}
        // todo temporary until we figure out a better image
        imageUrl = @"~\app_themes\default\icons\cube_yellow.png";

        (e.Item.FindControl("imgItem") as Image).ImageUrl = imageUrl;
    }
}