using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public static class Navigation
{
    public struct NavigationPage
    {
        public string Location;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Must be a relative path like SubPath/File.aspx, without ~ character</param>
        public NavigationPage(string location)
        {
            Location = location;
        }

        /// <summary>
        /// Returns an Url to this page that can be used on server side (using ~ notation)
        /// </summary>
        /// <param name="htmlEncode">True to encode the url so can be used without problems inside an xhtml article</param>
        /// <returns></returns>
        public string GetServerUrl(bool htmlEncode)
        {
            string url = BusiBlocks.PathHelper.CombineUrl("~", Location);
            if (htmlEncode)
                url = HttpUtility.HtmlEncode(url);
            return url;
        }

        /// <summary>
        /// Returns an absolute client url
        /// </summary>
        /// <param name="htmlEncode">True to encode the url so can be used without problems inside an xhtml article</param>
        /// <returns></returns>
        public string GetAbsoluteClientUrl(bool htmlEncode)
        {
            string url = BusiBlocks.PathHelper.CombineUrl(BusiBlocks.PathHelper.GetWebAppUrl(), Location);
            if (htmlEncode)
                url = HttpUtility.HtmlEncode(url);
            return url;
        }

        /// <summary>
        /// Returns an url to this page that can be used on client side
        /// </summary>
        /// <param name="source"></param>
        /// <param name="htmlEncode">True to encode the url so can be used without problems inside an xhtml article</param>
        /// <returns></returns>
        public string GetClientUrl(Control source, bool htmlEncode)
        {
            string url = source.ResolveClientUrl(GetServerUrl(false));
            if (htmlEncode)
                url = HttpUtility.HtmlEncode(url);
            return url;
        }

        /// <summary>
        /// Redirect the response to this page.
        /// </summary>
        /// <param name="source"></param>
        public void Redirect(Control source)
        {
            source.Page.Response.Redirect(GetServerUrl(false));
        }

        /// <summary>
        /// User Server.Transfer.
        /// </summary>
        public void Transfer()
        {
            System.Web.HttpContext.Current.Server.Transfer(GetServerUrl(false));
        }
    }

    /// <summary>
    /// Build a url based on the page and the parameters specified
    /// </summary>
    /// <param name="page">The page, like page.aspx</param>
    /// <param name="paramTemplate">Must be a format string like forum={0}&id={1}</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static string BuildUrl(string page, string paramTemplate, params string[] parameters)
    {
        string[] encodedParams;
        if (parameters != null && parameters.Length > 0)
        {
            encodedParams = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                encodedParams[i] = HttpUtility.UrlEncode(parameters[i]);
        }
        else
        {
            encodedParams = new string[0];
        }

        return page + "?" + string.Format(paramTemplate, encodedParams);
    }

    public static NavigationPage Error()
    {
        return new NavigationPage("/Error.aspx");
    }   

    public static NavigationPage Admin_FormList()
    {
        return new NavigationPage("/Forms/FormDefinitions.aspx");
    }

    public static NavigationPage Admin_ForumDetails(string id)
    {
        return new NavigationPage(BuildUrl("/Admin/ForumDetails.aspx", "id={0}", id));
    }

    public static NavigationPage Admin_ForumDetailsNew()
    {
        return new NavigationPage("/Admin/ForumDetails.aspx");
    }

    public static NavigationPage Admin_FormDetailsNew()
    {
        return new NavigationPage("/Forms/FormDefinitions.aspx");
    }

    public static NavigationPage Admin_DocoDetails(string id)
    {
        return new NavigationPage(BuildUrl("/Doco/DocoDetails.aspx", "id={0}", id));
    }

    public static NavigationPage Admin_DocoDetailsNew()
    {
        return new NavigationPage("/Doco/DocoDetails.aspx");
    }

    public static NavigationPage Admin_DocoDetailsNew(BusiBlocks.DocoBlock.Category parentCategory)
    {
        return new NavigationPage(BuildUrl("/Doco/DocoDetails.aspx", "parentCategory={0}", parentCategory.Id));
    }

    public static NavigationPage Admin_ManageComm()
    {
        return new NavigationPage("/Admin/ManageComm.aspx");
    }

    public static NavigationPage Admin_ManageSite(string id)
    {
        return new NavigationPage(BuildUrl("/Admin/ManageSites.aspx", "id={0}", id));
    }
    public static NavigationPage Admin_ManageSite(string id, string regionId)
    {
        return new NavigationPage(BuildUrl("/Admin/ManageSites.aspx", "id={0}&regionId={1}", id, regionId));
    }

    public static NavigationPage Admin_ManageUsers(string id)
    {
        //return new NavigationPage(BuildUrl("/Admin/ManageUsers.aspx", "id={0}", id));
        return new NavigationPage(BuildUrl("/Admin/EditUser.aspx", "id={0}", id));
    }

    public static NavigationPage Admin_ManageDoco()
    {
        return new NavigationPage("/Admin/ManageDoco.aspx");
    }

    public static NavigationPage Admin_ManageLocations()
    {
        return new NavigationPage("/Admin/ManageLocations.aspx");
    }

    public static NavigationPage Admin_SearchUsers()
    {
        return new NavigationPage("/Admin/SearchUsers.aspx");
    }

    public static NavigationPage Forum_ViewForum(string forumName)
    {
        return new NavigationPage(BuildUrl("/Communication/ViewForum.aspx", "forum={0}", forumName));
    }

    public static NavigationPage Forum_Search()
    {
        return new NavigationPage("/Communication/Search.aspx");
    }

    public static NavigationPage Doco_Default()
    {
        return new NavigationPage("/Doco/Default.aspx");
    }
    public static NavigationPage Doco_ViewCategory(string id)
    {
        return new NavigationPage(BuildUrl("/Doco/ViewCategory.aspx", "id={0}", id));
    }
    public static NavigationPage Doco_ViewUploadedDoc(string id)
    {
        return new NavigationPage(BuildUrl("/Doco/ViewUploadedDoc.aspx", "id={0}", id));
    }
    public static NavigationPage Doco_ViewVersionHistory(string groupId)
    {
        return new NavigationPage(BuildUrl("/Doco/VersionHistory.aspx", "vGID={0}", groupId));
    }

    /// <summary>
    /// Returns the rss page for the categories selected.
    /// </summary>
    /// <param name="categoryNames">Use null to returns all the available categories</param>
    /// <returns></returns>
    public static NavigationPage Doco_CategoryRss(params string[] categoryNames)
    {
        if (categoryNames == null || categoryNames.Length == 0)
            return new NavigationPage("/Doco/CategoryRSS.aspx");
        else
            return new NavigationPage(BuildUrl("/Doco/CategoryRSS.aspx", "name={0}", string.Join(",", categoryNames)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version">Use 0 for the latest version</param>
    /// <returns></returns>
    public static NavigationPage Doco_ViewArticle(string name, int version)
    {
        return new NavigationPage(BuildUrl("/Doco/ViewArticle2.aspx", "name={0}&version={1}", name, version.ToString()));
    }

    public static NavigationPage Doco_ViewArticle(string name, int version, string categoryId, string cMode)
    {
        return new NavigationPage(BuildUrl("/Doco/ViewArticle2.aspx", "name={0}&version={1}&cMode={3}&id={2}", name, version.ToString(), categoryId, cMode));
    }

    public static NavigationPage Doco_ViewArticleVersions(string name)
    {
        return new NavigationPage(BuildUrl("/Doco/ViewArticleVersions.aspx", "name={0}", name));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version">Use 0 for the latest version</param>
    /// <returns></returns>
    public static NavigationPage Doco_PrintArticle(string name, int version)
    {
        return new NavigationPage(BuildUrl("/Doco/PrintArticle.aspx", "name={0}&version={1}", name, version.ToString()));
    }

    public static NavigationPage Doco_EditArticle(string name)
    {
        return new NavigationPage(BuildUrl("/Doco/EditArticle.aspx", "name={0}", name));
    }

    public static NavigationPage Doco_NewArticle(string categoryId)
    {
        return new NavigationPage(BuildUrl("/Doco/NewArticle.aspx", "id={0}", categoryId));
    }

    public static NavigationPage Doco_Attach(string articleName, string attachName, bool download)
    {
        string mode;
        if (download)
            mode = "download";
        else
            mode = "show";

        return new NavigationPage(BuildUrl("/Doco/Attach.ashx", "article={0}&attach={1}&mode={2}", articleName, attachName, mode));
    }

    public static NavigationPage News_NewsDetailsNew()
    {
        return new NavigationPage("/Communication/NewsDetails.aspx");
    }

    public static NavigationPage News_ViewCategory(string categoryId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsViewCategory.aspx", "id={0}", categoryId));
    }

    public static NavigationPage Form_ViewForm(string formName)
    {
        return new NavigationPage(BuildUrl("/Forms/FormDetails.aspx", "form={0}", formName));
    }

    public static NavigationPage User_ChangePassword()
    {
        return new NavigationPage("/User/ChangePassword.aspx");
    }

    public static NavigationPage Communication_Default(int tabIndex)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t={0}", tabIndex.ToString()));
    }

    public static NavigationPage Communication_NewsCategory(string newsCategoryId)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "ncid={0}", newsCategoryId));
    }

    public static NavigationPage Communication_News()
    {
        return new NavigationPage("/Communication/Default.aspx");
    }

    public static NavigationPage Communication_NewsCategoryNewItem(BusiBlocks.CommsBlock.News.Category category)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsDetails.aspx", "category={0}", category.Id));
    }

    public static NavigationPage Communication_NewsNewItem(BusiBlocks.CommsBlock.News.Category category)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsEditItem.aspx", "category={0}", category.Id));
    }

    public static NavigationPage Communication_NewsCategoryViewItem(string newsItemId)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "niid={0}&action=view", newsItemId));
    }

    public static NavigationPage Communication_NewsViewItem(string newsItemId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsViewItem.aspx", "item={0}", newsItemId));
    }

    public static NavigationPage Communication_NewsCategoryEditItem(string newsCategoryId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsDetails.aspx", "id={0}", newsCategoryId));
    }

    public static NavigationPage Communication_NewsEditItem(string versionId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsEditItem.aspx", "item={0}", versionId));
    }

    public static NavigationPage Communication_NewsViewItemApproval(string versionId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsViewItem.aspx", "item={0}&mode=approve", versionId));
    }

    public static NavigationPage Communication_NewsEditItemApproval(string versionId)
    {
        return new NavigationPage(BuildUrl("/Communication/NewsEditItem.aspx", "item={0}&mode=approve", versionId));
    }

    public static NavigationPage Communication_NewsVersionHistory(string groupId)
    {
        return new NavigationPage(BuildUrl("/Communication/VersionHistory.aspx", "vGID={0}", groupId));
    }

    public static NavigationPage Communication_UserViewStatus(string newsItemId, string versionId)
    {
        return new NavigationPage(BuildUrl("/Communication/UserViewStatus.aspx", "itemId={0}&versionId={1}", newsItemId, versionId));
    }

    public static NavigationPage Communication_ForumNewTopic(string ForumName)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&forum={0}&action=newt", ForumName));
    }

    public static NavigationPage Communication_ForumViewTopic(string ForumName)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&forum={0}&action=viewt", ForumName));
    }

    public static NavigationPage Communication_ForumView(string ForumName)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&forum={0}&action=viewf", ForumName));
    }

    public static NavigationPage Communication_ForumNewMessage(string IdMessage)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&forum={0}&action=newfm", IdMessage));
    }

    public static NavigationPage Communication_ForumNew()
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&action=newf"));
    }

    public static NavigationPage Communication_ForumSearch()
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&action=searchf"));
    }

    public static NavigationPage Communication_ForumSearch(string ForumName)
    {
        return new NavigationPage(BuildUrl("/Communication/Default.aspx", "t=2&action=searchf&forum={0}", ForumName));
    }

    public static NavigationPage Communication_ForumAttach(string msgId, bool download)
    {
        string mode;
        if (download)
            mode = "download";
        else
            mode = "show";

        return new NavigationPage(BuildUrl("/Communication/Attach.ashx", "id={0}&mode={1}", msgId, mode));
    }



    public static NavigationPage Directory_Search()
    {
        return new NavigationPage("/Directory/DirectorySearch.aspx");
    }

    public static NavigationPage Directory_PersonDetails(string personId)
    {
        return new NavigationPage(BuildUrl("/Directory/PersonDetails.aspx", "id={0}", personId));
    }

    public static NavigationPage Directory_SiteDetails(string siteId)
    {
        return new NavigationPage(BuildUrl("/Directory/SiteDetails.aspx", "id={0}", siteId));
    }

    public class NavigateNewItem
    {
        public string URL;
        public bool Access;

        public NavigateNewItem(string url, bool access)
        {
            URL = url;
            Access = access;
        }
    }

    public static NavigationPage Admin_ManageRolesGroups()
    {
        return new NavigationPage("/Admin/ManageRolesGroups.aspx");
    }


    public static NavigationPage Webservice_Communication_News()
    {
        return new NavigationPage("/Communication/NewsWS.asmx");
    }
    
}
