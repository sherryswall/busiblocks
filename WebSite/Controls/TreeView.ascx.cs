using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Web.Script.Services;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.Audit;
using BusiBlocks.DocoBlock;
using BusiBlocks.PersonLayer;
using BusiBlocks.AccessLayer;
using System.Text.RegularExpressions;

public partial class Controls_TreeView : System.Web.UI.UserControl
{
    public string TreeViewType { get; set; }
    public string TreeViewTypeFullName { get; set; }
    private const string HtmlElementImageCubeGreen = "<img src='../app_themes/default/icons/cube_green.png'/>";
    private const string HtmlElementImageCubeRed = "<img src='../app_themes/default/icons/cube_red.png'/>";
    private const string HtmlElementImageCubeYellow = "<img src='../app_themes/default/icons/cube_yellow.png'/>";
    private const string ErrorCategoryNotEmpty = "There are items in this category";
    private const string ErrorRegionNotEmpty = "There are sites associated with this region";

    protected string CategoryType { get; set; }

    public class RepeaterItem
    {
        public Item Item { get; set; }
        public string TrafficLightUrl { get; set; }
    }

    public class RepeaterArticle
    {
        public Article Item { get; set; }
        public string TrafficLightUrl { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    { }

    public void PopulateTreeView<T>(IList<T> datasource, bool isAdmin, bool showNodeItems, string personId)
    {
        RadTreeView1.Nodes.Clear();
        var temp = typeof(T);

        TreeViewType = temp.Name;
        TreeViewTypeFullName = temp.FullName;

        switch (temp.Name)
        {
            case "Category":
                if (temp.FullName.Contains("News"))
                    CategoryType = "News";
                else
                    CategoryType = "Doco";
                CreateCategoryTreeView(datasource, isAdmin, showNodeItems);
                break;
            case "Region":
                CreateRegionTreeView(datasource, isAdmin, showNodeItems, personId);
                break;
            default:
                break;
        }

        // Expand the root node.
        if (RadTreeView1.Nodes.Count > 0)
            RadTreeView1.Nodes[0].Expanded = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodeType"></param>
    /// <param name="isAdmin"></param>
    /// <param name="showNodeItems"></param>
    /// <param name="userAdminList"></param>
    protected void AddParentNode<T>(T nodeType, bool isAdmin, bool showNodeItems, IList<T> userAdminList, string personId)
    {
        var tempNodeType = typeof(T);
        string nodeText = "";
        string nodeID;
        var t = nodeType;

        List<T> temp = new List<T>();

        // todo the following code is dodgey. The check for a parent needs to be handled better.
        switch (tempNodeType.Name)
        {
            case "Category":
                if (tempNodeType.FullName.Contains("News"))
                {
                    if ((nodeType as BusiBlocks.CommsBlock.News.Category).ParentCategory == null)
                        return;
                    nodeText = (nodeType as BusiBlocks.CommsBlock.News.Category).ParentCategory.Name;
                    BusiBlocks.CommsBlock.News.Category x = NewsManager.GetCategoryByName(nodeText, true);
                    t = (T)Convert.ChangeType(x, typeof(T));
                }
                else if (tempNodeType.FullName.Contains("Doco"))
                {
                    if ((nodeType as BusiBlocks.DocoBlock.Category).ParentCategory == null)
                        return;
                    nodeText = (nodeType as BusiBlocks.DocoBlock.Category).ParentCategory.DisplayName;
                    nodeID = (nodeType as BusiBlocks.DocoBlock.Category).ParentCategory.Id;
                    BusiBlocks.DocoBlock.Category x = DocoManager.GetCategory(nodeID);
                    t = (T)Convert.ChangeType(x, typeof(T));
                }
                break;
            case "Region":
                nodeText = (nodeType as Region).ParentRegion.Name;
                break;
            default:
                break;
        }

        if (RadTreeView1.FindNodeByText(nodeText) != null)
        {
            AddSubNode(nodeType, isAdmin, showNodeItems, userAdminList, personId);
        }
        else
        {
            //Add the parent node that does not have a parent node to a list.
            temp.Add(nodeType);
            AddParentNode(t, isAdmin, showNodeItems, userAdminList, personId);

            foreach (T item in temp)
            {
                AddSubNode(item, isAdmin, showNodeItems, userAdminList, personId);
            }
        }
    }

    protected void AddSubNode<T>(T nodeType, bool isAdmin, bool showNodeItems, IList<T> userAdminList, string personId)
    {
        var tempNodeType = typeof(T);
        RadTreeNode node = new RadTreeNode();

        switch (tempNodeType.Name)
        {
            case "Category":
                if (tempNodeType.FullName.Contains("News"))
                {
                    BusiBlocks.CommsBlock.News.Category category = nodeType as BusiBlocks.CommsBlock.News.Category;
                    CreateCategoryNode(category, isAdmin);
                }
                else if (tempNodeType.FullName.Contains("Doco"))
                {
                    BusiBlocks.DocoBlock.Category category = nodeType as BusiBlocks.DocoBlock.Category;
                    CreateCategoryNode(category, isAdmin);
                }
                break;
            case "Region":
                Region region = nodeType as Region;
                IList<Region> userAdminRegions = userAdminList as IList<Region>;
                CreateRegionNode(region, isAdmin, userAdminRegions, personId);
                if (showNodeItems)
                    CreateRegionNodeItems(region, personId);
                break;
            default:
                break;
        }
    }

    protected void Node_DraggedDropped<T>(object sender, RadTreeNodeDragDropEventArgs e)
    {
        string destNode = e.DestDragNode.Text;
        RadTreeView1.Nodes.FindNodeByText(destNode).ExpandParentNodes();
    }

    protected void CreateRegionNode(Region region, bool isAdminBlock, IList<Region> userAdminRegions, string personId)
    {
        RadTreeNode node = new RadTreeNode(region.Name, region.Id);
        node.ImageUrl = "../App_Themes/Default/images/region.png";

        string id = userAdminRegions.First<Region>(x => x.Id.Equals(region.Id)).Id;
        bool hasAccess = false;
        if (!string.IsNullOrEmpty(id))
            hasAccess = true;

        Panel p = new Panel();
        p.ID = "pnlNode";
        p.CssClass = "tvNode";

        Label lblName = new Label();
        lblName.ID = "lblDivNodeName";
        lblName.Text = region.Name + "&nbsp;(" + SiteManager.GetAllSitesByRegion(region, true).Count.ToString() + ")&nbsp;";

        Label lblContextMenu = new Label();
        lblContextMenu.ID = "lblContextMenu";
        lblContextMenu.CssClass = region.Id + "_tvContext hideElement";
        string regName = "\'" + Utilities.EscapeSpecialCharacters(region.Name.ToString()) + "\'";

        //create menu for the admin block
        if (isAdminBlock && hasAccess)
        {
            lblContextMenu.Text = "<a href=# class='edit' onclick=\"showEditRegionPopup('Region','" + region.Id.ToString() + "'," + regName + ",";

            string ParentCatId = string.Empty;
            string regParName = string.Empty;
            if (region.ParentRegion != null)
            {
                regParName = "\'" + Utilities.EscapeSpecialCharacters(region.ParentRegion.Name.ToString()) + "\'";
                lblContextMenu.Text += regParName + ",'Edit');\">Edit</a>&nbsp;";
                ParentCatId = region.ParentRegion.Id;
            }  //edit link
            else
                lblContextMenu.Text += "''" + "','Edit');\">Edit</a>&nbsp;";//edit link

            lblContextMenu.Text += "<a href=# class='deleteitem' onclick=\"showDeleteRegionPopup('Region','" + region.Id.ToString() + "'," + regName + ",'','Delete');\">Delete</a>&nbsp;" +//delete link
            "<a href=# class='addRegion' onclick=\"showAddRegionPopup('Region','" + region.Id.ToString() + "',''," + regName + ",'Add');\">Create Region</a>&nbsp;" +
            "<a href=# class='addSite' onclick=\"showAddSitePopup('Site','" + region.Id.ToString() + "'," + regName + ",'','Site');\">" +
                    "Create Site</a>";
        }
        //create menu for the non-admin block
        else
        {
            if (hasAccess)
            {
                lblContextMenu.Text = "<a href=# onclick=\"addSites('" + personId + "','" +
                        region.Id.ToString() + "'," + regName + ",'Region');\">" +
                    "<img src='../App_Themes/Default/icons/add.png' />Add Region Access</a>";
            }

            //don't allow drag and drop
            RadTreeView1.EnableDragAndDrop = false;
            RadTreeView1.EnableDragAndDropBetweenNodes = false;
        }
        
        p.Controls.Add(lblName);
        p.Controls.Add(lblContextMenu);

        node.Controls.Add(p);

        if (region.ParentRegion != null)
        {
            if (userAdminRegions.FirstOrDefault<Region>(x => x.Id.Equals(region.ParentRegion.Id)) != null)
                RadTreeView1.FindNodeByValue(region.ParentRegion.Id).Nodes.Add(node);
            else
                RadTreeView1.Nodes.Add(node);
        }
        else
            RadTreeView1.Nodes.Add(node);
    }

    protected void CreateRegionNodeItems(Region region, string personId)
    {
        IList<Site> sites = SiteManager.GetAllSitesByRegion(region, false);
        string parentRegionName = region.Name;
        string parentRegionId = region.Id;

        foreach (Site site in sites)
        {
            RadTreeNode node = new RadTreeNode(site.Name, site.Id);
            node.ImageUrl = "../App_Themes/Default/icons/site.png";

            Panel p = new Panel();
            p.ID = "pnlNode";
            p.CssClass = "tvNode";
            
            Label lblName = new Label();
            lblName.ID = "lblDivNodeName";
            lblName.Text = site.Name + "&nbsp;";

            Label lblContextMenu = new Label();
            lblContextMenu.CssClass = site.Id + "_tvContext hideElement"; 
            lblContextMenu.Text ="<a href=# onclick=\"addSites('" + personId + "','" + site.Id.ToString() + "','" + site.Name.ToString() + "','Site');\">" +
                "<img src='../App_Themes/Default/icons/add.png' />Add Site Access</a>";

            p.Controls.Add(lblName);
            p.Controls.Add(lblContextMenu);
            node.Controls.Add(p);

            if (region != null)
                RadTreeView1.FindNodeByValue(parentRegionId).Nodes.Add(node);
        }
    }

    protected void CreateCategoryNode<T>(T category, bool isAdmin)
    {
        var tempNode = typeof(T);
        string nodeName = string.Empty;
        string nodeId = string.Empty;
        string nodeParentName = string.Empty;
        string nodeParentId = string.Empty;
        string createLink = string.Empty;
        string createLinkText = string.Empty;
        bool hasAccess = false;
        var tempCategory = category;
        string createLinkStyle = string.Empty;

        if (tempNode.FullName.Contains("News"))
        {
            nodeName = (category as BusiBlocks.CommsBlock.News.Category).Name;
            nodeId = (category as BusiBlocks.CommsBlock.News.Category).Id;
            if ((category as BusiBlocks.CommsBlock.News.Category).ParentCategory != null)
            {
                nodeParentName = (category as BusiBlocks.CommsBlock.News.Category).ParentCategory.Name;
                nodeParentId = (category as BusiBlocks.CommsBlock.News.Category).ParentCategory.Id;
            }
            BusiBlocks.CommsBlock.News.Category x = NewsManager.GetCategoryByName(nodeName, true);
            tempCategory = (T)Convert.ChangeType(x, typeof(T));

            createLink = Navigation.Communication_NewsCategoryNewItem(x).GetAbsoluteClientUrl(false);
        }
        else if (tempNode.FullName.Contains("Doco"))
        {
            nodeName = (category as BusiBlocks.DocoBlock.Category).DisplayName;
            nodeId = (category as BusiBlocks.DocoBlock.Category).Id;
            if ((category as BusiBlocks.DocoBlock.Category).ParentCategory != null)
            {
                nodeParentName = (category as BusiBlocks.DocoBlock.Category).ParentCategory.DisplayName;
                nodeParentId = (category as BusiBlocks.DocoBlock.Category).ParentCategory.Id;
            }
            BusiBlocks.DocoBlock.Category x = DocoManager.GetCategory(nodeId);
            tempCategory = (T)Convert.ChangeType(x, typeof(T));

            createLink = Navigation.Doco_NewArticle(nodeId).GetAbsoluteClientUrl(false);
        }

        if (RadTreeView1.FindNodeByText(nodeName) == null)
        {
            RadTreeNode node = new RadTreeNode(nodeName, nodeId);

            string itemCount = string.Empty;

            //create node icons and links for new item.
            if (CategoryType == "News")
            {
                node.ImageUrl = "../app_themes/default/icons/commCat.gif";
                var cat = NewsManager.GetCategory(nodeId);
                var newsItems = NewsManager.GetItems(cat, false);
                itemCount = NewsManager.CountItems(newsItems, Page.User.Identity.Name).ToString();

                createLinkText = "Announcement";
                createLinkStyle = "newAnnounce";
                createLink = Navigation.Communication_NewsNewItem(NewsManager.GetCategory((category as BusiBlocks.CommsBlock.News.Category).Id)).GetAbsoluteClientUrl(false);
            }
            else if (CategoryType == "Doco")
            {
                node.ImageUrl = "../App_Themes/Default/icons/folder.png";
                itemCount = DocoManager.CountItems(nodeId, Page.User.Identity.Name, Utilities.GetUserName(Page.User.Identity.Name)).ToString();
                createLinkStyle = "newDoc";
                createLinkText = "Document";
            }

            Panel p = new Panel();
            p.ID = "pnlNode";
            p.CssClass = "tvNode";

            Label lblName = new Label();
            lblName.ID = "lblDivNodeName";
            lblName.Text = nodeName + "&nbsp;(" + itemCount + ")&nbsp;";

            Label lblContextMenu = new Label();
            lblContextMenu.ID = "lblContextMenu";
            lblContextMenu.CssClass = nodeId + "_tvContext hideElement";

            //if user can edit then no need to check for contribute.
            hasAccess = SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, nodeId);

            nodeName = "\'" + Utilities.EscapeSpecialCharacters(nodeName) + "\'";

            if (isAdmin)
            {
                string url = tempNode.FullName.Contains("News") ? Navigation.Communication_NewsCategoryEditItem(nodeId).GetClientUrl(this, false) : Navigation.Admin_DocoDetails(nodeId).GetClientUrl(this, false);
                lblContextMenu.Text = "<a href=" + url + " class='edit'>Edit</a>&nbsp;";

                string ParentCatId = string.Empty;
                //if all categories are listed as one node i.e. as list then the root is always null.(caters for any node with null parent)
                if (nodeParentId != string.Empty)
                {
                    ParentCatId = nodeParentId;
                }

                lblContextMenu.Text += "<a href=# class='deleteitem' onclick=\"showDeleteCategoryPopup('Category','" + nodeId + "'," + nodeName + ",'','Delete');\">Delete</a>&nbsp;" +//delete link
                "<a href=# class='addCategory' onclick=\"showAddCategoryPopup('Category','" + nodeId + "',''," + nodeName + ",'Add');\">Add Category</a>";
            }
            else
            {
                if (hasAccess)
                {
                    lblContextMenu.Text = "<a href='" + createLink + "' class='" + createLinkStyle + "'>Create " + createLinkText + "</a>";
                }
            }

            p.Controls.Add(lblName);
            p.Controls.Add(lblContextMenu);
            node.Controls.Add(p);

            RadTreeNode tNode = RadTreeView1.FindNode(x => x.Text.Contains(nodeParentName));

            if (tNode == null)
            {
                RadTreeView1.Nodes.Add(node);
            }
            else if (nodeParentId != string.Empty)
            {
                if (tNode != null)
                    RadTreeView1.FindNodeByText(nodeParentName).Nodes.Add(node);
            }
            else
            {
                RadTreeView1.Nodes.Add(node);
            }
        }
    }

    private class SortStruct
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public object Original { get; set; }
        public Type OriginalType { get; set; }
    }

