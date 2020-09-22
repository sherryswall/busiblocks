using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Doco_Search : System.Web.UI.Page
{
    private const int LIST_PAGING_SIZE = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        searchResult.LinkNextClick += new EventHandler(searchResult_LinkNextClick);
        searchResult.LinkPreviousClick += new EventHandler(searchResult_LinkPreviousClick);
        searchResult.SearchEntityUrlCallback += new Controls_SearchResult.SearchEntityUrlDelegate(searchResult_SearchEntityUrlCallback);

        if (!IsPostBack)
        {
            LoadCategories();
        }
    }

    private void LoadCategories()
    {
        IList<BusiBlocks.DocoBlock.Category> categories = BusiBlocks.DocoBlock.DocoManager.GetAllCategories();
        listForum.Items.Clear();

        foreach (BusiBlocks.DocoBlock.Category category in categories)
        {
            if (BusiBlocks.SecurityHelper.CanUserView(User.Identity.Name, category.Id))
            {
                ListItem listItem = new ListItem(category.DisplayName);
                listItem.Selected = true;
                listItem.Value = category.Id;


                listForum.Items.Add(listItem);
            }
        }
    }

    private string[] GetSelectedCategories()
    {
        List<string> categories = new List<string>();

        foreach (ListItem item in listForum.Items)
        {
            if (item.Selected)
            {
                BusiBlocks.DocoBlock.Category category = BusiBlocks.DocoBlock.DocoManager.GetCategory(item.Value);
                if (BusiBlocks.SecurityHelper.CanUserView(User.Identity.Name, category.Id))
                {
                    categories.Add(category.DisplayName);
                }
            }
        }

        return categories.ToArray();
    }

    void searchResult_LinkPreviousClick(object sender, EventArgs e)
    {
        LoadList(searchResult.CurrentPage - 1);
    }

    void searchResult_LinkNextClick(object sender, EventArgs e)
    {
        LoadList(searchResult.CurrentPage + 1);
    }

    Navigation.NavigationPage searchResult_SearchEntityUrlCallback(BusiBlocks.ISearchResult entity)
    {
        BusiBlocks.DocoBlock.Article article = (BusiBlocks.DocoBlock.Article)entity;

        return Navigation.Doco_ViewArticle(article.Name, 0);
    }


    protected void btSearch_Click(object sender, EventArgs e)
    {
        LoadList(0);
    }

    private void LoadList(int page)
    {
        try
        {
            string[] searchFor = BusiBlocks.SplitHelper.SplitSearchText(txtSearchFor.Text);
            string[] authorSearch = BusiBlocks.SplitHelper.SplitSearchText(txtAuthor.Text);

            BusiBlocks.PagingInfo paging = new BusiBlocks.PagingInfo(LIST_PAGING_SIZE, page);
            IList<BusiBlocks.DocoBlock.Article> articles = BusiBlocks.DocoBlock.DocoManager.FindArticles(
                                                            BusiBlocks.Filter.MatchOne(GetSelectedCategories()),
                                                            BusiBlocks.Filter.ContainsAll(searchFor),
                                                            BusiBlocks.Filter.ContainsOne(authorSearch), 
                                                            null, null, null, null, 
                                                            BusiBlocks.DocoBlock.ArticleStatus.EnabledAndApproved, 
                                                            paging);

            searchResult.LoadList(articles, page, (int)paging.PagesCount);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }
}
