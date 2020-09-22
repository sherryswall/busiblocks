using System;
using System.Text;

public partial class Communication_ForumNewTopic : System.Web.UI.UserControl
{
    public void LoadForum(string forumName)
    {
        BusiBlocks.CommsBlock.Forums.Category forum = GetForum();

        if (forum.AttachEnabled)
        {
            newMessage.SetAcceptedExtensions(BusiBlocks.Attachment.FileHelper.ReplaceExtensionsSets(forum.AttachExtensions));
            newMessage.SetMaxAttachSize(forum.AttachMaxSize);
            newMessage.EnabledAttach = true;
        }
        else
            newMessage.EnabledAttach = false;
    }

    private BusiBlocks.CommsBlock.Forums.Category GetForum()
    {
        BusiBlocks.CommsBlock.Forums.Category forum = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategoryByName(Request.QueryString["forum"], true);
        return forum;
    }

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            BusiBlocks.CommsBlock.Forums.Category forum = GetForum();

            var sb = new StringBuilder();

            var xhtml = new BusiBlocks.XHTMLText();
            xhtml.Load(newMessage.MessageBodyHtml);

            Exception validateError;
            if (xhtml.IsValid(forum.XHtmlMode, out validateError) == false)
                throw new BusiBlocks.TextNotValidException(validateError);

            BusiBlocks.Attachment.FileInfo attachment = null;
            //Create attachmentAccess.Access
            if (newMessage.AttachmentFile.HasFile)
                attachment = new BusiBlocks.Attachment.FileInfo(newMessage.AttachmentFile.FileName,
                                                            newMessage.AttachmentFile.PostedFile.ContentType,
                                                            newMessage.AttachmentFile.FileBytes);

            //Insert the topic
            BusiBlocks.CommsBlock.Forums.ForumsManager.CreateTopic(forum, Page.User.Identity.Name,
                                                newMessage.MessageSubject,
                                                xhtml.Xhtml,
                                                attachment, sb.ToString());

            Navigation.Communication_ForumView(forum.Name).Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Communication_ForumView(Request.QueryString["forum"]).Redirect(this);
    }
}