    private IList<SortStruct> Add0Levels(IList<SortStruct> sortList, bool isAdmin, bool showNodeItems)
    {
        IList<SortStruct> oLevels = new List<SortStruct>();
        // An 0 level item is one whose parent is null, or who's parent is not in the sort list.
        foreach (SortStruct item in sortList)
        {
            if (string.IsNullOrEmpty(item.ParentId) || sortList.FirstOrDefault(x => x.Id.Equals(item.ParentId)) == null)
            {
                oLevels.Add(item);
            }
        }

        // Add 0levels to the tree view. 
        foreach (SortStruct sortItem in oLevels)
        {
            Type originalType = sortItem.OriginalType;

            if (sortItem.OriginalType.FullName.Contains("News"))
            {
                BusiBlocks.CommsBlock.News.Category category = sortItem.Original as BusiBlocks.CommsBlock.News.Category;
                CreateCategoryNode(category, isAdmin);
            }
            else if (sortItem.OriginalType.FullName.Contains("Doco"))
            {
                BusiBlocks.DocoBlock.Category category = sortItem.Original as BusiBlocks.DocoBlock.Category;
                CreateCategoryNode(category, isAdmin);
            }
        }

        // Databind so that the children can find their parents in the tree view.
        RadTreeView1.DataBind();
        return oLevels;
    }

