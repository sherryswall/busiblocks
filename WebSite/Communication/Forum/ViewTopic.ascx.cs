using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Communication_ViewTopic : System.Web.UI.UserControl
{


    #region Properties
    /// <summary>
    /// Gets or sets the topic id to show. The value is saved in the ViewState.
    /// </summary>
    public string IdTopic
    {
        get
        {
            object val = ViewState["IdTopic"];
            return (string)val;
        }
        set { ViewState["IdTopic"] = value; }
    }
    #endregion

    private void LoadTopic(string IdTopic)
    {
        BusiBlocks.CommsBlock.Forums.Topic topic = BusiBlocks.CommsBlock.Forums.ForumsManager.GetTopic(IdTopic);
        BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;


        if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, topic.Category.Id))
            throw new BusiBlocks.InvalidGroupMembershipException();

        //if (!BusiBlocks.SecurityHelper.HasGroupAccess(Page.User.Identity.Name, topic))
        //    throw new BusiBlocks.InvalidGroupMembershipException();

        //if (BusiBlocks.SecurityHelper.CanRead(Page.User, forum, null) == false)
        //    throw new BusiBlocks.InvalidPermissionException("read forum");

        IList<BusiBlocks.CommsBlock.Forums.Message> messages = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessagesByTopic(topic);

        ExploreMessages(forum, topic, messages, null, 0);
    }

    private void ExploreMessages(BusiBlocks.CommsBlock.Forums.Category forum, BusiBlocks.CommsBlock.Forums.Topic topic,
                        IList<BusiBlocks.CommsBlock.Forums.Message> messages, string filterParent, int level)
    {
        foreach (BusiBlocks.CommsBlock.Forums.Message msg in messages)
        {
            if (string.Equals(msg.IdParentMessage, filterParent, StringComparison.InvariantCultureIgnoreCase))
            {
                Communication_ViewMessage ctlMessage = (Communication_ViewMessage)LoadControl("~/Communication/Forum/ViewMessage.ascx");
                ctlMessage.SetMessage(msg);
                ctlMessage.SetIndentLevel(level);

                Controls.Add(ctlMessage);

                ExploreMessages(forum, topic, messages, msg.Id, level + 1);
            }
        }
    }
}
