using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using BusiBlocks.DocoBlock;
using BusiBlocks;
using System.Web.Services;

public partial class Doco_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var link = new HtmlLink();
        link.Href = Navigation.Doco_CategoryRss(null).GetServerUrl(true);
        link.Attributes.Add("rel", "alternate");
        link.Attributes.Add("type", "application/rss+xml");
        link.Attributes.Add("title", "Doco news");
        Header.Controls.Add(link);

        tree1.PopulateTreeView<Category>(DocoManager.GetViewableCategories(User.Identity.Name), false, false, string.Empty);
    }

    private IList<Article> GetAllArticles()
    {
        IList<Category> categories = DocoManager.GetAllCategories();
        IList<Article> articles = new List<Article>();
        foreach (Category category in categories)
        {
            if (SecurityHelper.CanUserView(Page.User.Identity.Name, category.Id))
            {
                IList<Article> arts = DocoManager.GetArticles(category, ArticleStatus.All, false);
                foreach (Article article in arts)
                {
                    articles.Add(article);
                }
            }
        }
        return articles;
    }

    [WebMethod]
    public static object GetCategories()
    {
        string userName = HttpContext.Current.Request.QueryString["userid"];
        return DocoManager.GetDocoTree(userName);
    }
}