    /// <summary>
    /// Using the category data source, create the tree. Any categories with no parent will become root
    /// nodes. Any categories with a parent will be added to its parent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="categoryDataSource"></param>
    /// <param name="isAdmin"></param>
    /// <param name="showNodeItems"></param>
    protected void CreateCategoryTreeView<T>(IList<T> categoryDataSource, bool isAdmin, bool showNodeItems)
    {
        var t = typeof(T);
        //add root node
        var allNodes = from nodes in categoryDataSource select nodes;

        // Create the sort list.
        List<SortStruct> sortList = new List<SortStruct>();
        foreach (var r in allNodes)
        {
            if (r.GetType().FullName.Contains("News"))
            {
                BusiBlocks.CommsBlock.News.Category category = r as BusiBlocks.CommsBlock.News.Category;
                string parentId = (category.ParentCategory != null) ? category.ParentCategory.Id : string.Empty;
                sortList.Add(new SortStruct() { Id = category.Id, ParentId = parentId, Original = category, OriginalType = category.GetType() });
            }
            else if (r.GetType().FullName.Contains("Doco"))
            {
                BusiBlocks.DocoBlock.Category category = r as BusiBlocks.DocoBlock.Category;
                string parentId = (category.ParentCategory != null) ? category.ParentCategory.Id : string.Empty;
                sortList.Add(new SortStruct() { Id = category.Id, ParentId = parentId, Original = category, OriginalType = category.GetType() });
            }
        }

        int circuitBreaker = 0;
        do
        {
            // Find the o-level objects.
            IList<SortStruct> oLevels = Add0Levels(sortList, isAdmin, showNodeItems);
            foreach (SortStruct item in oLevels)
                sortList.Remove(item);
            circuitBreaker++;
        }
        while (sortList.Count > 0 && circuitBreaker < 100);
    }

