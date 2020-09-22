using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using BusiBlocks;
using BusiBlocks.AccessLayer;
using BusiBlocks.DocoBlock;

public partial class DocoDetails : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request["id"];
            string parentCategoryId = Request["parentCategory"];

            // Find all the categories which this user has permission to view.
            IList<Category> viewableCategories = GetAllViewableCategories();
            cmbParentCategory.DataSource = viewableCategories;
            cmbParentCategory.DataTextField = "Breadcrumb";
            cmbParentCategory.DataBind();

            //Edit
            if (id != null)
            {
                Category category = DocoManager.GetCategory(id);

                if (category.ParentCategory == null)
                {
                    cmbParentCategory.SelectedIndex = -1;
                }
                else
                {
                    IEnumerable<int> index =
                        from x in viewableCategories
                        where x.Id.Equals(category.ParentCategory.Id)
                        select viewableCategories.IndexOf(x);

                    if (index.Any())
                        cmbParentCategory.SelectedIndex = index.First();
                }

                txtDisplayName.Text = category.DisplayName;
                AccessControl1.CategoryId = category.Id;
                txtDescription.Text = category.Description;
            }
            else //New
            {
                if (!string.IsNullOrEmpty(parentCategoryId))
                {
                    // Populate the parent category combo with this category.

                    IEnumerable<int> index =
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
            IList<Category> categories = GetAllViewableCategories();

            //Edit
            if (id != null)
            {
                category = DocoManager.GetCategory(id);
                category.DisplayName = txtDisplayName.Text;
            }
            else //New
            {
                category = DocoManager.CreateCategory(txtDisplayName.Text);
            }

            Category parentCategory = categories[cmbParentCategory.SelectedIndex];
            // The parent category can be assiged as the same category, do not allow this.
            // Also do not allow the root category (with parent null) to be changed.
            if (!parentCategory.Id.Equals(category.Id) && category.ParentCategory != null)
                category.ParentCategory = categories[cmbParentCategory.SelectedIndex];
            category.Description = txtDescription.Text;
            DocoManager.UpdateCategory(category);
            IList<Access> currentAccess = AccessManager.GetItemAccess(category.Id);

            foreach (Access access in currentAccess)
            {
                AccessManager.RemoveAccess(access.Id);
            }

            foreach (Access a in AccessControl1.AccessList)
            {
                a.Id = null;
                a.ItemId = category.Id;
                a.ItemType = ItemType.DocoCategory;
                AccessManager.AddAccess(a);
            }
            
            ((IFeedback)this.Page.Master).QueueFeedback(
                 BusiBlocksConstants.Blocks.Documents.LongName,
                 "Category",
                 Feedback.Actions.Saved,
                 category.DisplayName
             );

            Navigation.Admin_ManageDoco().Redirect(this);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageDoco().Redirect(this);
    }

    public IList<Category> GetAllViewableCategories()
    {
        IList<Category> categories = DocoManager.GetAllCategories();
        return categories;
    }
}