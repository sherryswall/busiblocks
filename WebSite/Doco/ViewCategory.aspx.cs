using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using BusiBlocks.AccessLayer;
using BusiBlocks;
using BusiBlocks.DocoBlock;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using System.Web.Services;

public partial class Doco_ViewCategory : System.Web.UI.Page
{
    private string _CategoryId;
    public  virtual string CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; }
    }
    public static Navigation.NavigateNewItem newLink { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["RefUrl"] = Request.UrlReferrer.ToString();
        }
        //check previous page's URL for the id if Id hasn't been brought forward.
        if (Request.UrlReferrer.Query.Contains("?id"))
            CategoryId = Request.UrlReferrer.Query.Substring(Request.UrlReferrer.Query.IndexOf("id") + 3);
        else
            CategoryId = Request["id"]; //check for id in URL

        if (!string.IsNullOrEmpty(CategoryId))
        {
            Category category = null;
            if (!string.IsNullOrEmpty(CategoryId))
                category = DocoManager.GetCategory(CategoryId);

            if (!SecurityHelper.CanUserView(User.Identity.Name, category.Id))
            {
                // If the user cannot view the category, then return silently.
                object refUrl = ViewState["RefUrl"];
                if (refUrl != null)
                    Response.Redirect((string)refUrl);
            }

            lblDisplayName.InnerText = category.DisplayName;

            HtmlLink link = new HtmlLink();
            link.Href = Navigation.Doco_CategoryRss(CategoryId).GetServerUrl(true);
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("type", "application/rss+xml");
            link.Attributes.Add("title", "Category " + category.DisplayName + " Announcements");
            Header.Controls.Add(link);
            //commenting this till the Creation row is finalised.
            string URL = Navigation.Doco_NewArticle(CategoryId).GetAbsoluteClientUrl(true);
            bool Access = SecurityHelper.CanUserEdit(Page.User.Identity.Name, category.Id);

            newLink = new Navigation.NavigateNewItem(URL, Access);

            pmm.LoadPermissionsView(category.Id, category.DisplayName);
            LoadList(category);
        }
    }

    private void LoadList(Category category)
    {
        //Get the standard articles
        IList<Article> articles = DocoManager.GetArticles(category, ArticleStatus.All, false);

        // Add all the document instances to the list.
        string[] arr = articles.Select(x => x.Id).ToArray();
        list.SetState(string.Join(",", arr));
    }

    [WebMethod]
    public static string wmGetCategoryId()
    {
        return "";
    }
    [WebMethod]
    public static Navigation.NavigateNewItem wmCheckEditAccess()
    {
        return newLink;
    }
}
