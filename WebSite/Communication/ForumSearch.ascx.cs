using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

public partial class Communication_ForumSearch : System.Web.UI.UserControl
{
    private const int ListPagingSize = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        lnkBack.HRef = Navigation.Communication_Default(2).GetServerUrl(true);

        searchResult.LinkNextClick += new EventHandler(searchResult_LinkNextClick);
        searchResult.LinkPreviousClick += new EventHandler(searchResult_LinkPreviousClick);
        searchResult.SearchEntityUrlCallback += new Controls_SearchResult.SearchEntityUrlDelegate(searchResult_SearchEntityUrlCallback);
    }

    public void LoadForums(string forumName)
    {
        IList<BusiBlocks.CommsBlock.Forums.Category> forums = BusiBlocks.CommsBlock.Forums.ForumsManager.GetAllCategories();
        listForum.Items.Clear();

        foreach (BusiBlocks.CommsBlock.Forums.Category category in forums)
        {
            if (BusiBlocks.SecurityHelper.CanUserView(Page.User.Identity.Name, category.Id))
            {
                var listItem = new ListItem(category.DisplayName);
                
                if (string.IsNullOrEmpty(forumName))
                    listItem.Selected = true;
                else if (forumName == category.Name)
                    listItem.Selected = true;
                else
                    listItem.Selected = false;

                listItem.Value = category.Id;

                listForum.Items.Add(listItem);
            }
        }
    }

    private string[] GetSelectedForums()
    {
        var forums = new List<string>();

        foreach (ListItem item in listForum.Items)
        {
            if (item.Selected)
            {
                BusiBlocks.CommsBlock.Forums.Category category = BusiBlocks.CommsBlock.Forums.ForumsManager.GetCategory(item.Value);
                //if (BusiBlocks.SecurityHelper.CanRead(User, category, null))
                {
                    forums.Add(category.Name);
                }
            }
        }

        return forums.ToArray();
    }

    void searchResult_LinkPreviousClick(object sender, EventArgs e)
    {
        LoadList(searchResult.CurrentPage - 1);
    }

    void searchResult_LinkNextClick(object sender, EventArgs e)
    {
        LoadList(searchResult.CurrentPage + 1);
    }

    Navigation.NavigationPage searchResult_SearchEntityUrlCallback(BusiBlocks.ISearchResult entity)
    {
        var msg = (BusiBlocks.CommsBlock.Forums.Message)entity;
        return Navigation.Communication_ForumViewTopic(msg.Topic.Id);
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        LoadList(0);
    }

    private void LoadList(int page)
    {
        try
        {
            string[] searchFor = BusiBlocks.SplitHelper.SplitSearchText(txtSearchFor.Text);
            string[] authorSearch = BusiBlocks.SplitHelper.SplitSearchText(txtAuthor.Text);

            var paging = new BusiBlocks.PagingInfo(ListPagingSize, page);
            IList<BusiBlocks.CommsBlock.Forums.Message> messages = BusiBlocks.CommsBlock.Forums.ForumsManager.FindMessages(
                                                            BusiBlocks.Filter.MatchOne( GetSelectedForums() ),
                                                            BusiBlocks.Filter.ContainsAll( searchFor ), 
                                                            BusiBlocks.Filter.ContainsOne( authorSearch ),
                                                            null,
                                                            null, null, 
                                                            paging);

            searchResult.LoadList(messages, page, (int)paging.PagesCount);
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Page.Master).SetException(GetType(), ex);
        }
    }
}
