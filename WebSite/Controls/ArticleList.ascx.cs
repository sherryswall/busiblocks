using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using BusiBlocks;
using BusiBlocks.DocoBlock;
using Telerik.Web.UI;
using WebFiler;
using Resource = Resources.Resource;
using System.Text;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.PersonLayer;

public partial class Controls_ArticleList : UserControl
{
    private const string ArticleList = "articleList";
    public static string FilterExpression { get; set; }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        Control feedback = Utilities.FindControlRecursive(Page.Master, "feedback");
        RadAjaxManager1.AjaxSettings.AddAjaxSetting(RadGrid1, feedback);
        if (!Page.IsPostBack)
            FilterExpression = string.Empty;
    }

    protected string GetViewArticleUrl(string id, string cMode)
    {
        Article article = DocoManager.GetArticle(id);

        if (article.IsUpload)
        {
            if (!string.IsNullOrEmpty(article.FileName))
                return Navigation.Doco_ViewUploadedDoc(article.Id).GetAbsoluteClientUrl(true);
            else
                return string.Empty;
        }
        else
        {
            return Navigation.Doco_ViewArticle(article.Name, 0).GetServerUrl(false) + "&cMode=" + cMode + "&id=" +
                   article.Category.Id;
        }
    }

    protected string GetEditArticleUrl(string name, string id)
    {
        return Navigation.Doco_EditArticle(name).GetServerUrl(false) + "&id=" + id;
    }

    protected string GetArticleStatus(bool enabled, bool approved)
    {
        if (enabled == false)
            return "(Disabled)";
        else if (approved == false)
            return "(Not approved)";
        else
            return string.Empty;
    }

    public void SetState(string stateVal)
    {
        ViewState[ArticleList] = stateVal;
    }

    public string GetState()
    {
        return ViewState[ArticleList].ToString();
    }

    public void Bind()
    {
        IList<BusiBlocks.DocoBlock.Category> categories = DocoManager.GetViewableCategories(Page.User.Identity.Name);
        List<Article> allArticles = new List<Article>();

        foreach (BusiBlocks.DocoBlock.Category category in categories)
        {
            List<Article> articles = (List<Article>)DocoManager.GetArticles(category, ArticleStatus.All, false);
            articles.ToList().ForEach(x => allArticles.Add(x));
        }
        RadGrid1.DataSource = allArticles;
    }

    protected void listRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Header && e.Item.ItemType != ListItemType.Footer)
        {
            var item = e.Item.DataItem as Article;

            (e.Item.FindControl("imgType") as Image).ImageUrl =
                Utility.GetImageUrlType(item.FileName.Substring(item.FileName.IndexOf('.') + 1), item.IsUpload);
            (e.Item.FindControl("imgBtnEdit") as LinkButton).PostBackUrl = GetEditArticleUrl(item.Name, item.Category.Id);

            if (!SecurityHelper.CanUserEdit(Page.User.Identity.Name, item.Category.Id))
            {
                // Disable the Edit and Delete functionality.
                var editButton = e.Item.FindControl("imgBtnEdit") as LinkButton;
                var deleteButton = e.Item.FindControl("imgBtnDel") as LinkButton;
                if (editButton != null) editButton.Visible = false;
                if (deleteButton != null) deleteButton.Visible = false;
            }
        }
    }

    protected void RadGrid1_Init(object sender, EventArgs e)
    {
        GridFilterMenu menu = RadGrid1.FilterMenu;
        // Iterate through the items backwards 
        // so that the indexing is not thrown off 
        // when items are removed! 
        for (int i = menu.Items.Count - 1; i >= 0; i--)
        {
            if (menu.Items[i].Text == "Between")
            {
                menu.Items.RemoveAt(i);
            }
            else if (menu.Items[i].Text == "Custom")
                menu.Items[i].Text = "Between";
        }
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "UpdateDate";
            expression.SortOrder = GridSortOrder.Descending;
            RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
        }

        List<Article> articles = new List<Article>();

        if (!string.IsNullOrEmpty(FilterExpression))
            BindWithFilter(articles);
        else
        {
            SearchDefault(articles);
        }
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            var item = e.Item.DataItem as Article;

            var gridItem = (GridDataItem)e.Item;
            //Category
            var lnkCategory = (HyperLink)gridItem.FindControl("lnkCategory");
            lnkCategory.NavigateUrl = Navigation.Doco_ViewCategory(item.Category.Id).GetServerUrl(true);

            //Ack images.
            var imgAck = (Image)gridItem["View"].FindControl("imgAck");
            imgAck.ImageUrl = Utility.GetTrafficLight(item);

            //Type images.
            var imgType = (Image)gridItem["Type"].FindControl("imgType");
            imgType.ImageUrl = Utility.GetImageUrlType(item.FileName.Substring(item.FileName.IndexOf('.') + 1), item.IsUpload);

            //Actions
            var editButton = (LinkButton)gridItem["Actions"].FindControl("imgBtnEdit");
            var deleteButton = (LinkButton)gridItem["Actions"].FindControl("imgBtnDel");

            DateTime modifiedDate = DateTime.Parse(gridItem["Date"].Text);
            gridItem["Date"].Text = modifiedDate.ToString("dd/MM/yy - HH:mm");

            gridItem["Author"].Text = Utilities.GetDisplayUserName(item.Author);

            editButton.CommandArgument = item.Name;
            editButton.Attributes.Add("catid", item.Category.Id);

            bool hideActionButtons = false;

            if (!SecurityHelper.CanUserEdit(Page.User.Identity.Name, item.Category.Id))
            {
                bool canContrib = SecurityHelper.CanUserContribute(Page.User.Identity.Name, item.Category.Id);
                if (!canContrib)
                {
                    hideActionButtons = true;
                }
                else
                {
                    if (item.Owner.Equals(Utilities.GetUserName(Page.User.Identity.Name)) == false)
                    {
                        hideActionButtons = true;
                    }
                }
            }

            if (hideActionButtons)
            {
                editButton.Visible = false;
                deleteButton.Visible = false;
            }

        }
    }

    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName.Equals("Edt"))
        {
            //Navigation.Doco_EditArticle(e.CommandArgument.ToString()).Redirect(this);
            var lnk = (LinkButton)e.Item.FindControl("imgBtnEdit");

            var img = (Image)e.Item.FindControl("imgType");

            if (img.ImageUrl.Contains("onlineDoc"))
                Navigation.Doco_ViewArticle(e.CommandArgument.ToString(), 0, lnk.Attributes["catid"], "draft").Redirect(
                    this);
            else
                Navigation.Doco_EditArticle(e.CommandArgument.ToString()).Redirect(this);
        }
        else if (e.CommandName.Equals("Delete"))
        {
            string docId = e.CommandArgument.ToString();

            Article articleToDelete = DocoManager.GetArticle(docId);
            string itemNameBeforeDelete = articleToDelete.Title;
            string catIdBeforeDel = articleToDelete.Category.Id;
            DocoManager.DeleteArticle(articleToDelete);

            Controls_TreeView tree = (Controls_TreeView)this.Parent.FindControl("tree1");
            tree.PopulateTreeView<BusiBlocks.DocoBlock.Category>(DocoManager.GetViewableCategories(Page.User.Identity.Name), false, false, string.Empty);
            tree.RebindNode(catIdBeforeDel, false);

            ((IFeedback)Page.Master).ShowFeedback(BusiBlocksConstants.Blocks.Documents.ShortName, articleToDelete.GetType().Name, Feedback.Actions.Deleted, itemNameBeforeDelete);
        }
        else if (e.CommandName == RadGrid.FilterCommandName)
        {
            Pair filterPair = (Pair)e.CommandArgument;
            List<Article> items = new List<Article>();

            switch (filterPair.Second.ToString())
            {
                case "Category":
                    TextBox tbPattern = (e.Item as GridFilteringItem)["Category"].Controls[0] as TextBox;
                    FilterExpression = tbPattern.Text;
                    BindWithFilter(items);
                    RadGrid1.DataBind();
                    if (!string.IsNullOrEmpty(FilterExpression))
                    {
                        var foundArticles = DocoManager.GetCategoryByLikeName(FilterExpression.Split(',').First(), true);
                        if (foundArticles.Count > 0)
                        {
                            Controls_TreeView tree = (Controls_TreeView)this.Parent.FindControl("tree1");
                            tree.PopulateTreeView<BusiBlocks.DocoBlock.Category>(DocoManager.GetViewableCategories(Page.User.Identity.Name), false, false, string.Empty);
                            string selectedNode = (foundArticles.FirstOrDefault()).Id;
                            tree.RebindNode(selectedNode, true);
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }

    protected void RadGrid1_PreRender(object sender, EventArgs e)
    {
        bool permission = false;

        var radGrid = (GridTableView)sender;
        radGrid.DataSource = null;
        radGrid.Rebind();
        foreach (GridDataItem dataRow in radGrid.Items)
        {
            dataRow.Display = !bool.Parse(dataRow["Deleted"].Text);

            string categoryId = dataRow["CategoryId"].Text;

            if (SecurityHelper.CheckWriteAccess(Page.User.Identity.Name, categoryId))
                permission = true;
            else
                dataRow["Actions"].Text = "";
        }

        if (!permission)
        {
            RadGrid1.MasterTableView.GetColumnSafe("Show").Visible = false;
            hidAminState.Value = "-1";
        }
        else
        {
            RadGrid1.MasterTableView.GetColumn("Show").Visible = true;
            hidAminState.Value = "0";
        }
    }

    protected void AddArticleToList(IList<Article> articles, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            BusiBlocks.DocoBlock.Category category = DocoManager.GetCategoryByName(name, true);

            if (category != null)
            {
                AddArticles(category, articles);
            }
            else// wild card scenario.
            {
                IList<BusiBlocks.DocoBlock.Category> categories = DocoManager.GetCategoryByLikeName(name, true);

                foreach (BusiBlocks.DocoBlock.Category item in categories)
                {
                    AddArticles(item, articles);
                }
            }
        }
    }

    private void AddArticles(BusiBlocks.DocoBlock.Category category, IList<Article> articles)
    {
        IList<Article> artls = DocoManager.GetArticles(category, ArticleStatus.All, false);
        foreach (Article article in artls)
        {
            articles.Add(article);
        }
    }

    private void BindWithFilter(List<Article> items)
    {
        if (!String.IsNullOrEmpty(FilterExpression))
        {
            if (FilterExpression.Contains(","))
            {
                string[] values = FilterExpression.Split(',');

                foreach (string cat in values)
                {
                    AddArticleToList(items, cat.Trim());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    AddArticleToList(items, FilterExpression.Trim());
                }
            }
            RadGrid1.DataSource = GetAccessibleList(items);
        }
        else
            SearchDefault(items);
    }

    private void SearchDefault(List<Article> articles)
    {
        Bind();
        List<Article> allArticles = (List<Article>)RadGrid1.DataSource;
        if (allArticles != null)
        {
            foreach (Article article in allArticles)
            {
                articles.Add(article);
            }
            RadGrid1.DataSource = GetAccessibleList(articles);
        }
        else
        {
            RadGrid1.DataSource = new List<Article>();
        }
    }

    protected List<Article> GetAccessibleList(List<Article> articles)
    {
        List<Article> accesibleArticlesList = new List<Article>();
        foreach (Article article in articles)
        {
            if (article != null)
            {
                if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, article.Category.Id))
                    accesibleArticlesList.Add(article);
                else
                {
                    if (SecurityHelper.CanUserContribute(Page.User.Identity.Name, article.Category.Id))
                    {
                        //only add if the user is the owner
                        if (article.Owner.Equals(Utilities.GetUserName(Page.User.Identity.Name)))
                            accesibleArticlesList.Add(article);
                        else if (SecurityHelper.CanUserView(Page.User.Identity.Name, article.Category.Id))
                        {
                            //if (article.ApprovalStatus.Name.Equals("Published")) --NEEDS TO BE UN COMMENTED WHEN APPROVAL STATUS IS DONE FOR DOCO BLOCK
                            accesibleArticlesList.Add(article);
                        }
                    }
                    else if (SecurityHelper.CanUserView(Page.User.Identity.Name, article.Category.Id))
                    {
                        //if (article.ApprovalStatus.Name.Equals("Published"))
                        accesibleArticlesList.Add(article);
                    }
                }
            }
        }
        return accesibleArticlesList;
    }

    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
    }

    protected void RadGrid1_PageIndexChanged(object sender, GridPageChangedEventArgs e)
    {
        RadGrid1.DataBind();
    }

    public void repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var bi = (BindingItem)e.Item.DataItem;
        (e.Item.FindControl("lblUser") as Label).Text = bi.Username;
    }

    protected void listRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "delete")
        {
            string docId = e.CommandArgument.ToString();
            DeleteDocument(docId);
        }
    }

    protected string DeleteDocument(string docId)
    {
        try
        {
            Article article = DocoManager.GetArticle(docId);

            if (SecurityHelper.CanUserEdit(Page.User.Identity.Name, article.Category.Id))
            {
                return DocoManager.DeleteArticle(article);
            }
            else
                throw new InvalidPermissionException("delete article");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #region Nested type: BindingItem

    public struct BindingItem
    {
        public string Username { get; set; }
        public bool ViewedOrAcked { get; set; }
        public bool RequiresAck { get; set; }
        public bool IsUpload { get; set; }
    }

    #endregion
}