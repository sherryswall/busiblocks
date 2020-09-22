using System;
using System.Linq;
using System.Collections.Generic;
using BusiBlocks.AccessLayer;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;

public partial class NewsDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request["id"];
            string parentCategoryId = Request["category"];

            // Find all the categories which this user has permission to view.
            IList<Category> viewableCategories = GetAllViewableCategories(id);
            cmbParentCategory.Enabled = true;
            cmbParentCategory.DataSource = viewableCategories;
            cmbParentCategory.DataTextField = "Breadcrumb";
            cmbParentCategory.DataBind();

            //Edit
            if (id != null)
            {
                Category category = NewsManager.GetCategory(id);

                if (category.ParentCategory == null)
                {
                    cmbParentCategory.SelectedIndex = -1;
                    cmbParentCategory.DataSource = new List<object>();
                    cmbParentCategory.DataBind();
                    cmbParentCategory.Enabled = false;
                }
                else
                {
                    var index =
                        from x in viewableCategories
                        where x.Id.Equals(category.ParentCategory.Id)
                        select viewableCategories.IndexOf(x);

                    if (index.Any())
                        cmbParentCategory.SelectedIndex = index.First();
                }

                txtDisplayName.Text = category.Name;
                AccessControl1.CategoryId = category.Id;
                txtDescription.Text = category.Description;
            }
            else //New
            {
                if (!string.IsNullOrEmpty(parentCategoryId))
                {
                    // Populate the parent category combo with this category.

                    var index =
                        from x in viewableCategories
                        where x.Id.Equals(parentCategoryId)
                        select viewableCategories.IndexOf(x);

                    if (index.Any())
                    {
                        cmbParentCategory.SelectedIndex = index.First();
                    }
                }
            }
        }
        txtDisplayName.Focus();
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        try
        {
            string id = Request["id"];

            Category category;

            //Edit
            if (id != null)
            {
                category = NewsManager.GetCategory(id);
                category.Name = txtDisplayName.Text;
            }
            else //New
            {
                category = NewsManager.CreateCategory(txtDisplayName.Text);
            }

            IList<Category> categories = GetAllViewableCategories(id);
            // Remove this category from the categories list.
            var indexOfThisCategory = from x in categories
                                      where x.Name.Equals(category.Name)
                                      select categories.IndexOf(x);
            if (indexOfThisCategory.Any())
                categories.RemoveAt(indexOfThisCategory.First());

            if (categories.Count > 0)
            {
                if (cmbParentCategory.SelectedIndex >= 0)
                {
                    Category parentCategory = categories[cmbParentCategory.SelectedIndex];
                    // The parent category can be assiged as the same category, do not allow this.
                    if (!parentCategory.Id.Equals(category.Id))
                        category.ParentCategory = categories[cmbParentCategory.SelectedIndex];
                }
            }
            category.Description = txtDescription.Text;
            NewsManager.UpdateCategory(category);
            IList<Access> currentAccess = AccessManager.GetItemAccess(category.Id);

            foreach (Access access in currentAccess)
            {
                AccessManager.RemoveAccess(access.Id);
            }

            foreach (Access a in AccessControl1.AccessList)
            {
                a.Id = null;
                a.ItemId = category.Id;
                a.ItemType = BusiBlocks.ItemType.NewsItem;
                AccessManager.AddAccess(a);
            }

            ((IFeedback)this.Page.Master).QueueFeedback(
                 BusiBlocksConstants.Blocks.Communication.LongName,
                 "Category",
                 Feedback.Actions.Saved,
                 category.Name
             );

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

    public IList<Category> GetAllViewableCategories(string categoryId)
    {
        IList<Category> categories = NewsManager.GetAllCategories();
        var toRemove = new List<Category>();
        foreach (Category cat in categories)
        {
            // Remove this category from the list if it is not viewable by this user.
            // Also remove the current category.
            if (!BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, cat.Id) ||
                cat.Id.Equals(categoryId == null ? string.Empty : categoryId))
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
}