    protected void CreateRegionTreeView<T>(IList<T> regionDatasource, bool isAdmin, bool showNodeItems, string personId)
    {
        IList<Region> regions = (IList<Region>)regionDatasource;
        Person person = PersonManager.GetPersonByUserName(Page.User.Identity.Name);
        IList<Region> userRegions = PersonManager.GetAdminRegionsByPerson(person, true);

        bool isSuperAdmin = false;
        IList<PersonType> myGroups = PersonManager.GetPersonTypesByUser(Page.User.Identity.Name);

        if (myGroups.FirstOrDefault(x => x.Name.Equals(BusiBlocksConstants.AdministratorsGroup)) != null)
        {
            isSuperAdmin = true;
            userRegions = SiteManager.GetAllRegions();
        }
        //add root node
        var rootNode = from x in regions
                       where x.ParentRegion == null
                       select x;

        IEnumerable<Region> rootNodes = rootNode;

        foreach (Region r in rootNodes)
        {
            if (isSuperAdmin == false)
            {
                if (userRegions.FirstOrDefault<Region>(x => x.Id.Equals(r.Id)) != null)
                    AddSubNode(r, isAdmin, showNodeItems, userRegions, personId);
            }
            else
            {
                AddSubNode(r, isAdmin, showNodeItems, userRegions, personId);
            }
        }
        RadTreeView1.DataBind();

        //add sub nodes
        var subNode = from x in regions
                      where x.ParentRegion != null
                      select x;

        IEnumerable<Region> subNodes = subNode;

        foreach (Region r in subNodes)
        {
            if (isSuperAdmin == false)
            {
                if (userRegions.FirstOrDefault<Region>(x => x.Id.Equals(r.Id)) != null)
                    AddNodes(r, isAdmin, showNodeItems, userRegions, personId);
            }
            else
                AddNodes(r, isAdmin, showNodeItems, userRegions, personId);
        }
    }

