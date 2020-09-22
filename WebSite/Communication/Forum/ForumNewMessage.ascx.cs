using System;

public partial class Communication_ForumNewMessage : System.Web.UI.UserControl
{
    private const string MESSAGE_RESPONSE_TAG = "RE: ";

    private string IdParentMessage {
        get { return hidParentId.Value; }
        set { hidParentId.Value = value; }
    }

    public void LoadMessage(string parentMessageId )
    {
        IdParentMessage = parentMessageId;

        BusiBlocks.CommsBlock.Forums.Message parentMessage = GetParentMessage();

        if (!IsPostBack)
        {
            string title = parentMessage.Title;

            if (title.StartsWith(MESSAGE_RESPONSE_TAG))
                newMessage.MessageSubject = title;
            else
                newMessage.MessageSubject = MESSAGE_RESPONSE_TAG + title;

            BusiBlocks.CommsBlock.Forums.Category forum = parentMessage.Topic.Category;

            if (forum.AttachEnabled)
            {
                newMessage.SetAcceptedExtensions(BusiBlocks.Attachment.FileHelper.ReplaceExtensionsSets(forum.AttachExtensions));
                newMessage.SetMaxAttachSize(forum.AttachMaxSize);
                newMessage.EnabledAttach = true;
            }
            else
                newMessage.EnabledAttach = false;
        }

        viewParentMessage.SetMessage(parentMessage);
    }

    private BusiBlocks.CommsBlock.Forums.Message GetParentMessage()
    {
        return BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessage(IdParentMessage);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Communication_ForumViewTopic(GetParentMessage().Topic.Id).Redirect(this);
    }

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            BusiBlocks.CommsBlock.Forums.Topic topic = GetParentMessage().Topic;
            BusiBlocks.CommsBlock.Forums.Category forum = topic.Category;

            var xhtml = new BusiBlocks.XHTMLText();
            xhtml.Load(newMessage.MessageBodyHtml);

            Exception validateError;
            if (xhtml.IsValid(forum.XHtmlMode, out validateError) == false)
                throw new BusiBlocks.TextNotValidException(validateError);

            BusiBlocks.Attachment.FileInfo attachment = null;
            //Create attachment
            if (newMessage.AttachmentFile.HasFile)
                attachment = new BusiBlocks.Attachment.FileInfo(newMessage.AttachmentFile.FileName,
                                                            newMessage.AttachmentFile.PostedFile.ContentType,
                                                            newMessage.AttachmentFile.FileBytes);

            //Insert the message
            BusiBlocks.CommsBlock.Forums.ForumsManager.CreateMessage(topic, IdParentMessage, Page.User.Identity.Name,
                                                    newMessage.MessageSubject,
                                                    xhtml.Xhtml,
                                                    attachment);

            Navigation.Communication_ForumViewTopic(topic.Id).Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }
}
