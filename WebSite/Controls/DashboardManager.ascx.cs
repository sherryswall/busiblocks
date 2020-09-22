using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using System.Text;
using BusiBlocks.Audit;
using BusiBlocks.DocoBlock;

public partial class Controls_DashboardManager : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var articlesIManage = new List<Article>();
        var accesses = new List<Access>();

        // Go through each group I watch
        foreach (Access access in Profile.ManagedGroups.Accesses)
        {
            accesses.AddRange(AccessManager.GetItemsMatchingAccess(access, BusiBlocks.ItemType.DocoCategory, BusiBlocks.AccessType.View));
        }

        foreach (Access access in accesses)
        {
            articlesIManage.AddRange(DocoManager.GetArticles(DocoManager.GetCategory(access.ItemId), ArticleStatus.EnabledAndApproved, false));
        }
        
        listDocAckRepeater.DataSource = articlesIManage.Where(delegate(Article article) { return article.RequiresAck; });
        listDocAckRepeater.DataBind();

        listDocViewedRepeater.DataSource = articlesIManage.Where(delegate(Article article) { return !article.RequiresAck; });
        listDocViewedRepeater.DataBind();
    }

    protected void listDocAckRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        (e.Item.FindControl("lblDocTitle") as Label).Text = ((Article)e.Item.DataItem).Title;

        List<BindingItem> items = GetUserStatus((Article)e.Item.DataItem);

        var notAcked = new List<BindingItem>();
        notAcked.AddRange(items.Where(delegate(BindingItem b) { return !b.ViewedOrAcked; }));

        if (notAcked.Count > 0)
        {
            StringBuilder sb = new StringBuilder();

            foreach (BindingItem bi in notAcked)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(bi.Username);
            }

            (e.Item.FindControl("imgItem") as Image).ImageUrl = "../app_themes/default/icons/cube_red.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = notAcked.Count.ToString() + " user(s) have not acknowleged this document (" + sb.ToString() + ")";
        }
        else
        {
            (e.Item.FindControl("imgItem") as Image).ImageUrl = "../app_themes/default/icons/cube_green.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = "All users have acknowledged this document.";
        }
    }

    protected void listDocViewedRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        (e.Item.FindControl("lblDocTitle") as Label).Text = ((Article)e.Item.DataItem).Title;

        List<BindingItem> items = GetUserStatus((Article)e.Item.DataItem);

        var notViewed = new List<BindingItem>();
        notViewed.AddRange(items.Where(delegate(BindingItem b) { return !b.ViewedOrAcked; }));

        if (notViewed.Count > 0)
        {
            StringBuilder sb = new StringBuilder();

            foreach (BindingItem bi in notViewed)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(bi.Username);
            }

            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/app_themes/default/icons/cube_yellow.png";

            (e.Item.FindControl("lblDocMessage") as Label).Text = notViewed.Count.ToString() + " user(s) have not viewed this document (" + sb.ToString() + ")";
        }
        else
        {
            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/app_themes/default/icons/cube_green.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = "All users have viewed this document.";
        }
    }

    protected void listNewsAckRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var data = (KeyValuePair<string, List<string>>)e.Item.DataItem;

        (e.Item.FindControl("lblDocTitle") as Label).Text = data.Key;

        if (data.Value.Count > 0)
        {
            var sb = new StringBuilder();

            foreach (string s in data.Value)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(s);
            }

            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/app_themes/default/icons/cube_red.png";

            (e.Item.FindControl("lblDocMessage") as Label).Text = data.Value.Count.ToString() + " user(s) have not acknowleged this announcement (" + sb.ToString() + ")";
        }
        else
        {
            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/app_themes/default/icons/cube_green.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = "All users have acknowledged this announcement.";
        }
    }

    protected void listNewsViewedRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var data = (KeyValuePair<string, List<string>>)e.Item.DataItem;

        (e.Item.FindControl("lblDocTitle") as Label).Text = data.Key;

        if (data.Value.Count > 0)
        {
            var sb = new StringBuilder();

            foreach (string s in data.Value)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(s);
            }

            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/App_Themes/Default/icons/cube_yellow.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = data.Value.Count.ToString() + " user(s) have not viewed this announcement (" + sb.ToString() + ")";
        }
        else
        {
            (e.Item.FindControl("imgItem") as Image).ImageUrl = "~/App_Themes/Default/icons/cube_green.png";
            (e.Item.FindControl("lblDocMessage") as Label).Text = "All users have viewed this announcement.";
        }
    }

    public struct BindingItem
    {
        public string Username { get; set; }
        public bool ViewedOrAcked { get; set; }
        public bool RequiresAck { get; set; }
    }

    private List<BindingItem> GetUserStatus(Article item)
    {
        var userStatus = new List<BindingItem>();

        IList<Access> accesses = AccessManager.GetItemAccess(item.Category.Id);

        var users = new List<BusiBlocks.Membership.User>();

        foreach (Access access in accesses)
        {
            users.AddRange(AccessManager.GetAccessibleItemUsers(access));
        }

        foreach (BusiBlocks.Membership.User user in users)
        {
            if (item.RequiresAck)
            {
                if (!item.HasUserActioned(user.Name, AuditRecord.AuditAction.Acknowledged))
                {
                    if (!userStatus.Exists(delegate(BindingItem bi) { return bi.Username == user.Name; }))
                    {
                        userStatus.Add(new BindingItem() { Username = user.Name, ViewedOrAcked = false, RequiresAck = item.RequiresAck });
                    }
                }
                else
                {
                    if (!userStatus.Exists(delegate(BindingItem bi) { return bi.Username == user.Name; }))
                    {
                        userStatus.Add(new BindingItem() { Username = user.Name, ViewedOrAcked = true, RequiresAck = item.RequiresAck });
                    }
                }
            }
            else
            {
                if (!item.HasUserActioned(user.Name, AuditRecord.AuditAction.Viewed))
                {
                    if (!userStatus.Exists(delegate(BindingItem bi) { return bi.Username == user.Name; }))
                    {
                        userStatus.Add(new BindingItem() { Username = user.Name, ViewedOrAcked = false, RequiresAck = item.RequiresAck });
                    }
                }
                else
                {
                    if (!userStatus.Exists(delegate(BindingItem bi) { return bi.Username == user.Name; }))
                    {
                        userStatus.Add(new BindingItem() { Username = user.Name, ViewedOrAcked = true, RequiresAck = item.RequiresAck });
                    }
                }
            }
        }
        return userStatus;
    }
}