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
using System.Web.Services;
using Telerik.Web.UI;

public partial class Forum_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HtmlLink link = new HtmlLink();
        link.Attributes.Add("rel", "alternate");
        link.Attributes.Add("type", "application/rss+xml");
        link.Attributes.Add("title", "Communication");
        Header.Controls.Add(link);

        string NewsItemCategoryId = Request.QueryString["ncid"];
        string NewsItemId = Request.QueryString["niid"];
        string action = Request.QueryString["action"];

        string forumName = Request.QueryString["forum"];
        string forumTopicId = Request.QueryString["ftid"];

        string tabIndex = Request.QueryString["t"];

        NewsDefault.Visible = false;

        ctrlForum.Visible = false;
        ctrlForumDetails.Visible = false;
        ctrlForumNewTopic.Visible = false;
        ctrlForumSearch.Visible = false;
        ctrlForumViewForum.Visible = false;
        ctrlForumViewTopic.Visible = false;
        ctrlForumNewMessage.Visible = false;

        if (!string.IsNullOrEmpty(tabIndex)) tabCommunication.ActiveTabIndex = Int16.Parse(tabIndex);

        // Display Forum - or not.
        if (action == "newf")
        {
            ctrlForumDetails.Visible = true;
        }
        else if (action == "newt")
        {
            if (!string.IsNullOrEmpty(forumName))
            {
                ctrlForumNewTopic.LoadForum(forumName);
                ctrlForumNewTopic.Visible = true;
            }
        }
        else if (action == "viewt")
        {
            if (!string.IsNullOrEmpty(forumName))
            {
                ctrlForumViewTopic.LoadTopic(forumName);
                ctrlForumViewTopic.Visible = true;
            }
        }
        else if (action == "viewf")
        {
            if (!string.IsNullOrEmpty(forumName))
            {
                ctrlForumViewForum.Visible = true;
                ctrlForumViewForum.BindForum(forumName);
            }
            else if (!string.IsNullOrEmpty(forumTopicId))
            {
                ctrlForumViewTopic.Visible = true;
                ctrlForumViewTopic.LoadTopic(forumName);
            }
        }
        else if (action == "newfm")
        {
            ctrlForumNewMessage.Visible = true;
            ctrlForumNewMessage.LoadMessage(forumName);
        }
        else if (action == "searchf")
        {
            ctrlForumSearch.LoadForums(forumName);
            ctrlForumSearch.Visible = true;
        }
        else
        {
            ctrlForum.Visible = true;
        }

        // Display Announcements.
        if (!string.IsNullOrEmpty(NewsItemCategoryId))
        {
            IList<BusiBlocks.CommsBlock.News.Category> categories = BusiBlocks.CommsBlock.News.NewsManager.GetCategories(NewsItemCategoryId, true);
            NewsDefault.Visible = true;
            NewsDefault.Bind(categories);
        }
        else
        {
            NewsDefault.Visible = true;
            if (!string.IsNullOrEmpty(NewsItemCategoryId))
            {
                IList<BusiBlocks.CommsBlock.News.Category> categories = BusiBlocks.CommsBlock.News.NewsManager.GetCategories(NewsItemCategoryId, true);
                NewsDefault.Bind(categories);
            }
            else
            {
                // Display all news items.
                IList<BusiBlocks.CommsBlock.News.Category> categories = BusiBlocks.CommsBlock.News.NewsManager.GetAllCategories();
                NewsDefault.Bind(categories);
            }
        }
    }

    [WebMethod]
    public static object GetNewsCategories()
    {
        return Communication_NewsDefault.GetNewsCategories();
    }

    [WebMethod]
    public static object GetUsernames()
    {
        return Communication_DefaultPrivateMessages.GetUsernames();
    }
}