    private void AddNodes<T>(T node, bool isAdmin, bool showNodeItems, IList<T> userAdminList, string personId)
    {
        var t = typeof(T);
        var nodes = from items in userAdminList
                    select items;

        if (t.FullName.Contains("Region"))
        {
            Region r = node as Region;
            IList<Region> userRegionList = userAdminList as IList<Region>;
            if (r.ParentRegion != null)
            {
                if (RadTreeView1.FindNodeByText(r.ParentRegion.Name) != null)
                {
                    AddSubNode(r, isAdmin, showNodeItems, userRegionList, personId);
                }
                else
                {
                    IList<Region> regions = new List<Region>();

                    foreach (var item in nodes)
                    {
                        regions.Add(item as Region);
                    }
                    if (regions.Contains(r.ParentRegion))//if sub node's parent not add parent nodes first!.
                        AddParentNode(r.ParentRegion, isAdmin, showNodeItems, userRegionList, personId);
                    //add the node again after adding parent nodes.
                    AddSubNode(r, isAdmin, showNodeItems, userRegionList, personId);
                }
            }
        }
    }

    private static string GetImageUrl(bool requiresAck, bool viewedOrAcked)
    {
        if (requiresAck)
        {
            if (viewedOrAcked)
                return HtmlElementImageCubeGreen;
            return HtmlElementImageCubeRed;
        }
        if (viewedOrAcked)
            return HtmlElementImageCubeGreen;
        return HtmlElementImageCubeYellow;
    }

