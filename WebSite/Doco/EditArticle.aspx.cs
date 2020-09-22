using System;
using System.Linq;
using System.Collections.Generic;
using BusiBlocks.DocoBlock;
using System.Globalization;
using Resources;
using WebFiler;
using System.IO;
using BusiBlocks;

public partial class Doco_EditArticle : System.Web.UI.Page
{
    private string ArticleName
    {
        get { return Request["name"]; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Article article = DocoManager.GetArticleByName(ArticleName, true);
        if (!IsPostBack)
        {
            txtDescription.Text = article.Description;
            //txtEditorialComment.Text = string.Empty;
            txtTitle.Text = article.Title;
            txtOwner.Text = Utilities.GetDisplayUserName(article.Owner);
            txtAuthor.Text = Utilities.GetDisplayUserName(article.Author);

            IList<Category> viewableCategories = GetAllEditableCategories();
            cmbCategory.DataSource = viewableCategories;
            cmbCategory.DataTextField = "Breadcrumb";
            cmbCategory.DataBind();

            if (article.Category == null)
            {
                cmbCategory.SelectedIndex = -1;
            }
            else
            {
                var index =
                    from x in viewableCategories
                    where x.Id.Equals(article.Category.Id)
                    select viewableCategories.IndexOf(x);

                if (index.Any())
                    cmbCategory.SelectedIndex = index.First();
            }

            if (article.NumberedChaps)
                rblNumberedChapters.Items[0].Selected = true;
            else
                rblNumberedChapters.Items[1].Selected = true;

            if (article.RequiresAck)
                rblAcknowledge.Items[1].Selected = true;
            else
                rblAcknowledge.Items[0].Selected = true;
        }
        if (article.IsUpload)
        {
            rbListDocoType.Items[1].Selected = true;
            rowUploadFile.Visible = true;
            rowNumberedChapters.Visible = false;
            lnkFilename.Text = article.FileName;
            lnkFilename.NavigateUrl = GetViewArticleUrl(article.Id);
        }
        else
            rbListDocoType.Items[0].Selected = true;

        pmm.LoadPermissionsView(article.Category.Id, article.Category.DisplayName);
    }

    public IList<Category> GetAllEditableCategories()
    {
        IList<Category> categories = DocoManager.GetAllCategories();

        IList<Category> toRemove = new List<Category>();
        foreach (Category cat in categories)
        {
            // Remove this category from the list if it is not viewable by this user.
            if (!BusiBlocks.SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, cat.Id))
            {
                toRemove.Add(cat);
            }
        }
        foreach (Category cat in toRemove)
        {
            categories.Remove(cat);
        }
        return categories;
    }

    protected string GetViewArticleUrl(string id)
    {
        Article article = DocoManager.GetArticle(id);

        string root = Server.MapPath(Path.Combine("~/Doco/", System.Web.Configuration.WebConfigurationManager.AppSettings.Get(Resources.Resource.Root)));
        string path =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resource.NewObjectPath,
                    root,
                    article.Category.Id);

        if (!string.IsNullOrEmpty(article.FileName))
        {
            string url = string.Format(
                            CultureInfo.InvariantCulture,
                            Resource.DocoFilesLoc + Resource.FileOpen,
                            UrlEncoding.Encode(Path.Combine(path, article.FileName)));
            return url;
        }
        else
            return string.Empty;
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtTitle.Text.Length >= 100)
            {
                ((IFeedback)Page.Master).SetError(GetType(), "The announcement title must be less than 100 characters long");
                return;
            }
            Article article = DocoManager.GetArticleByName(ArticleName, true);

            //Check permissions
            if (BusiBlocks.SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, article.Category.Id) == false)
                throw new BusiBlocks.InvalidPermissionException("edit the article");

            var xhtml = new BusiBlocks.XHTMLText();
            Exception validateError;

            if (xhtml.IsValid(article.Category.XHtmlMode, out validateError) == false)
                throw new BusiBlocks.TextNotValidException(validateError);

            article.TOC = null;
            article.Description = txtDescription.Text;
            article.Title = txtTitle.Text;
            article.UpdateUser = Utilities.GetUserName(Page.User.Identity.Name);
            article.Author = txtAuthor.Text;
            article.NumberedChaps = rblNumberedChapters.SelectedValue.ToLower().Equals("yes") ? true : false;
            article.RequiresAck = rblAcknowledge.SelectedValue.ToLower().Equals("required") ? true : false;

            //if (!string.IsNullOrEmpty(txtEditorialComment.Text))
            //    article.Body += txtEditorialComment.Text + " - <b>(" + DateTime.Now + ")</b><br/>"; // currently using article.body as editorial comments.

            IList<Category> categories = GetAllEditableCategories();
            Category selectedCategory = categories[cmbCategory.SelectedIndex];
            if (!selectedCategory.Id.Equals(article.Category.Id))
            {
                Category newCategory = categories[cmbCategory.SelectedIndex];
                // Need to also physically move the document into the new category if it is an uploaded document.
                if (article.IsUpload)
                {
                    // Create the path for the new file.
                    string root = Path.Combine(Path.Combine(Server.MapPath("~"), "Doco"), "Files");
                    string oldCategoryPath = Path.Combine(root, article.Category.Id);
                    string newCategoryPath = Path.Combine(root, newCategory.Id);
                    string oldPath = Path.Combine(oldCategoryPath, article.FileName);
                    string newPath = Path.Combine(newCategoryPath, article.FileName);
                    if (!Directory.Exists(newCategoryPath))
                        Directory.CreateDirectory(newCategoryPath);
                    File.Move(oldPath, newPath);
                }
                article.Category = newCategory;
            }

            DocoManager.UpdateArticle(article, false);
            
            ((IFeedback)Page.Master).QueueFeedback(
                    BusiBlocksConstants.Blocks.Documents.LongName,
                    "Document",
                    Feedback.Actions.Saved,
                    article.Name
                );

            Navigation.Doco_Default().Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Article article = DocoManager.GetArticleByName(ArticleName, true);
        Navigation.Doco_ViewCategory(article.Category.Id).Redirect(this);
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        Article article = DocoManager.GetArticleByName(ArticleName, true);
        article.FileName = UploadFile(article.Category.Id);
        DocoManager.UpdateArticle(article, false);
    }

    protected string UploadFile(string id)
    {
        if (IsValid)
        {
            // Leave if no file selected.
            if (!fuUpload.HasFile)
            {
                fuUpload.Focus();
                return "";
            }
            // Create the path for the new file.
            string root = Path.Combine(Path.Combine(Server.MapPath("~"), "Doco"), "Files");
            string categoryPath = Path.Combine(root, id);
            string path = Path.Combine(categoryPath, fuUpload.FileName);
            if (!Directory.Exists(categoryPath))
                Directory.CreateDirectory(categoryPath);
            lnkFilename.NavigateUrl = path;
            // Save to the current folder.
            fuUpload.SaveAs(path);
            lnkFilename.Text = fuUpload.FileName;
        }
        return lnkFilename.Text;
    }
}
