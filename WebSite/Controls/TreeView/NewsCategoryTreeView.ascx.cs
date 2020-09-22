using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using System.Web.UI.HtmlControls;

public partial class Controls_NewsCategoryTreeView : UserControl
{
    private const string PathIconBranch = "../../app_themes/default/icons/commCat.gif";
    private const int maxLevel = 100;

    public string UrlNewsWebService = Navigation.Webservice_Communication_News().GetAbsoluteClientUrl(true);
    private BbTreeView TreeNodes;
    private string Username { get { return Page.User.Identity.Name; } }

    public event EventHandler OnNodeClicked;

    public string OnClientNodeClicked { get; set; }
    public TreeMenuMode MenuMode { get; set; }
    public TreePermissionMode PermissionMode { get; set; }
    public bool ShowItemCount { get; set; }

    public enum TreeMenuMode { Off, Browse, Administration }
    public enum TreePermissionMode { View, Edit }

    protected void Page_Load(object sender, EventArgs e)
    {
        BindServerEvents();
        BindEvents();
        BindCategoriesTreeStructure();
        BindTree();
    }

    public void BindServerEvents()
    {
        if (OnNodeClicked != null)
            treeView.OnNodeClicked += OnNodeClicked;
    }


    private void BindEvents()
    {
        if (!string.IsNullOrEmpty(OnClientNodeClicked))
            treeView.OnClientNodeClicked = OnClientNodeClicked;
    }

    private void BindTree()
    {
        treeView.BindTree(TreeNodes);
    }

    private void BindCategoriesTreeStructure()
    {
        TreeNodes = new BbTreeView();

        List<Category> allCategories = NewsManager.GetAllCategories().ToList<Category>();
        IEnumerable<Category> rootCategories = from x in allCategories where x.ParentCategory == null select x;

        List<BbTreeViewNode> nodes = new List<BbTreeViewNode>();

        foreach (Category rootCategory in rootCategories)
        {
            if (UserHasPermission(rootCategory.Id))
            {
                BbTreeViewNode node = new BbTreeViewNode(rootCategory.Id, rootCategory.Name, false, PathIconBranch, null);
                node.Menu.AddRange(CreateContextMenuContent(rootCategory.Id, rootCategory.Name));
                if (ShowItemCount)
                    node.Value += " " + GetDisplayItemCount(rootCategory);

                nodes.Add(node);
                BuildCategoriesTreeStructure(node, allCategories, rootCategory, maxLevel, 0);
            }
        }
        TreeNodes.Nodes.AddRange(nodes);
    }

    private void BuildCategoriesTreeStructure(BbTreeViewNode rootNode, IList<Category> allCategories, Category category, int maxLevel, int level)
    {
        if (level > maxLevel)
            return;

        var subCategories = from x in allCategories where (x.ParentCategory == null) ? 1 == 0 : x.ParentCategory.Equals(category) select x;

        foreach (Category subCategory in subCategories)
        {
            BbTreeViewNode categoryNode = new BbTreeViewNode(subCategory.Id, subCategory.Name, false, PathIconBranch, rootNode);
            categoryNode.Menu.AddRange(CreateContextMenuContent(subCategory.Id, subCategory.Name));
            if (ShowItemCount)
                categoryNode.Value += " " + GetDisplayItemCount(subCategory);

            if (UserHasPermission(subCategory.Id))
            {
                if (UserHasPermission(rootNode.Id))
                {
                    rootNode.ChildNodes.Add(categoryNode);
                }
                else
                {
                    BbTreeViewNode tempNode = rootNode.ParentNode;
                    bool parentWithPermissionFound = false;
                    while (!parentWithPermissionFound && (tempNode != null))
                    {
                        if (UserHasPermission(tempNode.Id))
                        {
                            tempNode.ChildNodes.Add(categoryNode);
                            parentWithPermissionFound = true;
                        }
                        else
                        {
                            tempNode = tempNode.ParentNode;
                        }
                    }
                }
            }
            BuildCategoriesTreeStructure(categoryNode, allCategories, subCategory, maxLevel, ++level);
        }
    }

    public bool EnableDragAndDrop
    {
        set { treeView.EnableDragAndDrop = value; }
    }

    public string Selected
    {
        get { return treeView.Selected; }
        set { treeView.Selected = value; }
    }

    public string SelectedValue
    {
        get { return treeView.SelectedValue; }
    }

