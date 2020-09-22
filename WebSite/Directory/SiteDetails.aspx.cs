using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using BusiBlocks.AddressLayer;
using BusiBlocks;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using Telerik.Web.UI;

public partial class Directory_SiteDetails : System.Web.UI.Page
{
    private const string PersonList = "personList";
    private const string CurrentSite = "currentSite";

    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request["id"];
        if (!string.IsNullOrEmpty(id))
        {
            RefreshData(id);
        }
    }

    private void RefreshData(string siteId)
    {
        Site currentSite = SiteManager.GetSiteById(siteId);

        if (currentSite != null)
        {
            txtSiteName.Text = currentSite.Name;
            txtSiteType.Text = currentSite.SiteType.Name;
            txtRegion.Text = currentSite.Region.Name;
            txtPhoneNumber.Text = currentSite.PhoneNumber;
            txtAltPhoneNumber.Text = currentSite.AltPhoneNumber;
            txtEmail.Text = currentSite.Email;

            if (currentSite.PostalAddress == null)
                currentSite.PostalAddress = new Address();

            txtPostalAddress1.Text = currentSite.PostalAddress.Address1;
            txtPostalAddress2.Text = currentSite.PostalAddress.Address2;
            txtPostalSuburb.Text = currentSite.PostalAddress.Suburb;
            txtPostalPostcode.Text = currentSite.PostalAddress.Postcode;
            txtPostalState.Text = currentSite.PostalAddress.State;

            if (currentSite.PhysicalAddress == null)
                currentSite.PhysicalAddress = new Address();

            txtPhysicalAddress1.Text = currentSite.PhysicalAddress.Address1;
            txtPhysicalAddress2.Text = currentSite.PhysicalAddress.Address2;
            txtPhysicalSuburb.Text = currentSite.PhysicalAddress.Suburb;
            txtPhysicalPostcode.Text = currentSite.PhysicalAddress.Postcode;
            txtPhysicalState.Text = currentSite.PhysicalAddress.State;

            // Display the persons visible to this user.
            ViewState[PersonList] = string.Join(",", PersonManager.GetAllPersonsInSite(currentSite).Select(x => x.Id).ToArray());
            ViewState[CurrentSite] = currentSite.Name;

            lblHeadingSite.Text = "Site : " + currentSite.Name;
        }
    }

    protected void personRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "view")
        {
            string id = (string)e.CommandArgument;
            Navigation.Directory_PersonDetails(id).Redirect(this);
        }
    }

    protected void RadGrid1_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (RadGrid1.MasterTableView.SortExpressions.Count == 0)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = "Person.LastName";
            expression.SortOrder = GridSortOrder.Ascending;
            RadGrid1.MasterTableView.SortExpressions.AddSortExpression(expression);
        }
        var personsAndUsers = new List<PersonWithUser>();
        object personListObj = ViewState[PersonList];
        if (personListObj != null)
        {
            string personListStr = personListObj.ToString();
            if (!string.IsNullOrEmpty(personListStr))
            {
                if (personListStr.Equals("All"))
                {
                    foreach (Person person in PersonManager.GetAllPersons())
                    {
                        BusiBlocks.Membership.User user = BusiBlocks.Membership.MembershipManager.GetUserByPerson(person);
                        Site defaultSite = PersonManager.GetDefaultSiteByPerson(person);
                        if (defaultSite == null) defaultSite = new Site();
                        personsAndUsers.Add(new PersonWithUser { Person = person, User = user, PrimarySite = defaultSite });
                    }
                }
                else
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

                personsAndUsers.Sort(new KeyComparer<PersonWithUser>(x => x.User.Name));
                RadGrid1.DataSource = personsAndUsers;

                if (personsAndUsers.Count == 0)
                {
                    RadGrid1.Visible = false;
                    headingUser.Visible = false;
                }
                else
                {
                    RadGrid1.Visible = true;
                    headingUser.Visible = true;
                    lblHeadingUser.Text = "Users : " + ViewState[CurrentSite] != null
                                              ? ViewState[CurrentSite].ToString()
                                              : string.Empty;
                }
            }
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
        ViewState[PersonList] = "All";
        ViewState[CurrentSite] = string.Empty;
    }

    private class PersonWithUser
    {
        public Person Person { get; set; }
        public BusiBlocks.Membership.User User { get; set; }
        public Site PrimarySite { get; set; }
    }
}