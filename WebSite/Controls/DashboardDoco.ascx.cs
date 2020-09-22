// <copyright file="DashboardDoco.ascx.cs" company="BusiBlocks">
//     BusiBlocks. All rights reserved.
// </copyright>
// <author>BusiBlocks employee</author>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using BusiBlocks.Audit;
using BusiBlocks.DocoBlock;
using Resources;
using WebFiler;
using BusiBlocks.Versioning;
using BusiBlocks;

/// <summary>
/// A user control to display articles.
/// </summary>
public partial class Controls_DashboardDoco : System.Web.UI.UserControl
{
    /// <summary>
    /// Displays a summary of notifications from the document block.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">Event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.LoadArticles();

        sectionError.Visible = false;
        listRepeater.Visible = true;
    }

    /// <summary>
    /// Retrieves a URL friendly string that points to an article.
    /// </summary>
    /// <param name="name">The name of the article</param>
    /// <returns>A user friendly Url for the article</returns>
    protected string GetViewArticleUrl(string name)
    {
        // If it is an uploaded document, then view the document and set the acknowledged flag
        // to true (via audit).
        Article article = DocoManager.GetArticleByName(name, false);
        if (article.IsUpload)
        {
            if (!article.Acknowledged)
            {
                // todo Popup to say that this will be acknowledged.
                AuditManager.Audit(Page.User.Identity.Name, article.Id + article.Version.ToString(CultureInfo.InvariantCulture), AuditRecord.AuditAction.Acknowledged);
            }

            string root = Server.MapPath(System.IO.Path.Combine("../Doco/", System.Web.Configuration.WebConfigurationManager.AppSettings.Get(Resource.Root)));
            string path =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.NewObjectPath,
                        root,
                        article.Category.Id);

            if (!string.IsNullOrEmpty(article.FileName))
            {
                string url = string.Format(
                                CultureInfo.InvariantCulture,
                                Resource.DocoFilesLoc + Resource.FileOpen,
                                UrlEncoding.Encode(System.IO.Path.Combine(path, article.FileName)));
                return url;
            }
            else
            {
                return string.Empty;
            }
        }
        else
        {
            return Navigation.Doco_ViewArticle(name, 0).GetClientUrl(this, true) + "&cMode=pub&id=" + article.Category.Id;
        }
    }

    /// <summary>
    /// Modifies the ImageUrl based on certain properties.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">Event arguments</param>
    protected void ListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string imageUrl = string.Empty;

        var item = e.Item.DataItem as Article;

        if (item.RequiresAck)
        {
            imageUrl = item.Acknowledged ? @"~\app_themes\default\icons\cube_green.png" : @"~\app_themes\default\icons\cube_red.png";
        }
        else
        {
            imageUrl = item.Viewed ? @"~\app_themes\default\icons\cube_green.png" : @"~\app_themes\default\icons\cube_yellow.png";
        }

        (e.Item.FindControl("imgItem") as Image).ImageUrl = imageUrl;
    }

    /// <summary>
    /// Retrieve all the articles.
    /// </summary>
    private void LoadArticles()
    {
        var articles = new List<Article>();

        // Get eligible document categories
        // todo Refactor Versioning so that I can pass the categories that I'm interested in (viewable news categories), 
        // and then it returns be all the versionItems that are in those categories.
        IList<Access> accessibleList = AccessManager.GetUsersAccessibleItems(Page.User.Identity.Name, BusiBlocks.ItemType.DocoCategory, BusiBlocks.AccessType.View);
        
        foreach (Access accessItem in accessibleList)
        {
            Category category = DocoManager.GetCategory(accessItem.ItemId);
            articles.AddRange(DocoManager.GetArticles(category, ArticleStatus.EnabledAndApproved, false)
                .Where(x => x.RequiresAck)
                .Distinct(new KeyEqualityComparer<Article>(x => x.Name)));
        }

        // Filter articles to only include those that have not been viewed or ack'd
        var itemsToList = new List<Article>();

        foreach (Article article in articles)
        {
            if (!itemsToList.Exists(i => i.Id == article.Id))
            {
                if (article.RequiresAck)
                {
                    if (article.Acknowledged)
                    {
                        continue;
                    }
                }
                else
                {
                    if (article.Viewed)
                    {
                        continue;
                    }
                }
                //restricting to display only 5 items for each dashboard. 
                if (itemsToList.Count < 5)
                    itemsToList.Add(article);
            }
        }

        lblNoResults.Visible = itemsToList.Count == 0;
        listRepeater.DataSource = itemsToList;
        listRepeater.DataBind();
    }
}