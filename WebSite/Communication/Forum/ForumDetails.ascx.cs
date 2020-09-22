using System;

public partial class Communication_ForumDetails : System.Web.UI.UserControl
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
                txtDescription.Text = forum.Description;
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

            forum.Description = txtDescription.Text;
            BusiBlocks.CommsBlock.Forums.ForumsManager.UpdateCategory(forum);
            Navigation.Communication_Default(2).Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Communication_Default(2).Redirect(this);
    }
}