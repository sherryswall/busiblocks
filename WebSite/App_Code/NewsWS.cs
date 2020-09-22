using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;

[System.Web.Script.Services.ScriptService]
public class NewsWS : System.Web.Services.WebService
{
    [WebMethod]
    public object GetNewsItemsByCategoryId(string id)
    {
        Category selectedNews = NewsManager.GetCategory(id);
        IList<Category> news = NewsManager.GetCategories(selectedNews.Id, true);

        List<KeyValuePair<string, string>> listItems = new List<KeyValuePair<string, string>>();

        foreach (Category newsItem in news)
            listItems.Add(new KeyValuePair<string, string>(newsItem.Id, newsItem.Name));

        return listItems;
    }

    [WebMethod]
    public Navigation.NavigateNewItem wmCheckEditAccess()
    {
        bool access = false;

        string URL = string.Empty;
        string categoryId = HttpContext.Current.Request.QueryString["id"];
        string userName = HttpContext.Current.User.Identity.Name;

        if (!string.IsNullOrEmpty(categoryId))
        {
            Category category = NewsManager.GetCategory(categoryId);
            URL = Navigation.Communication_NewsNewItem(category).GetAbsoluteClientUrl(true);
            access = true;
        }

        return new Navigation.NavigateNewItem(URL, access);
    }

    [WebMethod]
    public object DeleteCategory(string id)
    {
        bool deleted;
        try
        {
            string username = HttpContext.Current.User.Identity.Name;
            Category newsCategory = NewsManager.GetCategory(id);
            NewsManager.DeleteCategory(newsCategory);
            deleted = true;
        }
        catch (Exception)
        {
            deleted = false;
        }

        return deleted;
    }

    [WebMethod]
    public object MoveCategory(string source, string destination)
    {
        bool moved;
        try
        {
            Category destNewsCategory = NewsManager.GetCategory(destination);
            Category sourceNewsCategory = NewsManager.GetCategory(source);
            sourceNewsCategory.ParentCategory = destNewsCategory;
            NewsManager.UpdateCategory(sourceNewsCategory);
            moved = true;
        }
        catch (Exception)
        {
            moved = false;
        }

        return moved;
    }
}
