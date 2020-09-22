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
using System.ComponentModel;

public partial class Communication_ViewMessage : System.Web.UI.UserControl
{
    private const int MESSAGE_INDENT_PIXEL = 8;

    private bool mMessageLoaded = false;

    public bool ReplyLinkVisible
    {
        get
        {
            object val = ViewState["ReplyLinkVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["ReplyLinkVisible"] = value; }
    }

    public bool DeleteLinkVisible
    {
        get
        {
            object val = ViewState["DeleteVisible"];
            return val == null ? true : (bool)val;
        }
        set { ViewState["DeleteVisible"] = value; }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        { 
            // For some weird reason, onclick events are not firing
            // Instead capture the event targets to determine which events should be fired
            // I AM GOING TO COMPUTER-SCIENCE-HELL FOR THIS!!!
            string target = Request["__EVENTTARGET"];
            string id = Request["__EVENTARGUMENT"];

            if ((!string.IsNullOrEmpty(target)) && (!string.IsNullOrEmpty(id)))
            {
                if (target == "reply")
                    NewMessage(id);
                else if (target == "delete")
                    DeleteMessage(id);
            }
        }
    }

    public void LoadMessage(string MessageId)
    {
        if (mMessageLoaded == false)
        {
            BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(MessageId);
            
            LoadMessage(msg);
            
        }
    }


    public void SetMessage(BusiBlocks.CommsBlock.Forums.Message msg)
    {
        LoadMessage(msg);
    }



    private void LoadMessage(BusiBlocks.CommsBlock.Forums.Message msg)
    { 
        linkNew.HRef = "javascript:__doPostBack('reply','" + msg.Id + "')";
        linkDelete.HRef = "javascript:if(confirm('Are you sure to delete the message?')) __doPostBack('delete','" + msg.Id + "')";

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

        bool visible = BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id);

        linkNew.Visible = linkNew.Visible && ReplyLinkVisible;
        linkDelete.Visible = linkDelete.Visible && DeleteLinkVisible;
        

        //sectionDelete.Visible = (&& BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id));
        //sectionNew.Visible = (ReplyLinkVisible && BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id));

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


    private void DeleteMessage(string Id)
    {
        BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(Id);
        BusiBlocks.CommsBlock.Forums.Topic topic = msg.Topic;
        BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;

        if (!BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id))
            throw new BusiBlocks.InvalidPermissionException("delete message");

        //If there isn't a parent it is because it is the root message, in this case I delete directly the topic
        if (string.IsNullOrEmpty(msg.IdParentMessage))
        {
            BusiBlocks.CommsBlock.Forums.ForumsManager.DeleteTopic(topic);

            Navigation.Communication_ForumView(forum.Name).Redirect(this);
        }
        else
        {
            BusiBlocks.CommsBlock.Forums.ForumsManager.DeleteMessage(msg);
            Navigation.Communication_ForumViewTopic(topic.Id).Redirect(this);
        }
    }


    private void NewMessage(string ParentId)
    {
        try
        {
            BusiBlocks.CommsBlock.Forums.Message msg = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(ParentId);
            BusiBlocks.CommsBlock.Forums.Topic topic = msg.Topic;
            BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;

            if (BusiBlocks.SecurityHelper.CanUserEdit(Page.User.Identity.Name, forum.Id))
                Navigation.Communication_ForumNewMessage(ParentId).Redirect(this);
            else
                throw new BusiBlocks.InvalidPermissionException("insert new message");
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }

}
