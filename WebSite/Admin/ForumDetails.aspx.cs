using System;
using System.Globalization;
using BusiBlocks.AccessLayer;
using System.Collections.Generic;

public partial class ForumDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string forumId = Request["id"];

            //Edit
            if (forumId != null)
            {
                BusiBlocks.CommsBlock.Forums.Category forum = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategory(forumId);

                txtName.Enabled = false;

                txtName.Text = forum.Name;
                txtDisplayName.Text = forum.DisplayName;
                ctrlAccess.CategoryId = forum.Id;

                txtDescription.Text = forum.Description;

                chkEnabledAttach.Checked = forum.AttachEnabled;
                txtAttachExtensions.Text = forum.AttachExtensions;
                txtAttachMaxSize.Text = forum.AttachMaxSize.ToString(CultureInfo.InvariantCulture);
            }
            else //New
            {
                chkEnabledAttach.Checked = true;
                txtAttachExtensions.Text = BusiBlocks.Attachment.FileHelper.EXTENSIONS_ALL;
                txtAttachMaxSize.Text = "500"; //Kb
            }
        }
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        try
        {
            string forumId = Request["id"];
            BusiBlocks.CommsBlock.Forums.Category forum;

            //Edit
            if (forumId != null)
            {
                forum = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategory(forumId);                
                forum.DisplayName = txtDisplayName.Text;
            }
            else //New
            {
                forum = BusiBlocks.CommsBlock.Forums.ForumsManager.CreateCategory(txtName.Text, txtDisplayName.Text);
            }

            forum.AttachEnabled = chkEnabledAttach.Checked;
            forum.AttachExtensions = txtAttachExtensions.Text;
            forum.AttachMaxSize = int.Parse(txtAttachMaxSize.Text);

            BusiBlocks.CommsBlock.Forums.ForumsManager.UpdateCategory(forum);

            IList<Access> currentAccess = AccessManager.GetItemAccess(forum.Id);
            foreach (Access access in currentAccess) AccessManager.RemoveAccess(access.Id);
            foreach (Access a in ctrlAccess.AccessList)
            {
                a.Id = null;
                a.ItemId = forum.Id;
                a.ItemType = BusiBlocks.ItemType.ForumTopic;
                AccessManager.AddAccess(a);
            }
            Navigation.Admin_ManageComm().Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageComm().Redirect(this);
    }
}
