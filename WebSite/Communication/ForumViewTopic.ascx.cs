using System;
using System.Collections.Generic;
using BusiBlocks.CommsBlock.Forums;

public partial class Communication_ForumViewTopic : System.Web.UI.UserControl
{
    public void LoadTopic(string idTopic)
    {
        if (idTopic != null)
        {
            Topic topic = ForumsManager.GetTopic(idTopic);
            Category forum = topic.Category;

            if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, forum.Id))
                throw new BusiBlocks.InvalidGroupMembershipException();

            lblTopic.InnerText = topic.Title;
            lnkForum.HRef = Navigation.Communication_ForumView(forum.Name).GetServerUrl(true);

            IList<Message> messages = ForumsManager.GetMessagesByTopic(topic);
            
            ExploreMessages(messages, null, 0);

            viewTopic.IdTopic = idTopic;
        }
    }

    private void ExploreMessages(IEnumerable<Message> messages, string filterParent, int level)
    {
        foreach (Message msg in messages)
        {
            if (string.Equals(msg.IdParentMessage, filterParent, StringComparison.InvariantCultureIgnoreCase))
            {
                Communication_ViewMessage ctlMessage = (Communication_ViewMessage)LoadControl("~/Communication/Forum/ViewMessage.ascx");
                ctlMessage.SetMessage(msg);
                ctlMessage.SetIndentLevel(level);
                
                Controls.Add(ctlMessage);

                ExploreMessages(messages, msg.Id, level + 1);
            }
        }
    }

}