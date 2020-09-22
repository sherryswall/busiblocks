using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using BusiBlocks.Membership;
using System.Web.Security;
using Telerik.Web.UI;

public partial class Controls_ManageGroups : System.Web.UI.UserControl
{

    private const string PersonList = "personList";

    private RadGrid radgrid { get { return (RadGrid)pupUsers.GetCustomContent("RadGrid1"); } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ViewState[PersonList] = string.Join(",", RegionVisibilityHelper.GetPersonsForUser(Page.User.Identity.Name).Select(x => x.Id).ToArray());
            radgrid.DataSource = GetDataSource();
            radgrid.DataBind();

        }
    }

    protected void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        int[] selectedItems = lstUsers.GetSelectedIndices();
        for (int i = 0; i < selectedItems.Length; i++)
        {
            lstUsers.Items.RemoveAt(selectedItems[i]);
        }
    }

    protected void btnShowUsersList_Click(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
            radgrid.Visible = true;
    }

    public PersonType GetGroupDetails()
    {
        return new PersonType { Name = txtBxGroupName.Text, Description = txtBxDescription.Text };
    }

    public void DisplayGroupDetails(PersonType editPersonType)
    {
        txtBxGroupName.Text = editPersonType.Name;
        txtBxDescription.Text = editPersonType.Description;
    }

    public void SetList(ListBox list)
    {
        foreach (ListItem item in list.Items)
        {
            lstUsers.Items.Add(item);
        }
    }

    public List<string> GetList()
    {
        List<string> userNamesList = new List<string>();
        foreach (ListItem item in lstUsers.Items)
        {
            userNamesList.Add(item.Text);
        }
        return userNamesList;
    }

    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        radgrid.DataSource = GetDataSource();
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
        }
    }

    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "add")
        {
            string personId = (string)e.CommandArgument;

            Person person = PersonManager.GetPersonById(personId);
            User user = MembershipManager.GetUserByPerson(person);

            if (lstUsers.Items.FindByText(user.Name) == null)
            {
                lstUsers.Items.Add(user.Name);
            }

        }
    }

    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        radgrid.PageSize = e.NewPageSize;
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

        seperator = (isPrimaryExisting == true) ? ",&nbsp;" : string.Empty;
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
            sites += siteName +"&nbsp;";
        }
        return sites;
    }

    protected class PersonWithUser
    {
        public Person Person { get; set; }
        public User User { get; set; }
        public Site PrimarySite { get; set; }
    }

}