    private string GetUserStatus(Item item)
    {
        return GetImageUrl(item.RequiresAck, item.HasUserActioned(Page.User.Identity.Name, AuditRecord.AuditAction.Acknowledged));
    }

    private string GetUserStatus(Article item)
    {
        return GetImageUrl(item.RequiresAck, item.HasUserActioned(Page.User.Identity.Name, AuditRecord.AuditAction.Acknowledged));
    }

    protected void AddCategoryClick(object sender, EventArgs e)
    {
        string categoryName = popAddCategory.Value;
        string categoryId = popAddCategory.ReferrerId;
        string categoryTypeName = string.Empty;

        // Need to figure out if this is a comms block category or a doco block category.
        if (!string.IsNullOrEmpty(categoryId))
        {
            BusiBlocks.CommsBlock.News.Category news = null;
            try
            {
                news = NewsManager.GetCategory(categoryId);
                categoryTypeName = news.GetType().Name;
            }
            catch (NewsCategoryNotFoundException) { }
            if (news != null)
            {
                BusiBlocks.CommsBlock.News.Category childCategory = NewsManager.CreateCategory(categoryName);
                childCategory.ParentCategory = news;
                NewsManager.UpdateCategory(childCategory);
                this.PopulateTreeView<BusiBlocks.CommsBlock.News.Category>(NewsManager.GetAllCategories(), true, false, string.Empty);
            }
            else
            {
                BusiBlocks.DocoBlock.Category doco = null;
                try
                {
                    doco = DocoManager.GetCategory(categoryId);
                    categoryTypeName = doco.GetType().Name;
                }
                catch (DocoCategoryNotFoundException) { }
                if (doco != null)
                {
                    BusiBlocks.DocoBlock.Category childDocoCategory = DocoManager.CreateCategory(categoryName);
                    childDocoCategory.ParentCategory = doco;
                    DocoManager.UpdateCategory(childDocoCategory);
                    this.PopulateTreeView<BusiBlocks.DocoBlock.Category>(DocoManager.GetAllCategories(), true, false, string.Empty);
                }
            }
            RadTreeView1.DataBind();
            RadTreeView1.FindNodeByText(categoryName).ExpandParentNodes();
            RadTreeView1.FindNodeByText(categoryName).Selected = true;

            ((IFeedback)this.Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.LongName,
                    categoryTypeName,
                    Feedback.Actions.Created,
                    categoryName
                );
        }
    }

