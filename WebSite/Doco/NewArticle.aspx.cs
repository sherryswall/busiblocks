using System;
using System.Globalization;
using System.IO;
using System.Web.Configuration;
using System.Web.UI;
using BusiBlocks;
using BusiBlocks.DocoBlock;
using Resources;
using WebFiler;

public partial class Doco_NewArticle : Page
{
    private string CategoryId
    {
        get { return Request["id"]; }
    }

    /// <summary>
    /// Gets the root path.
    /// </summary>
    /// <value>The _root.</value>
    private string _root
    {
        get
        {
            // Is the root in the session?
            string root = UrlEncoding.Decode(Request.QueryString[Resource.QSR]);

            // If it's not there use the default.
            if (string.IsNullOrEmpty(root))
            {
                root = Server.MapPath(WebConfigurationManager.AppSettings.Get(Resource.Root));
            }

            return root;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(CategoryId))
        {
            Category category = DocoManager.GetCategory(CategoryId);
            //sectionApprove.Visible = !category.AutoApprove; -- uncomment once the Acknowledge feature is available.
            pmm.LoadPermissionsView(category.Id, category.DisplayName);
        }
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtTitle.Text.Length >= 100)
            {
                ((IFeedback)Page.Master).SetError(GetType(), "The document name must be less than 100 characters long");
                return;
            }
            Category category = DocoManager.GetCategory(CategoryId);

            //check for uploaded or online

            bool isUploaded = rbListDocoType.SelectedValue.ToLower().Equals("uploaded") ? true : false;
            bool isAckRequired = rblAcknowledge.SelectedValue.ToLower().Equals("required") ? true : false;

            bool isNumbChaps = false;

            if (!isUploaded)
                isNumbChaps = rblChapNumbs.SelectedValue.ToLower().Equals("yes") ? true : false;
            if (isUploaded)
            {
                if (!fuUpload.HasFile)
                {
                    ((IFeedback)Page.Master).SetError(GetType(), "You must add a file for an uploaded document");
                    return;
                }
            }

            if (SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, category.Id))
            {
                // Check whether this article name is in use, because there is a unique name restriction.
                Article oldArticle = DocoManager.GetArticleByName(txtTitle.Text, false);
                if (oldArticle != null)
                {
                    ((IFeedback)Page.Master).SetError(GetType(), "An document with this name already exists. You must have a unique name for the document");
                    return;
                }

                string owner = Utilities.GetUserName(Page.User.Identity.Name);
                DocoManager.CreateArticle(category, owner, txtTitle.Text, !string.IsNullOrEmpty(fuUpload.FileName) ? fuUpload.FileName : string.Empty,
                                                    txtTitle.Text, txtDescription.Text, null, isUploaded, true,
                                                    isNumbChaps, isAckRequired);

                ((IFeedback)Page.Master).QueueFeedback(
                    BusiBlocksConstants.Blocks.Documents.LongName,
                    "Document",
                    Feedback.Actions.Saved,
                    txtTitle.Text
                );
            }
            else
                throw new InvalidPermissionException("insert an article");

            if (isUploaded)
            {
                string path =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.NewObjectPath,
                        _root,
                        category.Id);

                UploadFile(category.Id);
                Navigation.Doco_Default().Redirect(this);
            }
            else
            {
                // Edit the online article.
                Navigation.Doco_ViewArticle(txtTitle.Text, 0, category.Id, "draft").Redirect(this);
            }
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Doco_Default().Redirect(this);
    }

    #region Upload Events

    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UploadFile(string id)
    {
        if (IsValid)
        {
            // Leave if no file selected.
            if (!fuUpload.HasFile)
            {
                fuUpload.Focus();
                return;
            }
            // Create the path for the new file.
            string root = Path.Combine(Path.Combine(Server.MapPath("~"), "Doco"), "Files");
            string categoryPath = Path.Combine(root, id);
            string path = Path.Combine(categoryPath, fuUpload.FileName);
            if (!Directory.Exists(categoryPath))
                Directory.CreateDirectory(categoryPath);

            // Save to the current folder.
            fuUpload.SaveAs(path);
        }
    }

    #endregion
}