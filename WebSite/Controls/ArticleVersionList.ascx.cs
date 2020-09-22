using System;
using System.Collections.Generic;

public partial class Controls_ArticleVersionList : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadList();
        }
    }

    #region Properties
    /// <summary>
    /// Gets or sets the article name used to load the list of versions
    /// </summary>
    public string ArticleName
    {
        get
        {
            object val = ViewState["ArticleName"];
            return val == null ? null : (string)val;
        }
        set { ViewState["ArticleName"] = value; }
    }
    #endregion

    private void LoadList()
    {
        BusiBlocks.DocoBlock.Article article = BusiBlocks.DocoBlock.DocoManager.GetArticleByName(ArticleName, true);

        lblTitle.InnerText = article.Title;

        //if (BusiBlocks.SecurityHelper.CanRead(Page.User, article.Category, article) == false)
        //    throw new BusiBlocks.InvalidPermissionException("read article");

        IList<BusiBlocks.DocoBlock.ArticleBase> versions = BusiBlocks.DocoBlock.DocoManager.GetArticleVersions(article);

        listRepeater.DataSource = versions;
        listRepeater.DataBind();
    }

    protected string GetViewUrl(BusiBlocks.DocoBlock.ArticleBase article)
    {
        return Navigation.Doco_ViewArticle(ArticleName, article.Version).GetClientUrl(Page, true);
    }
}
