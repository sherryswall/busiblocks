using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using Telerik.Web.UI;
using System.Web.Services;
using BusiBlocks.SiteLayer;
using BusiBlocks.Membership;
using System.Web.Security;
using BusiBlocks;
using System.Web.Script.Serialization;

public partial class Admin_EditUser : System.Web.UI.Page
{
    public static string Id { get; set; }
    private const string PasswordUnchanged = "Password unchanged";

    IList<PersonType> personTypes = PersonManager.GetAllPersonTypes(false);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
            Id = Request.QueryString["id"].ToString();

        if (!Page.IsPostBack)
        {
            listAllSitesDetails.Value = GetSiteAccessDetails();
            listRegionAccessDetails.Value = GetRegionAccessDetails();

            Person queryPerson = PersonManager.GetAllPersons().FirstOrDefault<Person>(x => x.Id.Equals(Id));
            DisplayPersonalDetails(queryPerson);
            listGroupsDetails.Value = GetPersonGroups(queryPerson);

            //disabling the required field validator for password
            ((RequiredFieldValidator)ctrlPD.FindControl("requiredPassword")).Enabled = false;
            ((Label)ctrlPD.FindControl("lblAsteriskPwd")).Visible = false;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_SearchUsers().Redirect(this);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IList<SiteAccessDetail> allSites = serializer.Deserialize<List<SiteAccessDetail>>(listAllSitesDetails.Value);
            IList<PersonRegionAccessDetail> allRegions = serializer.Deserialize<List<PersonRegionAccessDetail>>(listRegionAccessDetails.Value);
            List<PersonType> allGroups = serializer.Deserialize<List<PersonType>>(listGroupsDetails.Value);

            //save personal details
            Person editedPerson = ctrlPD.GetPersonDetails();
            // Problem is that ctrlPD gives you a new person, not necessarily a real existing person.
            Person aPerson = PersonManager.GetPersonById(editedPerson.Id);
            if (aPerson == null)
            {
                // Find a person using their first name and last name.
                IList<Person> persons = PersonManager.SearchPersonsByFirstName(editedPerson.FirstName);
                if (persons.Count == 1)
                    aPerson = persons[0];
                else if (persons.Count > 1)
                {
                    IList<Person> personsLastName = PersonManager.SearchPersonsByLastName(editedPerson.LastName);
                    var matches = from x in personsLastName where persons.FirstOrDefault(y => y.Id.Equals(x.Id)) != null select x;
                    if (matches.Count() == 1)
                    {
                        aPerson = matches.First();
                    }
                    // Can't find em.
                }
            }
            if (aPerson == null)
                throw new InvalidOperationException("There is no valid user to operate on");

            editedPerson.Address = PersonManager.GetPersonById(editedPerson.Id).Address;
            IList<User> users = MembershipManager.GetAllUsers();
            User user = null;
            foreach (User u in users)
            {
                if (u.Person != null)
                    if (u.Person.Id.Equals(editedPerson.Id))
                        user = u;
            }

            string password = ctrlPD.GetPassword();

            if (!string.IsNullOrEmpty(password))
                user.ChangePassword(password, true);

            user.Name = ctrlPD.GetUserId();

            MembershipManager.UpdateUser(user);
            PersonManager.UpdatePerson(editedPerson);

            //save regions if changed
            if (allRegions != null)
            {
                IList<Region> allRegionsList = SiteManager.GetAllRegions();
                IEnumerable<string> allRegionIds = from st in allRegionsList select st.Id;

                IList<string> currentRegionIds = new List<string>();

                foreach (PersonRegionAccessDetail item in allRegions)
                {
                    Region tempRegion = SiteManager.GetRegionById(item.LocationId);

                    if (tempRegion != null)
                    {
                        PersonRegion PR = new PersonRegion();
                        PR = PersonManager.GetPersonRegionByPersonAndRegion(editedPerson, tempRegion);
                        //if PS is null that means a new site is being added for the person.
                        if (PR != null)
                        {
                            PR.IsView = item.IsView;
                            PR.IsAdministrator = item.IsAdmin;
                            PR.IsManager = item.IsManager;
                            PersonManager.UpdatePersonRegion(PR);
                        }
                        else
                        {
                            PersonManager.AddPersonToRegion(editedPerson.Id, tempRegion.Id, item.IsAdmin, item.IsManager);
                        }
                    }
                    currentRegionIds.Add(item.LocationId);
                }
                //remove regions
                foreach (string regionId in allRegionIds)
                {
                    if (!currentRegionIds.Contains(regionId))
                    {
                        PersonManager.DeletePersonFromRegion(editedPerson.Id, regionId);
                    }
                }
            }
            // save sites if changed
            if (allSites != null)
            {
                IList<Site> personSites = SiteManager.GetSitesByUser(user.Name, true);

                IEnumerable<string> siteIds = from st in personSites select st.Id;

                IList<string> sites = new List<string>();
                foreach (SiteAccessDetail item in allSites)
                {
                    Site tempSite = SiteManager.GetSiteById(item.LocationId);

                    if (tempSite != null)
                    {
                        PersonSite PS = new PersonSite();
                        PS = PersonManager.GetPersonSiteByPersonAndSite(editedPerson, tempSite, true);
                        //if PS is null that means a new site is being added for the person.
                        if (PS != null)
                        {
                            PS.IsAdministrator = item.IsAdmin;
                            PS.IsAssigned = item.IsView;
                            PS.IsDefault = item.IsPrimary;
                            PS.IsManager = item.IsManager;
                            PersonManager.UpdatePersonSite(PS);
                        }
                        else
                        {
                            PersonManager.AddPersonToSite(editedPerson.Id, item.LocationId, item.IsAdmin, item.IsManager, item.IsView, item.IsPrimary);
                        }
                    }
                    sites.Add(item.LocationId);
                }
                //remove sites
                foreach (string siteId in siteIds)
                {
                    if (!sites.Contains(siteId))
                    {
                        PersonManager.DeletePersonFromSite(editedPerson.Id, siteId);
                    }
                }
            }

            // save groups
            foreach (PersonType pType in personTypes)
            {
                if (allGroups.Exists(x => x.Name.Equals(pType.Name)))
                {
                    bool isPersonType = PersonManager.IsPersonInPersonType(editedPerson, pType);

                    if (!isPersonType)
                    {
                        PersonManager.AddPersonToPersonType(editedPerson.Id, pType.Id);
                    }
                }
                else
                {
                    bool isPersonType = PersonManager.IsPersonInPersonType(editedPerson, pType);

                    if (isPersonType)
                    {
                        PersonManager.DeletePersonFromPersonType(editedPerson.Id, pType.Id);
                    }
                }
            }

            //show feedback!
            string usernameLink = "<a href=../Directory/PersonDetails.aspx?id=" + editedPerson.Id + ">" + user.Name + "</a>";
            Feedback feedback = new Feedback(BusiBlocksConstants.Blocks.Documents.BlockName, user.GetType().Name, Feedback.Actions.Saved, usernameLink);
            Session["feedback"] = feedback;
            Navigation.Admin_SearchUsers().Redirect(this);
        }
    }

    public void DisplayPersonalDetails(Person queryPerson)
    {
        ctrlPD.DisplayPersonDetails(queryPerson);
    }

    public string GetPersonGroups(Person queryPerson)
    {
        IList<PersonType> personTypes = PersonManager.GetPersonTypesByPerson(queryPerson);
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        ctrlGD.PopulateUserGroups(queryPerson.Id);

        return serializer.Serialize(personTypes);
    }

    public string GetSiteAccessDetails()
    {
        Person queryPerson = PersonManager.GetAllPersons().FirstOrDefault<Person>(x => x.Id.Equals(Id));
        IList<SiteAccessDetail> sites = new List<SiteAccessDetail>();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        if (queryPerson != null)
        {
            User user1 = MembershipManager.GetUserByPerson(queryPerson);
            MembershipUser membershipUser = Membership.GetUser(user1.Name);

            if (!string.IsNullOrEmpty(membershipUser.UserName))
            {
                IList<Site> userSites = SiteManager.GetSitesByUser(membershipUser.UserName, false);

                foreach (Site site in userSites)
                {
                    PersonSite PS = PersonManager.GetPersonSiteByPersonAndSite(queryPerson, site, false);
                    sites.Add(new SiteAccessDetail() { PersonId = PS.Person.Id, LocationId = site.Id, IsAdmin = PS.IsAdministrator, IsManager = PS.IsManager, IsPrimary = PS.IsDefault, IsView = PS.IsAssigned, Name = site.Name });
                }
            }
        }
        return serializer.Serialize(sites);
    }

    public string GetRegionAccessDetails()
    {
        Person queryPerson = PersonManager.GetAllPersons().FirstOrDefault<Person>(x => x.Id.Equals(Id));
        IList<PersonRegionAccessDetail> regions = new List<PersonRegionAccessDetail>();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        if (queryPerson != null)
        {
            User user1 = MembershipManager.GetUserByPerson(queryPerson);
            MembershipUser membershipUser = Membership.GetUser(user1.Name);

            if (!string.IsNullOrEmpty(membershipUser.UserName))
            {
                IList<Region> userRegions = SiteManager.GetAllRegions();
                foreach (Region region in userRegions)
                {
                    PersonRegion PR = PersonManager.GetPersonRegionByPersonAndRegion(queryPerson, region);
                    if (PR != null)
                        regions.Add(new PersonRegionAccessDetail() { PersonId = PR.Person.Id, LocationId = region.Id, IsView = PR.IsView, IsAdmin = PR.IsAdministrator, IsManager = PR.IsManager, Name = region.Name });
                }
            }
        }
        return serializer.Serialize(regions);
    }
}