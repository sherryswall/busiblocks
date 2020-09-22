using System;
using System.Collections.Generic;
using BusiBlocks.CommsBlock.Forums;

public partial class Controls_ViewTopic : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IdTopic != null)
            LoadTopic();
    }

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

    private void LoadTopic()
    {
        Topic topic = ForumsManager.GetTopic(IdTopic);
        //BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;
        
        if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, topic.Category.Id))
            throw new BusiBlocks.InvalidGroupMembershipException();

        //if (!BusiBlocks.SecurityHelper.HasGroupAccess(Page.User.Identity.Name, topic))
        //    throw new BusiBlocks.InvalidGroupMembershipException();

        //if (BusiBlocks.SecurityHelper.CanRead(Page.User, forum, null) == false)
        //    throw new BusiBlocks.InvalidPermissionException("read forum");

        IList<Message> messages = ForumsManager.GetMessagesByTopic(topic);

        ExploreMessages(messages, null, 0);
    }

    private void ExploreMessages(IEnumerable<Message> messages, string filterParent, int level)
    {
        if (messages != null)
            foreach (Message msg in messages)
            {
                if (string.Equals(msg.IdParentMessage, filterParent, StringComparison.InvariantCultureIgnoreCase))
                {
                    var ctlMessage = (Controls_ViewMessage)LoadControl("~/Controls/ViewMessage.ascx");
                    ctlMessage.SetMessage(msg);
                    ctlMessage.SetIndentLevel(level);

                    Controls.Add(ctlMessage);

                    ExploreMessages(messages, msg.Id, level + 1);
                }
            }
    }
}
