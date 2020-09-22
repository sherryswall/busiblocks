using System;
using System.Collections.Generic;
using System.Linq;
using BusiBlocks.PersonLayer;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using Telerik.Web.UI;
using System.Web.Security;
using BusiBlocks.Membership;
using System.Web.UI.WebControls;
using BusiBlocks.Audit;
using System.Web.UI;
using System.Web.Services;

public partial class Admin_SearchUsers : System.Web.UI.Page
{
    private const string PersonList = "personList";
    public static string FilterExpression { get; set; }
    public static object originalDS { get; set; }//holds the value of radgrid datasource on first page load. used when filtering items.
    public static object currentDS { get; set; }//holds the value of radgrid datasource on filter. used when sorting items.

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        // Display the persons visible to this user.
        ViewState[PersonList] = string.Join(",", RegionVisibilityHelper.GetPersonsForUser(Page.User.Identity.Name).Select(x => x.Id).ToArray());
        if (!Page.IsPostBack)
        {
            FilterExpression = string.Empty;
            originalDS = null;
            currentDS = null;
        }
    }

    protected void btnCreatePerson_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageUsers(string.Empty).Redirect(this);
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

        List<PersonWithUser> personsAndUsers = new List<PersonWithUser>();

        if (!string.IsNullOrEmpty(FilterExpression))
            BindWithFilter(personsAndUsers);
        else
            SearchDefault(personsAndUsers);
    }

    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            PersonWithUser user = (PersonWithUser)e.Item.DataItem;

            string personId = ((HiddenField)item.FindControl("hfPersonId")).Value;
            bool isPrimaryExisting = false;
            string userName = string.Empty;
            if (user.User != null)
                userName = (string.IsNullOrEmpty(user.User.Name)) ? string.Empty : user.User.Name;
            //add primary site if any for the person
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

            //add secondary sites if any for the person.
            Label sites = new Label();
            sites = (Label)item.FindControl("lblSecondarySites");
            sites.Text = GetSecondarySites(personId, isPrimaryExisting);

            //append modal popup on the delete link.
            LinkButton lnkBtnDelete = new LinkButton();
            lnkBtnDelete = (LinkButton)item.FindControl("lnkBtnDelete");

            lnkBtnDelete.OnClientClick = "showDeleteUserPopup('" + personId + "','" + userName + "');";
        }
    }

    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {

        if (e.CommandName == "edit")
        {
            var id = (string)e.CommandArgument;
            Navigation.Admin_ManageUsers(id).Redirect(this);
        }
        //custom filter function being called.
        else if (e.CommandName == RadGrid.FilterCommandName)
        {
            List<PersonWithUser> personsAndUsers = new List<PersonWithUser>();
            //cancelling the default functionality of the filter! as that only filters by primary site's name using the DataField.


            Pair filterPair = (Pair)e.CommandArgument;
            switch (filterPair.Second.ToString())
            {
                case "PrimarySite":
                    e.Canceled = true;
                    TextBox tbPattern = (e.Item as GridFilteringItem)["PrimarySite"].Controls[0] as TextBox;
                    FilterExpression = tbPattern.Text;

                    BindWithFilter(personsAndUsers);
                    RadGrid1.DataBind();
                    break;
                default:
                    break;
            }
        }
        else if (e.CommandName == RadGrid.SortCommandName)
        {
            if (currentDS != null)
            {
                RadGrid1.DataSource = currentDS;
                RadGrid1.DataBind();
            }
        }
    }

    protected void RadGrid1_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
    }

    #endregion

    #region Functions

    /// <summary>
    /// Get primary site for a person
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>   
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

    /// <summary>
    /// Gets secondary sites if any for a person
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isPrimaryExisting"></param>
    /// <returns>string of secondary sites</returns>     
    protected string GetSecondarySites(string id, bool isPrimaryExisting)
    {
        string secondarySites = string.Empty;
        string seperator = string.Empty;

        seperator = (isPrimaryExisting == true) ? ", " : string.Empty;
        //get user and person
        Person queryPerson = PersonManager.GetPersonById(id);
        User user1 = MembershipManager.GetUserByPerson(queryPerson);

        if (user1 != null && !string.IsNullOrEmpty(user1.Name))
        {
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
                            secondarySites = CombineSites(seperator + sites[i].Name, secondarySites);
                        }
                    }
                    else
                    {
                        secondarySites = CombineSites(sites[i].Name + ((i == (sites.Count - 1)) ? string.Empty : ", "), secondarySites);
                    }
                }
            }
        }
        return secondarySites;
    }

    /// <summary>
    /// Combines the primary site and secondary site into a string
    /// </summary>
    /// <param name="siteName"></param>
    /// <param name="sites"></param>
    /// <returns>Concatenated string</returns>
    private string CombineSites(string siteName, string sites)
    {
        if (!string.IsNullOrEmpty(siteName))
        {
            sites += siteName;
        }
        return sites;
    }

    /// <summary>
    /// Deletes a user after confirming from Modal Popup. Accepts button's default parameters.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DeleteUserClick(object sender, EventArgs e)
    {
        string personId = popDeleteUser.ReferrerId;

        try
        {
            Person person = PersonManager.GetPersonById(personId);
            User user = MembershipManager.GetUserByPerson(person);

            // Do not delete person if they are in a site or group.
            if (PersonManager.IsPersonInPersonRegion(personId) ||
                PersonManager.IsPersonInPersonSite(personId) ||
                PersonManager.IsPersonInPersonType(personId))
            {
                ((IFeedback)Page.Master).SetError(
                    GetType(),
                    Utilities.GetDisplayUserId(user.Id) + " could not be deleted because they are assigned to a Site, Region or Group"
                );
                return;
            }

            PersonManager.DeletePerson(person.Id);

            //remove from ViewState
            List<string> personList = ViewState[PersonList].ToString().Split(',').ToList<string>();
            personList.Remove(personId);
            ViewState[PersonList] = string.Join(",", personList.ToArray());

            ((IFeedback)Master).ShowFeedback(BusiBlocksConstants.Blocks.Administration.BlockName, user.GetType().Name, Feedback.Actions.Deleted, user.Name);
            //if person deletes itself..logout him out.
            if (user.Name.Equals(Utilities.GetUserName(Page.User.Identity.Name)))
            {
                FormsAuthentication.SignOut();
                Navigation.Admin_SearchUsers().Redirect(this);
            }

            RadGrid1.MasterTableView.Rebind();
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    /// <summary>
    /// A Custom list of PersonWithUser constructed by searching the concatenated primary and secondary sites. Acts like a custom filter.
    /// </summary>
    /// <param name="personsAndUsers"></param>
    /// <param name="siteName"></param>
    protected void AddSiteToList(List<PersonWithUser> personsAndUsers, string siteName)
    {
        if (!string.IsNullOrEmpty(siteName))
        {
            //setting the DS to original list to perform filtering. Only assign it once when this function is called in loop for ',' seperated list.
            if (RadGrid1.DataSource != originalDS)
            {
                RadGrid1.DataSource = originalDS;
                RadGrid1.DataBind();
            }
            //loop through the radgrid rows.Combine primary and secondary sites and check if site name exists in it.
            foreach (GridDataItem item in RadGrid1.Items)
            {
                string userName = ((LinkButton)item["LoginId"].FindControl("imgBtnView")).Text;
                Label lblPrimarySite = (Label)item["PrimarySite"].FindControl("lblPrimarySite");
                Label lblSecondarySites = (Label)item["PrimarySite"].FindControl("lblSecondarySites");
                string sites = lblPrimarySite.Text + lblSecondarySites.Text;
                //if site name is existing in the concatenated string then add it to the PersonUser list.
                if (!string.IsNullOrEmpty(userName) && sites.ToLower().Contains(siteName.ToLower()))
                {
                    Person person = PersonManager.GetPersonByUserName(userName);
                    User user = MembershipManager.GetUserByName(userName);
                    Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                    if (defaultSite == null)
                    {
                        defaultSite = new Site();
                    }
                    if (person != null && user != null)
                    {
                        PersonWithUser personUser = new PersonWithUser() { Person = person, User = user, PrimarySite = defaultSite };
                        personsAndUsers.Add(personUser);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Binds radgrid to a new DS using results based on filter expression. Filter expression is typed into filter textbox on RadGrid.
    /// </summary>
    /// <param name="personsAndUsers"></param>
    protected void BindWithFilter(List<PersonWithUser> personsAndUsers)
    {
        if (!string.IsNullOrEmpty(FilterExpression))
        {
            //comma seperated list
            if (FilterExpression.Contains(","))
            {
                string[] values = FilterExpression.Split(',');
                foreach (string cat in values)
                {
                    AddSiteToList(personsAndUsers, cat);
                }
            }
            //single value
            else
            {
                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    AddSiteToList(personsAndUsers, FilterExpression);
                }
            }
            RadGrid1.DataSource = personsAndUsers;
            currentDS = personsAndUsers;
        }
        else
        {
            SearchDefault(personsAndUsers);
        }
    }

    /// <summary>
    /// Default list to which the RadGrid is databound. Uses viewstate that is populated using person's sites
    /// </summary>
    /// <param name="personsAndUsers"></param>
    protected void SearchDefault(List<PersonWithUser> personsAndUsers)
    {
        object personListObj = ViewState[PersonList];
        if (personListObj != null)
        {
            if (!string.IsNullOrEmpty(personListObj.ToString()))
            {
                string[] personList = personListObj.ToString().Split(',');
                foreach (string personId in personList)
                {
                    Person person = PersonManager.GetPersonById(personId);
                    if (!person.Deleted)
                    {
                        BusiBlocks.Membership.User user = BusiBlocks.Membership.MembershipManager.GetUserByPerson(person);
                        Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                        if (defaultSite == null)
                            defaultSite = new Site();
                        personsAndUsers.Add(new PersonWithUser { Person = person, User = user, PrimarySite = defaultSite });
                    }
                }
            }
        }
        RadGrid1.DataSource = personsAndUsers;
        originalDS = personsAndUsers;
        currentDS = null;
    }

    private IEnumerable<Person> FindAllPersonDetails(IEnumerable<string> userNameParts)
    {
        var persons = new List<Person>();
        foreach (string part in userNameParts)
        {
            persons.AddRange(PersonManager.SearchPersonsByLastName(part));
            persons.AddRange(PersonManager.SearchPersonsByFirstName(part));
        }
        return persons;
    }

    #endregion

    [WebMethod]
    public static object wmGetPersonalDetails(string userName)
    {
        Person currentPerson = PersonManager.GetPersonByUserName(userName);
        return currentPerson;
    }

    protected class PersonWithUser
    {
        public Person Person { get; set; }
        public BusiBlocks.Membership.User User { get; set; }
        public Site PrimarySite { get; set; }
    }
}