    protected void AddRegionClick(object sender, EventArgs e)
    {
        Region newRegion = new Region();
        newRegion.Name = popAddRegion.Value;

        newRegion.ParentRegion = SiteManager.GetRegionById(popAddRegion.ReferrerId);
        IList<RegionType> regionTypes = SiteManager.GetAllRegionTypes();
        RegionType regionType = regionTypes.Last();
        newRegion.RegionType = regionType;
        if (!SiteManager.IsRegionUnique(newRegion))
        {
            ((IFeedback)this.Page.Master).SetError(GetType(), "The region is not unique within its parent region");
            return;
        }
        SiteManager.CreateRegion(newRegion);

        PopulateTreeView<Region>(SiteManager.GetAllRegions(), true, false, string.Empty);
        RadTreeView1.DataBind();

        RadTreeView1.FindNodeByText(newRegion.Name).ExpandParentNodes();
        RadTreeView1.FindNodeByText(newRegion.ParentRegion.Name).Selected = true;

        ((IFeedback)this.Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.LongName,
                    newRegion.GetType().Name,
                    Feedback.Actions.Created,
                    newRegion.Name
                );
    }

    protected void AddSiteClick(object sender, EventArgs e)
    {
        string regionId = popAddSite.ReferrerId;
        string siteName = popAddSite.Value;

        // If this site name already exists, then error
        if (SiteManager.GetSiteByName(siteName) != null)
        {
            ((IFeedback)Page.Master).ShowFeedback(Feedback.Actions.Error, "Site " + siteName + " already exists");
        }
        else
        {
            Region region = SiteManager.GetRegionById(regionId);
            Site site = new Site();
            site.Name = siteName;
            site.Region = region;
            site.SiteType = SiteManager.GetAllSiteTypes().First();
            SiteManager.CreateSite(site);

            this.PopulateTreeView<Region>(SiteManager.GetAllRegions(), true, false, string.Empty);

            RadTreeView1.DataBind();
            RadTreeView1.FindNodeByText(region.Name).ExpandParentNodes();
            RadTreeView1.FindNodeByText(region.Name).Selected = true;

            ((IFeedback)this.Page.Master).ShowFeedback(
                       BusiBlocksConstants.Blocks.Administration.LongName,
                       site.GetType().Name,
                       Feedback.Actions.Created,
                       site.Name
                   );
        }
    }

    protected void DeleteRegionClick(object sender, EventArgs e)
    {
        string regionID = popDeleteRegion.ReferrerId;
        Region region = SiteManager.GetRegionById(regionID);
        Region parentRegion = region.ParentRegion;
        string regionName = region.Name;
        string regionTypeName = region.GetType().Name;
        IList<Site> sites = SiteManager.GetAllSitesByRegion(region, true);

        if (sites.Count == 0)
        {
            SiteManager.DeleteRegion(region);
            ((IFeedback)this.Page.Master).ShowFeedback(
                    BusiBlocksConstants.Blocks.Administration.LongName,
                    regionTypeName,
                    Feedback.Actions.Deleted,
                    regionName
                );

            PopulateTreeView<Region>(SiteManager.GetAllRegions(), true, false, string.Empty);
            RadTreeView1.DataBind();

            RadTreeView1.FindNodeByText(parentRegion.Name).Selected = true;
        }
        else
        {

            ((IFeedback)this.Page.Master).ShowFeedback(
                   BusiBlocksConstants.Blocks.Administration.LongName,
                   region.GetType().Name,
                   Feedback.Actions.Error,
                   ErrorRegionNotEmpty
               );
        }
    }

