using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using BusiBlocks.Membership;
using System.Web.Security;
using BusiBlocks;
using System.Web.Services;

public partial class Controls_UsersList : System.Web.UI.UserControl
{
    private const string PersonList = "personList";
    public static bool showAddAction = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ViewState[PersonList] = string.Join(",", RegionVisibilityHelper.GetPersonsForUser(Page.User.Identity.Name).Select(x => x.Id).ToArray());
            RadGrid1.DataSource = GetDataSource();
            RadGrid1.DataBind();
        }
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "User.Name";
            expression.SortOrder = GridSortOrder.Ascending;
            RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
        }
        RadGrid1.DataSource = GetDataSource();
    }

    protected List<PersonWithUser> GetDataSource()
    {
         List<PersonWithUser> personsAndUsers = new List<PersonWithUser>();
       
        object personListObj = ViewState[PersonList];
        if (personListObj != null)
        {
            if (!string.IsNullOrEmpty(personListObj.ToString()))
            {
                string[] personList = personListObj.ToString().Split(',');
                foreach (string personId in personList)
                {
                    Person person = PersonManager.GetPersonById(personId);
                    BusiBlocks.Membership.User user = BusiBlocks.Membership.MembershipManager.GetUserByPerson(person);
                    Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                    if (defaultSite == null) defaultSite = new Site();
                    personsAndUsers.Add(new PersonWithUser { Person = person, User = user, PrimarySite = defaultSite });
                }
            }
        }
        return personsAndUsers;
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            string personId = ((HiddenField)item.FindControl("hfPersonId")).Value;
            bool isPrimaryExisting = false;

            Label primarySite = new Label();
            primarySite = (Label)item.FindControl("lblPrimarySite");
            primarySite.CssClass = "bold";
            primarySite.Text = GetPrimarySite(personId);

            if (!string.IsNullOrEmpty(primarySite.Text))
            {
                isPrimaryExisting = true;
                primarySite.Text += "<img src=../App_Themes/default/icons/star_16.png />";
            }
            else
                isPrimaryExisting = false;

            Label sites = new Label();
            sites = (Label)item.FindControl("lblSecondarySites");

            sites.Text = GetSecondarySites(personId, isPrimaryExisting);

            //show the add link which will add users to the users in a listbox.
            LinkButton lnkBtnAdd = (LinkButton)item.FindControl("lnkBtnAdd");
            lnkBtnAdd.OnClientClick = "javascript:addToList('" + lnkBtnAdd.CommandName.ToString() + "','" + personId + "');";

            lnkBtnAdd.Text = "<img src=\"../App_Themes/default/icons/add.png\" />Add";
        }
    }

    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "view")
        {
            string id = (string)e.CommandArgument;
            Navigation.Directory_PersonDetails(id).Redirect(this);
        }
        else if (e.CommandName == "delete")
        {
            try
            {
                var id = (string)e.CommandArgument;
                Person person = PersonManager.GetPersonById(id);
                PersonManager.DeletePerson(person.Id);
                RadGrid1.Rebind();
            }
            catch (Exception ex)
            {
                throw ex;
                ((IFeedback)this.Page.Master).SetException(GetType(), ex);
            }
        }
        else if (e.CommandName == "edit")
        {
            var id = (string)e.CommandArgument;
            Navigation.Admin_ManageUsers(id).Redirect(this);
        }
    }

    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
    }

    protected string GetPrimarySite(string Id)
    {
        //get user and person
        Person queryPerson = PersonManager.GetPersonById(Id);
        User user1 = MembershipManager.GetUserByPerson(queryPerson);

        //get primary site to avoid duplicate site names 
        Site primarySite = PersonManager.GetDefaultSiteByPerson(queryPerson);

        if (primarySite != null && !string.IsNullOrEmpty(primarySite.Name))
            return primarySite.Name;
        else
            return string.Empty;
    }

    protected string GetSecondarySites(string id, bool isPrimaryExisting)
    {
        string secondarySites = string.Empty;
        string seperator = string.Empty;

        seperator = (isPrimaryExisting == true) ? "," : string.Empty;
        //get user and person
        Person queryPerson = PersonManager.GetPersonById(id);
        User user1 = MembershipManager.GetUserByPerson(queryPerson);

        //get primary site to avoid duplicate site names 
        Site primarySite = PersonManager.GetDefaultSiteByPerson(queryPerson);

        MembershipUser membershipUser = Membership.GetUser(user1.Name);

        if (!string.IsNullOrEmpty(membershipUser.UserName))
        {
            IList<Site> sites = SiteManager.GetSitesByUser(membershipUser.UserName, true);

            for (int i = 0; i < sites.Count; i++)
            {
                if (primarySite != null)
                {
                    //check if not primary site
                    if ((primarySite.Id != sites[i].Id) && !string.IsNullOrEmpty(sites[i].Name))
                    {
                        secondarySites = CombineSites(seperator + sites[i].Name, secondarySites, i, sites.Count - 1);
                    }
                }
                else
                {
                    secondarySites = CombineSites(sites[i].Name, secondarySites, i, sites.Count - 1);
                }
            }
        }
        return secondarySites;
    }

    private string CombineSites(string siteName, string sites, int siteCount, int sitesCount)
    {
        if (!string.IsNullOrEmpty(siteName))
        {
            sites += siteName;
        }
        return sites;
    }

    [WebMethod]
    public void wmShowAddLink()
    {
        showAddAction = true;
    }

    protected class PersonWithUser
    {
        public Person Person { get; set; }
        public BusiBlocks.Membership.User User { get; set; }
        public Site PrimarySite { get; set; }
    }

}