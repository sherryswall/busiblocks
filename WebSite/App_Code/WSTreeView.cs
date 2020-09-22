using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BusiBlocks.SiteLayer;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.DocoBlock;
using System.Web.Script.Serialization;
using BusiBlocks.PersonLayer;
using BusiBlocks;

/// <summary>
/// Summary description for WSTreeView
/// </summary>
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WSTreeView : System.Web.Services.WebService
{
    public static string currentNode { get; set; }
    private const string ErrorCategoryNotEmpty = "There are items in this category";
    private const string ErrorRegionNotEmpty = "There are persons associated with this region/site";

    [WebMethod]
    public void EditRegion(string Id, string Name)
    {
        Region region = SiteManager.GetRegionById(Id);
        region.Name = Name;
        SiteManager.UpdateRegion(region);
    }
    [WebMethod(EnableSession = true)]
    public object DeleteRegion(string Id)
    {
        Region region = SiteManager.GetRegionById(Id);
        IList<Person> PR = PersonManager.GetAllPersonsInRegion(region, true);
        if (PR.Count == 0)
        {
            SiteManager.DeleteRegion(region);
            return true;
        }
        else
        {
            Feedback feedBack = new Feedback(BusiBlocksConstants.Blocks.Administration.BlockName, "Locations", Feedback.Actions.Error, ErrorRegionNotEmpty);
            Session["feedback"] = feedBack;
            return false;
        }
    }
    [WebMethod]
    public void EditCategory(string Id, string Name, string treeViewName)
    {
        switch (treeViewName)
        {
            case "News":
                BusiBlocks.CommsBlock.News.Category category = NewsManager.GetCategory(Id);
                category.Name = Name;
                NewsManager.UpdateCategory(category);
                break;
            case "Doco":
                BusiBlocks.DocoBlock.Category docoCategory = DocoManager.GetCategory(Id);
                docoCategory.DisplayName = Name;
                DocoManager.UpdateCategory(docoCategory);
                break;
            default:
                break;
        }
    }
    [WebMethod]
    public void DeleteCategory(string Id, string treeViewName)
    {
        switch (treeViewName)
        {
            case "News":
                BusiBlocks.CommsBlock.News.Category newsCategory = NewsManager.GetCategory(Id);
                NewsManager.DeleteCategory(newsCategory);
                break;
            case "Doco":
                BusiBlocks.DocoBlock.Category docoCategory = DocoManager.GetCategory(Id);
                DocoManager.DeleteCategory(docoCategory);
                break;
            default:
                break;
        }
    }
    [WebMethod]
    public void DeleteCategory(string Id)
    {
        // Need to figure out if this is a comms block category or a doco block category.
        if (!string.IsNullOrEmpty(Id))
        {
            BusiBlocks.CommsBlock.News.Category news = null;

            try
            {
                news = NewsManager.GetCategory(Id);
            }
            catch (NewsCategoryNotFoundException) { }
            if (news != null)
            {
                NewsManager.DeleteCategory(news);
            }
            else
            {
                BusiBlocks.DocoBlock.Category doco = null;

                try
                {
                    doco = DocoManager.GetCategory(Id);
                }
                catch (DocoCategoryNotFoundException) { }
                if (doco != null)
                {
                    DocoManager.DeleteCategory(doco);
                }
            }
        }
    }
    [WebMethod]
    public void MoveRegion(string source, string destination)
    {
        Region destRegion = SiteManager.GetRegionByName(destination);
        Region sourceRegion = SiteManager.GetRegionByName(source);

        sourceRegion.ParentRegion = destRegion;
        SiteManager.UpdateRegion(sourceRegion);
    }
    [WebMethod]
    public void MoveCategory(string source, string destination, string treeViewName)
    {
        switch (treeViewName)
        {
            case "News":
                BusiBlocks.CommsBlock.News.Category destNewsCategory = NewsManager.GetCategoryByName(destination, true);
                BusiBlocks.CommsBlock.News.Category sourceNewsCategory = NewsManager.GetCategoryByName(source, true);
                sourceNewsCategory.ParentCategory = destNewsCategory;
                NewsManager.UpdateCategory(sourceNewsCategory);
                break;
            case "Doco":
                BusiBlocks.DocoBlock.Category destDocoCategory = DocoManager.GetCategoryByName(destination, true);
                BusiBlocks.DocoBlock.Category sourceDocoCategory = DocoManager.GetCategoryByName(source, true);
                sourceDocoCategory.ParentCategory = destDocoCategory;
                DocoManager.UpdateCategory(sourceDocoCategory);
                break;
            default:
                break;
        }
    }
    [WebMethod]
    public object GetSelectedItemBelow(string nodeName, string treeViewType, string treeViewName)
    {
        List<KeyValuePair<string, string>> listItems = new List<KeyValuePair<string, string>>();

        switch (treeViewType)
        {
            case "Category":

                switch (treeViewName)
                {
                    case "Doco":
                        BusiBlocks.DocoBlock.Category cat = DocoManager.GetCategoryByName(nodeName, true);
                        listItems.Add(new KeyValuePair<string, string>(cat.Id, cat.DisplayName));
                        IList<BusiBlocks.DocoBlock.Category> docoCategories = DocoManager.GetAllCategoriesBelow(cat.Id);

                        foreach (BusiBlocks.DocoBlock.Category item in docoCategories)
                        {
                            listItems.Add(new KeyValuePair<string, string>(item.Id, item.DisplayName));
                        }
                        break;
                    case "News":
                        BusiBlocks.CommsBlock.News.Category selectedNews = NewsManager.GetCategoryByName(nodeName, true);
                        IList<BusiBlocks.CommsBlock.News.Category> news = NewsManager.GetCategories(selectedNews.Id, true);

                        foreach (BusiBlocks.CommsBlock.News.Category newsItem in news)
                        {
                            listItems.Add(new KeyValuePair<string, string>(newsItem.Id, newsItem.Name));
                        }
                        break;
                    default: break;
                }
                break;
            case "Region":
                BusiBlocks.SiteLayer.Region selectedRegion = SiteManager.GetRegionByName(nodeName);
                if (selectedRegion != null)
                {
                    listItems.Add(new KeyValuePair<string, string>(selectedRegion.Id, selectedRegion.Name));
                    IList<Region> regions = SiteManager.GetAllRegionsBelow(selectedRegion);
                    foreach (Region region in regions)
                    {
                        listItems.Add(new KeyValuePair<string, string>(region.Id, region.Name));
                    }
                }
                break;
            default:
                break;
        }
        return listItems;
    }
    [WebMethod]
    public object GetSelectedBelowNodeItems(string nodeId, string treeViewType, string treeViewName)
    {
        List<KeyValuePair<string, string>> listSites = new List<KeyValuePair<string, string>>();

        switch (treeViewType)
        {
            case "Category":
                switch (treeViewName)
                {
                    case "Doco":
                        break;
                    case "News":
                        break;
                    default:
                        break;
                }
                break;
            case "Region":
                Region region = SiteManager.GetRegionById(nodeId);
                IList<Site> sites = SiteManager.GetAllSitesByRegion(region, true);
                foreach (Site site in sites)
                {
                    listSites.Add(new KeyValuePair<string, string>(site.Id, site.Name));
                }
                break;
            default:
                break;
        }

        return listSites;
    }
}
