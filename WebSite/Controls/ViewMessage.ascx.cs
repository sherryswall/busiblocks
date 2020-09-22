using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;

public partial class Controls_ViewMessage : System.Web.UI.UserControl
{
    private const int MESSAGE_INDENT_PIXEL = 8;

    public void LoadMessage(string MessageId)
    {
        if (mMessageLoaded == false)
        {
            BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(IdMessage);
            LoadMessage(msg);
        }
    }

    #region Properties
    [Bindable(false),
     Category("Display"),
    DefaultValue(true),
     Description("True to display the Reply link. The link is visible only if the user can insert messages.")]
    public bool ReplyLinkVisible
    {
        get
        {
            object val = ViewState["ReplyLinkVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["ReplyLinkVisible"] = value; }
    }

    [Bindable(false),
     Category("Display"),
    DefaultValue(true),
     Description("True to display the Delete link. The link is visible only if the user can delete the message.")]
    public bool DeleteLinkVisible
    {
        get
        {
            object val = ViewState["DeleteVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["DeleteVisible"] = value; }
    }

    public string IdMessage
    {
        get
        {
            object val = ViewState["IdMessage"];
            return val == null ? null : (string)val;
        }
        set { ViewState["IdMessage"] = value; }
    }
    #endregion

    /// <summary>
    /// Set the message to load. This method autmatically set also the IdMessage property.
    /// </summary>
    /// <param name="msg"></param>
    public void SetMessage(BusiBlocks.CommsBlock.Forums.Message msg)
    {
        IdMessage = msg.Id;

        LoadMessage(msg);
    }

    /// <summary>
    /// Flag that indicates if the control structure is loaded. Is used to optimize performance and don't load more than the necessary times the Message
    /// </summary>
    private bool mMessageLoaded;
    private void LoadMessage(BusiBlocks.CommsBlock.Forums.Message msg)
    {
        BusiBlocks.CommsBlock.Forums.Topic topic = msg.Topic;
        BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;


        if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, forum.Id))
            throw new BusiBlocks.InvalidPermissionException("read message");

        //if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, Profile.Locations, msg.Id))
        //    throw new BusiBlocks.InvalidGroupMembershipException();

        //Create a link (a element) that can be used for anchor (vertical navigation), note that I cannot use ASP.NET element because ASP.NET automatically change the ID adding the container id (containerid:controlid)
        string anchorId = "msg" + msg.Id; //Note: this is the format that you must use when you want to navigate to a message: es. ViewTopic.aspx?id=xxx#msgYYY
        messageTitle.InnerHtml = string.Format("<a id=\"{0}\">{1}</a>", anchorId, HttpUtility.HtmlEncode(msg.Title));


        lblAuthor.InnerText = Utilities.GetDisplayUser(msg.Owner);
        lblDate.InnerText = Utilities.GetDateTimeForDisplay(msg.InsertDate);

        sectionBody.InnerHtml = msg.Body;

        sectionDelete.Visible = (DeleteLinkVisible && BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id));
            

        sectionNew.Visible = (ReplyLinkVisible && BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id));
            

        if (msg.Attachment != null)
        {
            sectionAttachment.Visible = true;

            linkAttach.InnerHtml = HttpUtility.HtmlEncode(msg.Attachment.Name);
            linkAttach.HRef = Navigation.Communication_ForumAttach(msg.Id, true).GetServerUrl(true);
        }
        else
            sectionAttachment.Visible = false;

        //Flag the control as loaded
        mMessageLoaded = true;
    }

    public void SetIndentLevel(int level)
    {
        if (level > 0)
            controlDiv.Style[HtmlTextWriterStyle.MarginLeft] = (level * MESSAGE_INDENT_PIXEL).ToString() + "px";
    }

    protected void MessageNew_Click(object sender, EventArgs e)
    {
        try
        {
            BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(IdMessage);
            BusiBlocks.CommsBlock.Forums.Topic topic = msg.Topic;
            BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;

            if (BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id))
                Navigation.Communication_ForumNewMessage(IdMessage).Redirect(this);
            else
                throw new BusiBlocks.InvalidPermissionException("insert new message");
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }

    protected void MessageDelete_Click(object sender, EventArgs e)
    {
        BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(IdMessage);
        BusiBlocks.CommsBlock.Forums.Topic topic = msg.Topic;
        BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;

        if (!BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id))
            throw new BusiBlocks.InvalidPermissionException("delete message");

        //If there isn't a parent it is because it is the root message, in this case I delete directly the topic
        if (string.IsNullOrEmpty(msg.IdParentMessage))
        {
            BusiBlocks.CommsBlock.Forums.ForumsManager.DeleteTopic(topic);

            Navigation.Forum_ViewForum(forum.Name).Redirect(this);
        }
        else
        {
            BusiBlocks.CommsBlock.Forums.ForumsManager.DeleteMessage(msg);

            Navigation.Communication_ForumViewTopic(topic.Id).Redirect(this);
        }
    }
}
