using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using BusiBlocks.AddressLayer;
using BusiBlocks.Membership;
using System.Web.Services;
using BusiBlocks;
using Telerik.Web.UI;
using BusiBlocks.AccessLayer;
using System.Web.Script.Serialization;

public partial class Admin_CreateUser : System.Web.UI.Page
{
    private Site newPersonSite { get; set; }
    private User newUser;
    private Person newPerson { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            newPerson = new Person();
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        int stepNumber = tabStripSteps.SelectedIndex;
        if (stepNumber < (tabStripSteps.Tabs.Count - 1))
            stepNumber = stepNumber + 1;

        tabStripSteps.SelectedIndex = stepNumber;
        tabStripSteps.Tabs[stepNumber].Enabled = true;
        mpCreateUserStep.PageViews[stepNumber].Selected = true;

        tabStripSteps.AutoPostBack = false;
        CheckButtons();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        tabStripSteps.SelectedIndex = tabStripSteps.SelectedIndex - 1;
        mpCreateUserStep.PageViews[tabStripSteps.SelectedIndex].Selected = true;
        CheckButtons();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_SearchUsers().Redirect(this);
    }

    protected void btnCreateUser_Click(object sender, EventArgs e)
    {
        TextBox password = (TextBox)ctrlPD.FindControl("txtPassword");
        //Create Person with User.
        newPerson = ctrlPD.GetPersonDetails();
        newUser = MembershipManager.CreateUser(newPerson.Id, password.Text);
        newUser.PasswordChangeRequired = true;
        newUser.Person = newPerson;
        PersonManager.CreatePerson(newUser, newPerson);

        //Attach regions to person if any.
        UpsertRegions(newUser.Person);

        //Attach sites to the person if any.
        UpsertSites(newUser.Person);

        //attach groups to the person if any.
        AddGroups(newUser.Person);

        //create feedback 
        string usernameLink = "<a href=../Directory/PersonDetails.aspx?id=" + newUser.Person.Id + ">" + newUser.Name + "</a>";
        Feedback feedBack = new Feedback(BusiBlocksConstants.Blocks.Documents.BlockName, "User", Feedback.Actions.Created, usernameLink);
        Session["feedback"] = feedBack;

        Navigation.Admin_SearchUsers().Redirect(this);
    }

    private void RefreshUserDetails()
    {
        newPerson = ctrlPD.GetPersonDetails();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        List<PersonRegionAccessDetail> allRegions = (!string.IsNullOrEmpty(listRegionAccessDetails.Value)) ? serializer.Deserialize<List<PersonRegionAccessDetail>>(listRegionAccessDetails.Value) : new List<PersonRegionAccessDetail>();
        List<SiteAccessDetail> allSites = (!string.IsNullOrEmpty(listAllSitesDetails.Value)) ? serializer.Deserialize<List<SiteAccessDetail>>(listAllSitesDetails.Value) : new List<SiteAccessDetail>();
        List<PersonType> allGroups = (!string.IsNullOrEmpty(listGroupsDetails.Value)) ? serializer.Deserialize<List<PersonType>>(listGroupsDetails.Value) : new List<PersonType>();
        ctrlUD.DisplayUserDetails(newPerson, allSites, allRegions, allGroups);
    }

    private void CheckButtons()
    {
        if (tabStripSteps.SelectedIndex == 3)
        {
            RefreshUserDetails();
            btnCreate.Visible = true;

            HtmlButton buttonBack = btnBack;
            buttonBack.Attributes.Remove("class");
            buttonBack.Attributes.Add("class", "btn");

            HtmlButton buttonNext = btnNext;
            buttonNext.Attributes.Remove("class");
            buttonNext.Attributes.Add("class", "btn hideElement");
        }
    }

    protected void UpsertRegions(Person person)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        List<PersonRegionAccessDetail> allRegions = serializer.Deserialize<List<PersonRegionAccessDetail>>(listRegionAccessDetails.Value);
        if (allRegions != null)
        {
            foreach (PersonRegionAccessDetail item in allRegions)
            {
                Region tempRegion = SiteManager.GetRegionById(item.LocationId);
                PersonRegion personRegion = PersonManager.GetPersonRegionByPersonAndRegion(person, tempRegion);
                if (personRegion == null)
                {
                    PersonManager.AddPersonToRegion(person.Id, tempRegion.Id, item.IsAdmin, item.IsManager);
                }
            }
        }
    }

    protected void UpsertSites(Person person)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        List<SiteAccessDetail> allSites = serializer.Deserialize<List<SiteAccessDetail>>(listAllSitesDetails.Value);
        if (allSites != null)
        {
            foreach (SiteAccessDetail item in allSites)
            {
                Site tempSite = SiteManager.GetSiteById(item.LocationId);
                PersonSite personSite = PersonManager.GetPersonSiteByPersonAndSite(person, tempSite, true);
                if (personSite == null)
                {
                    PersonManager.AddPersonToSite(person.Id, item.LocationId, item.IsAdmin, item.IsManager, item.IsView, item.IsPrimary);
                }
            }
        }
    }

    protected void AddGroups(Person newPerson)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        IList<PersonType> personTypes = PersonManager.GetAllPersonTypes(false);
        List<PersonType> allGroups = serializer.Deserialize<List<PersonType>>(listGroupsDetails.Value);

        if (allGroups != null)
        {
            foreach (PersonType pType in personTypes)
            {
                if (allGroups.Exists(x => x.Name.Equals(pType.Name)))
                {
                    bool isPersonType = PersonManager.IsPersonInPersonType(newPerson, pType);

                    if (!isPersonType)
                    {
                        PersonManager.AddPersonToPersonType(newPerson.Id, pType.Id);
                    }
                }
            }
        }
    }

    [WebMethod]
    public static bool wmIsAuthenticated()
    {
        return true;
    }
}

