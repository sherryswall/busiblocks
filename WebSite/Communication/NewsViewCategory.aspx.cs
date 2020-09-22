using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using BusiBlocks.AccessLayer;
using BusiBlocks;
using BusiBlocks.CommsBlock.News;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using System.Web.Services;
using System.Web;

public partial class Communication_NewsViewCategory : System.Web.UI.Page
{
    private string _categoryId;
    public virtual string CategoryId
    {
        get { return _categoryId; }
        set { _categoryId = value; }
    }
    public static bool CanEdit { get; set; }
    public static Navigation.NavigateNewItem newLink { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["RefUrl"] = Request.UrlReferrer.ToString();
        }
        //check previous page's URL for the id if Id hasn't been brought forward.
        //if (Request.UrlReferrer.Query.Contains("id"))
        //    CategoryId = Request.UrlReferrer.Query.Substring(Request.UrlReferrer.Query.IndexOf("id") + 3);
        //else
            CategoryId = Request["id"]; //check for id in URL

        if (!string.IsNullOrEmpty(CategoryId))
        {
            Category category = null;
            if (!string.IsNullOrEmpty(CategoryId))
                category = NewsManager.GetCategory(CategoryId);

            if (!SecurityHelper.CanUserView(User.Identity.Name, category.Id))
            {
                // If the user cannot view the category, then return silently.
                object refUrl = ViewState["RefUrl"];
                if (refUrl != null)
                    Response.Redirect((string)refUrl);
            }

            lblDisplayName.InnerText = category.Name;

            var link = new HtmlLink();
            link.Href = Navigation.Doco_CategoryRss(CategoryId).GetServerUrl(true);
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("type", "application/rss+xml");
            link.Attributes.Add("title", "Category " + category.Name + " Announcements");
            Header.Controls.Add(link);
            
            string url = Navigation.Communication_NewsNewItem(category).GetAbsoluteClientUrl(true);
            bool access = SecurityHelper.CanUserEdit(Page.User.Identity.Name, category.Id);
            newLink = new Navigation.NavigateNewItem(url, access);

            if (category.ParentCategory != null)
                lblParentCategoryName.Text = category.ParentCategory.Name;

            LoadPermissionsView();

            LoadList(category);
        }
    }

    private void LoadPermissionsView()
    {
        // Copied from Controls/AccessControl.ascx.cs

        const string allGroupsLabel = "All Groups";
        const string allLocationsLabel = "All Sites";

        IList<string> viewingItems = new List<string>();
        IList<string> editingItems = new List<string>();

        if (!string.IsNullOrEmpty(CategoryId))
        {
            foreach (Access access in AccessManager.GetItemAccess(CategoryId))
            {
                string personType = string.Empty;
                string site = string.Empty;

                if (access.AllPersonTypes)
                    personType = allGroupsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.PersonTypeId))
                        personType = PersonManager.GetPersonTypeById(access.PersonTypeId).Name;
                }

                if (access.AllSites)
                    site = allLocationsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.SiteId))
                        site = SiteManager.GetSiteById(access.SiteId).Name;
                }

                if (!string.IsNullOrEmpty(personType) && !string.IsNullOrEmpty(site))
                {
                    string listItem = string.Format("{0} from {1}", personType, site);
                    if (access.AccessType == AccessType.View)
                    {
                        viewingItems.Add(listItem);
                    }
                    else if (access.AccessType == AccessType.Edit)
                    {
                        editingItems.Add(listItem);
                    }
                }
            }
        }

        lstSummaryViewing.DataSource = viewingItems;
        lstSummaryViewing.DataBind();
        lstSummaryEditing.DataSource = editingItems;
        lstSummaryEditing.DataBind();
    }
    
    private void LoadList(Category category)
    {        
        // Add all the announcement instances to the list.
        list.CategoryId = category.Id;
        list.Bind(category.Name);
    }



    [WebMethod]
    public static Navigation.NavigateNewItem wmCheckEditAccess()
    {

        bool access = false;
        string URL = string.Empty;
        string categoryId = HttpContext.Current.Request.QueryString["id"];
        string userName = HttpContext.Current.User.Identity.Name;

        if (!string.IsNullOrEmpty(categoryId) && (SecurityHelper.CanUserEdit(userName, categoryId)))
        {
            Category category = NewsManager.GetCategory(categoryId);
            URL = Navigation.Communication_NewsNewItem(category).GetAbsoluteClientUrl(true);
            access = true;
        }
        
        return new Navigation.NavigateNewItem(URL, access);
    }


}
