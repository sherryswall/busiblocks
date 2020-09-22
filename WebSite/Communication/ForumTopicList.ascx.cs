using System;
using System.Collections.Generic;
using BusiBlocks.PersonLayer;

public partial class Controls_TopicList : System.Web.UI.UserControl
{
    private const int ListPagingSize = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadList();
        }
    }

    #region Properties
    /// <summary>
    /// Gets or sets the forum category name used to load the list of topics
    /// </summary>
    public string CategoryName
    {
        get
        {
            object val = ViewState["CategoryName"];
            return val == null ? null : (string)val;
        }
        set { ViewState["CategoryName"] = value; }
    }

    /// <summary>
    /// The page used for the list pagination.
    /// </summary>
    protected int CurrentPage
    {
        get
        {
            object val = ViewState["CurrentPage"];
            return val == null ? 0 : (int)val;
        }
        set { ViewState["CurrentPage"] = value; }
    }
    #endregion

    private void LoadList()
    {
        if (!string.IsNullOrEmpty(CategoryName))
        {
            BusiBlocks.CommsBlock.Forums.Category category = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategoryByName(CategoryName, true);

            var paging = new BusiBlocks.PagingInfo(ListPagingSize, CurrentPage);
            IList<BusiBlocks.CommsBlock.Forums.Topic> topics = BusiBlocks.CommsBlock.Forums.ForumsManager.GetTopics(category,
                                                        paging);

            var allowedTopics = new List<BusiBlocks.CommsBlock.Forums.Topic>();

            IList<PersonType> groups = PersonManager.GetPersonTypesByUser(Page.User.Identity.Name);

            foreach (BusiBlocks.CommsBlock.Forums.Topic topic in topics)
            {
                if (BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, topic.Category.Id))
                {
                    // If the user is in at least one group, then let them continue.
                    if (groups.Count > 0)
                        allowedTopics.Add(topic);
                }
            }

            lblCurrentPage.InnerText = (CurrentPage + 1).ToString();
            lblTotalPage.InnerText = paging.PagesCount.ToString();

            listRepeater.DataSource = allowedTopics;
            listRepeater.DataBind();

            if (CurrentPage == 0)
                linkPrev.Enabled = false;
            else
                linkPrev.Enabled = true;
            if (CurrentPage + 1 >= paging.PagesCount)
                linkNext.Enabled = false;
            else
                linkNext.Enabled = true;
        }
    }

    protected string GetViewTopicUrl(string idTopic)
    {
        //return Navigation.Forum_ViewTopic(idTopic).GetClientUrl(this, true);
        return "default.aspx?t=2&action=viewf&ftid=" + idTopic;
    }

    protected string GetLastPost(BusiBlocks.CommsBlock.Forums.Topic topic)
    {
        IList<BusiBlocks.CommsBlock.Forums.Message> messages = BusiBlocks.CommsBlock.Forums.ForumsManager.GetMessagesByTopic(topic);

        string status = "{1}<br />&nbsp;&nbsp;by {2}";

        DateTime lastReply = messages[messages.Count - 1].InsertDate;
        string lastUser = messages[messages.Count - 1].Owner;

        status = string.Format(status,
                        messages.Count,
                        Utilities.GetDateTimeForDisplay(lastReply),
                        Utilities.GetDisplayUser(lastUser));

        return status;
    }

    protected int GetRepliesCount(BusiBlocks.CommsBlock.Forums.Topic topic)
    {
        //Remove 1 because it is the topic message
        return BusiBlocks.CommsBlock.Forums.ForumsManager.MessageCountByTopic(topic) - 1;
    }

    protected void linkPrev_Click(object sender, EventArgs e)
    {
        CurrentPage--;
        LoadList();
    }
    protected void linkNext_Click(object sender, EventArgs e)
    {
        CurrentPage++;
        LoadList();
    }
}