    protected void DeleteCategoryClick(object sender, EventArgs e)
    {
        string categoryName = string.Empty;
        string categoryId = popDeleteCategory.ReferrerId;
        string categoryTypeName = string.Empty;
        string parentCategoryName = string.Empty;
        bool deleteFailure = false;

        // Need to figure out if this is a comms block category or a doco block category.
        if (!string.IsNullOrEmpty(categoryId))
        {
            BusiBlocks.CommsBlock.News.Category news = null;
            try
            {
                news = NewsManager.GetCategory(categoryId);
            }
            catch (NewsCategoryNotFoundException) { }
            if (news != null)
            {
                categoryTypeName = news.GetType().Name;
                categoryName = news.Name;

                // Don't allow the root category to be deleted.
                if (news.ParentCategory == null)
                {
                    ((IFeedback)this.Page.Master).ShowFeedback(
                        BusiBlocksConstants.Blocks.Administration.LongName,
                        news.Name,
                        Feedback.Actions.Error,
                        "Cannot delete the highest level category"
                    );
                    return;
                }

                parentCategoryName = news.ParentCategory.Name;
                IList<BusiBlocks.CommsBlock.News.Item> newsItems = NewsManager.GetItems(news, true);
                IList<BusiBlocks.CommsBlock.News.Category> newsSubCategories = NewsManager.GetCategories(news.Id, true);
                // NewsManager.GetCategories returns the root category, so it will always have at least one item
                if (newsSubCategories.Count <= 1 && newsItems.Count == 0)
                {
                    NewsManager.DeleteCategory(news);
                    PopulateTreeView<BusiBlocks.CommsBlock.News.Category>(NewsManager.GetViewableCategories(Page.User.Identity.Name), true, false, string.Empty);
                }
                else
                {
                    deleteFailure = true;
                }
            }
            else
            {
                BusiBlocks.DocoBlock.Category doco = null;
                try
                {
                    doco = DocoManager.GetCategory(categoryId);
                }
                catch (DocoCategoryNotFoundException) { }

                if (doco != null)
                {

                    categoryTypeName = doco.GetType().Name;
                    categoryName = doco.DisplayName;
                    parentCategoryName = doco.ParentCategory.DisplayName;

                    IList<Article> docoItems = DocoManager.GetArticles(doco, ArticleStatus.All, true);
                    IList<BusiBlocks.DocoBlock.Category> docoSubCategories = DocoManager.GetAllCategoriesBelow(doco.Id);
                    if (docoSubCategories.Count == 0 && docoItems.Count == 0)
                    {
                        DocoManager.DeleteCategory(doco);
                        this.PopulateTreeView<BusiBlocks.DocoBlock.Category>(DocoManager.GetAllCategories(), true, false, string.Empty);
                    }
                    else
                    {
                        deleteFailure = true;
                    }
                }
            }
            RadTreeView1.DataBind();
        }
        //Displaying feedback.
        if (deleteFailure)
        {
            ((IFeedback)this.Page.Master).ShowFeedback(
                 BusiBlocksConstants.Blocks.Administration.LongName,
                 categoryTypeName,
                 Feedback.Actions.Error,
                 ErrorCategoryNotEmpty
             );
            RadTreeView1.FindNodeByText(categoryName).ExpandParentNodes();
            RadTreeView1.FindNodeByText(categoryName).Selected = true;
        }
        else
        {
            ((IFeedback)this.Page.Master).ShowFeedback(
                            BusiBlocksConstants.Blocks.Administration.LongName,
                            categoryTypeName,
                            Feedback.Actions.Deleted,
                            categoryName
                        );
            RadTreeView1.FindNodeByText(parentCategoryName).ExpandParentNodes();
            RadTreeView1.FindNodeByText(parentCategoryName).Selected = true;
        }
    }

    public IList<RadTreeNode> Nodes
    {
        get
        {
            return RadTreeView1.GetAllNodes();
        }
    }

    public string Selected
    {
        set
        {
            RadTreeView1.FindNodeByValue(value).Selected = true;
            RadTreeView1.FindNodeByValue(value).ExpandParentNodes();
        }

        get
        {
            if(RadTreeView1 != null)
                if(RadTreeView1.SelectedNode != null)
                    if(!string.IsNullOrEmpty(RadTreeView1.SelectedNode.Value))
                        return RadTreeView1.SelectedNode.Value;
            
            return string.Empty;
        }
    }

    public void EnableDragAndDrop()
    {
        RadTreeView1.EnableDragAndDrop = true;
        RadTreeView1.EnableDragAndDropBetweenNodes = false;
    }

    protected string GetRegionNameAvailableUrl()
    {
        return Navigation.Admin_ManageLocations().GetAbsoluteClientUrl(true) + "/RegionNameAvailable";
    }

    public void RebindNode(string id, bool showContextMenu)
    {
        RadTreeNode node = RadTreeView1.FindNodeByValue(id);
        Panel pnl = (Panel)node.FindControl("pnlNode");

        node.Selected = true;
        Label plbl = (Label)pnl.FindControl("lblContextMenu");

        if (showContextMenu)
            plbl.CssClass = id + "_tvContext";
        else
            plbl.CssClass = id + "_tvContext hideElement";

        RadTreeView1.DataBind();
        node.ExpandParentNodes();
    }

}