using System;
using System.Collections.Generic;
using System.Linq;
using BusiBlocks;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using Telerik.Web.UI;
using System.Web;

public partial class Directory_DirectorySearch : System.Web.UI.Page
{
    private const string PersonList = "personList";
    private const string SiteList = "siteList";    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session[PersonList] != null)
            {
                ViewState[PersonList] = Session[PersonList];
                Session[PersonList] = null;
            }
        }
    }

    #region Person
    
    protected void RadGrid1_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "User.Name";
            expression.SortOrder = GridSortOrder.Ascending;
            RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
        }
        if (RadGrid2.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "Name";
            expression.SortOrder = GridSortOrder.Ascending;
            RadGrid2.MasterTableView.SortExpressions.AddSortExpression(expression);
        }

        object personsList = ViewState[PersonList];
        if (personsList == null || personsList is string)
        {
            var personsAndUsers = new List<PersonWithUser>();
            foreach (Person person in PersonManager.GetAllPersons())
            {
                BusiBlocks.Membership.User user = BusiBlocks.Membership.MembershipManager.GetUserByPerson(person);
                Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                if (defaultSite == null) defaultSite = new Site();
                personsAndUsers.Add(new PersonWithUser {Person = person, User = user, PrimarySite = defaultSite});
            }
            RadGrid1.DataSource = personsAndUsers;
        } else if (personsList is IList<string>)
        {
            var personIdList = personsList as IList<string>;
            var personList = new List<PersonWithUser>();
            foreach (string personId in personIdList)
            {
                Person person = PersonManager.GetPersonById(personId);
                BusiBlocks.Membership.User user = BusiBlocks.Membership.MembershipManager.GetUserByPerson(person);
                Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                if (defaultSite == null) defaultSite = new Site();
                personList.Add(new PersonWithUser { Person = person, User = user, PrimarySite = defaultSite });
            }
            RadGrid1.DataSource = personList;
        }
    }

    protected void RadGrid1_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "view")
        {
            string id = (string)e.CommandArgument;
            Navigation.Directory_PersonDetails(id).Redirect(this);
        }
    }

    protected void RadGrid1_PageSizeChanged(object sender, Telerik.Web.UI.GridPageSizeChangedEventArgs e)
    {
        RadGrid1.PageSize = e.NewPageSize;
        ViewState[PersonList] = "all";
    }

    #endregion

    #region Site

    protected void RadGrid2_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        object siteList = ViewState[SiteList];
        if (siteList == null || siteList is string)
        {
            RadGrid2.DataSource = SiteManager.GetAllSites();
        } else if (siteList is IList<string>)
        {
            var siteListArray = siteList as IList<string>;
            var sites = new List<Site>();
            foreach (string siteId in siteListArray)
            {
                sites.Add(SiteManager.GetSiteById(siteId));
            }
            RadGrid2.DataSource = sites;
        }
    }

    protected void RadGrid2_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "view")
        {
            string id = (string)e.CommandArgument;
            Navigation.Directory_SiteDetails(id).Redirect(this);
        }
    }

    protected void RadGrid2_PageSizeChanged(object sender, Telerik.Web.UI.GridPageSizeChangedEventArgs e)
    {
        RadGrid2.PageSize = e.NewPageSize;
        RadGrid2.Rebind();
    }

    #endregion

    private IEnumerable<Person> FindAllPersonDetails(string[] userNameParts)
    {
        var persons = new List<Person>();
        IList<Person> allPersons = PersonManager.GetAllPersons();

        foreach (string part in userNameParts)
        {
            var matches = from x in allPersons
                          where x.FirstName.ToUpper().Contains(part.ToUpper()) ||
                          x.LastName.ToUpper().Contains(part.ToUpper())
                          select x;

            persons.AddRange(matches);
        }
        return persons;
    }

    private class PersonWithUser
    {
        public Person Person { get; set; }
        public BusiBlocks.Membership.User User { get; set; }
        public Site PrimarySite { get; set; }
    }
}