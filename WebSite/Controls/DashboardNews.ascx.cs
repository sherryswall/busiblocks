using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using BusiBlocks.CommsBlock.News;
using BusiBlocks;

public partial class Controls_DashboardNews : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadNews();

        sectionError.Visible = false;
        listRepeater.Visible = true;
    }

    private void LoadNews()
    {
        //if (BusiBlocks.SecurityHelper.CanRead(Page.User, category, null) == false)
        //    throw new BusiBlocks.InvalidPermissionException("read category");
        var news = new List<Item>();

        IList<Access> accessibleList = AccessManager.GetUsersAccessibleItems(Page.User.Identity.Name, BusiBlocks.ItemType.NewsItem, BusiBlocks.AccessType.View);

        foreach (Access access in accessibleList)
        {
            Category category = NewsManager.GetCategory(access.ItemId);
            news.AddRange(NewsManager.GetPublishedItems(category, false));
        }

        List<KeyValuePair<Item, TrafficLightStatus>> newsWithTLList = new List<KeyValuePair<Item, TrafficLightStatus>>();
        if (accessibleList != null)
        {
            foreach (Item newsItem in news)
            {
                if (!newsWithTLList.Exists(i => i.Key.Id == newsItem.Id))
                {
                    if (newsItem != null)
                    {
                        TrafficLightStatus tflStatus = NewsManager.GetTrafficLight(Page.User.Identity.Name, newsItem);
                        if (tflStatus.RequiresAck)
                        {
                            if (tflStatus.Acknowledged)
                                continue;
                        }
                        else
                        {
                            if (tflStatus.Viewed)
                            {
                                continue;
                            }
                        }
                        if (newsWithTLList.Count < 5)
                        {
                            newsWithTLList.Add(new KeyValuePair<Item, TrafficLightStatus>(newsItem, tflStatus));
                        }
                    }
                }
            }
        }

        lblNoResults.Visible = newsWithTLList.Count == 0;

        listRepeater.DataSource = newsWithTLList;
        listRepeater.DataBind();
    }

    protected string GetViewUrl(Item item)
    {
        return Navigation.Communication_NewsViewItem(item.Id).GetClientUrl(Page, true) + "&mode=view";
    }

    protected string GetShortDescription(Item item)
    {
        if (item.Description == null)
            return string.Empty;

        if (item.Description.Length > 100)
            return item.Description.Substring(0, 100) + "...";
        else
            return item.Description;
    }

    protected void listRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        KeyValuePair<Item, TrafficLightStatus> item = (KeyValuePair<Item, TrafficLightStatus>)e.Item.DataItem;
        HyperLink lnkItem = (HyperLink)e.Item.FindControl("lnkItem");
        lnkItem.Text = item.Key.Title;
        lnkItem.NavigateUrl = GetViewUrl(item.Key);

        Image imgItem = (Image)e.Item.FindControl("imgItem");
        imgItem.ImageUrl = Utilities.GetTrafficLightImageUrl(item.Value.RequiresAck, (item.Value.Acknowledged || item.Value.Viewed));
        
        Label lblAuthor = (Label)e.Item.FindControl("lblAuthor");
        lblAuthor.Text = item.Key.Author;
    }
}