    private string GetDisplayItemCount(Category category)
    {
        var newsItems = NewsManager.GetItems(category, false);
        int itemCount = NewsManager.CountItems(newsItems, Username);

        return String.Format("({0})", itemCount);
    }

    private bool UserHasPermission(string categoryId)
    {
        Category cat = NewsManager.GetCategory(categoryId);
        bool UserCanView = (cat.Name.Equals("All Announcements")) ? true : SecurityHelper.CanUserView(Username, categoryId);
        bool UserCanEdit = (cat.Name.Equals("All Announcements")) ? true : SecurityHelper.CanUserEdit(Username, categoryId);
        bool UserCanContribute = (cat.Name.Equals("All Announcements")) ? true : SecurityHelper.CanUserContribute(Username, categoryId);

        bool permission = false;

        if (PermissionMode == TreePermissionMode.View)
            permission = (UserCanView || UserCanEdit || UserCanContribute);
        else if (PermissionMode == TreePermissionMode.Edit)
            permission = (UserCanEdit || UserCanContribute);

        return permission;
    }

    protected void AddCategoryClick(object sender, EventArgs e)
    {
        string categoryName = popAddCategory.Value;
        string categoryId = popAddCategory.ReferrerId;

        // If there is a category with the same name, then don't continue.
        if (NewsManager.GetCategoryByName(categoryName, false) != null)
        {
            ((IFeedback)this.Page.Master).SetError(
                    GetType(), "A category called " + categoryName + " already exists"
                );
            return;
        }

        if (!string.IsNullOrEmpty(categoryId))
        {
            Category parentCategory = NewsManager.GetCategory(categoryId);
            Category newCategory = NewsManager.CreateCategory(categoryName);
            newCategory.ParentCategory = parentCategory;

            NewsManager.UpdateCategory(newCategory);

            BindCategoriesTreeStructure();

            BindTree();

            treeView.BindEvents();

            treeView.Selected = parentCategory.Id;

            ((IFeedback)this.Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.LongName,
                    parentCategory.GetType().Name,
                    Feedback.Actions.Created,
                    categoryName
                );
        }
    }

    private List<HtmlAnchor> CreateContextMenuContent(string categoryId, string categoryName)
    {
        List<HtmlAnchor> menu = new List<HtmlAnchor>();

        if (MenuMode != TreeMenuMode.Off)
        {
            if (MenuMode == TreeMenuMode.Browse)
            {
                if (SecurityHelper.CanUserEdit(Username, categoryId) || SecurityHelper.CanUserContribute(Username, categoryId))
                {
                    string addNewsItemUrl = Navigation.Communication_NewsNewItem(NewsManager.GetCategory(categoryId)).GetAbsoluteClientUrl(false);
                    HtmlAnchor createAnnItem = new HtmlAnchor { InnerText = "Create Announcement", HRef = addNewsItemUrl };
                    createAnnItem.Attributes.Add("class", "newAnnounce");
                    menu.Add(createAnnItem);
                }
            }
            else if (MenuMode == TreeMenuMode.Administration)
            {
                if (SecurityHelper.CanUserEdit(Username, categoryId))
                {
                    if (categoryName != "All Announcements")
                    {
                        string editNewsCategoryUrl = Navigation.Communication_NewsCategoryEditItem(categoryId).GetClientUrl(this, false);
                        HtmlAnchor editItem = new HtmlAnchor { InnerText = "Edit", HRef = editNewsCategoryUrl };
                        editItem.Attributes.Add("class", "edit");
                        menu.Add(editItem);

                        string deleteNewsCategoryHref = String.Format("javascript:deleteNewsCategory(\"{0}\", \"{1}\");", categoryId, categoryName);
                        HtmlAnchor deleteItem = new HtmlAnchor { InnerText = "Delete", HRef = deleteNewsCategoryHref };
                        deleteItem.Attributes.Add("class", "deleteitem");
                        menu.Add(deleteItem);
                    }
                    string addNewsCategoryHref = String.Format("javascript:addNewsCategory(\"{0}\", \"{1}\");", categoryId, categoryName);
                    HtmlAnchor addCategory = new HtmlAnchor { InnerText = "Add Category", HRef = addNewsCategoryHref };
                    addCategory.Attributes.Add("class", "addCategory");
                    menu.Add(addCategory);
                }
            }
        }
        return menu;
    }
}
