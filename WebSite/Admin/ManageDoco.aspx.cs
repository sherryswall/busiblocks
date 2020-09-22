using System;
using System.Web;
using BusiBlocks.DocoBlock;
using System.Web.Services;

public partial class Admin_ManageDoco : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        tree1.EnableDragAndDrop();
        tree1.PopulateTreeView<Category>(DocoManager.GetAllCategories(), true, false, string.Empty);
    }
}