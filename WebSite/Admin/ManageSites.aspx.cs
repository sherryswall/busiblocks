using System;
using System.Collections.Generic;
using System.Linq;
using BusiBlocks.SiteLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks.AddressLayer;
using BusiBlocks;

public partial class Admin_ManageSites : System.Web.UI.Page
{
    IList<Region> regions;

    private const string CreateSite = "Create Site";
    private const string UpdateSite = "Update Site";
    private const string ManageSite = "Edit Site";
    //private const string SiteId = "SiteId";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Find regions/sites which this user is allowed to see.
        Person person = PersonManager.GetPersonByUserName(Page.User.Identity.Name);
        if (PersonManager.IsPersonInPersonTypeAdmin(person))
            regions = SiteManager.GetAllRegions();
        else 
            regions = PersonManager.GetAdminRegionsByPerson(person, true);

        string currentSiteId = Request["id"];
        if (string.IsNullOrEmpty(currentSiteId))
        {
            if (!string.IsNullOrEmpty(lblSiteId.Text))
            {
                currentSiteId = lblSiteId.Text;
            }
        }

        lblPageTitle.Text = ManageSite;

        if (!string.IsNullOrEmpty(currentSiteId))
        {
            Site currentSite1 = SiteManager.GetSiteById(currentSiteId);
            if (regions.Count == 0)
            {
                regions.Add(currentSite1.Region);
                cmbRegions.DataSource = regions;
                cmbRegions.DataTextField = "Breadcrumb";
                cmbRegions.DataBind();
            }
        }

        if (!IsPostBack)
        {
            //ViewState["RefUrl"] = Request.UrlReferrer.ToString();

            lblSiteId.Text = currentSiteId;

            cmbRegions.DataSource = regions;
            cmbRegions.DataTextField = "Breadcrumb";
            cmbRegions.DataBind();

            string parentRegionId = Request["regionId"];
            if (!string.IsNullOrEmpty(parentRegionId))
            {
                // Fill the region field.
                var index = from x in regions
                            where x.Id.Equals(parentRegionId)
                            select x;
                Region rIndex = index.FirstOrDefault();
                if (rIndex != null)
                {
                    cmbRegions.SelectedIndex = regions.IndexOf(rIndex);
                    btnUpdate.Text = CreateSite;
                }
            }

            //Edit
            if (!string.IsNullOrEmpty(currentSiteId))
            {
                RefreshData(currentSiteId);
            }
            else
            {
                lblPageTitle.Text = CreateSite;
            }
            txtSiteName.Focus();
        }
    }

    private void RefreshData(string siteId)
    {
        Site currentSite = SiteManager.GetSiteById(siteId);
        txtSiteName.Text = currentSite.Name;

        // If the user is only allowed to view sites, then the regions
        // list will be empty. So add just the region which the site
        // is part off.
        if (regions.Count == 0)
        {
            regions.Add(currentSite.Region);
            cmbRegions.DataSource = regions;
            cmbRegions.DataTextField = "Breadcrumb";
            cmbRegions.DataBind();
        }

        var regionPos =
            from x in regions
            where x.Id.Equals(currentSite.Region.Id)
            select regions.IndexOf(x);

        cmbRegions.SelectedIndex = regionPos.First();

        txtPhoneNumber.Text = currentSite.PhoneNumber;
        txtAltPhoneNumber.Text = currentSite.AltPhoneNumber;
        txtEmail.Text = currentSite.Email;

        if (currentSite.PostalAddress != null)
        {
            txtPostalAddress1.Text = currentSite.PostalAddress.Address1;
            txtPostalAddress2.Text = currentSite.PostalAddress.Address2;
            txtPostalSuburb.Text = currentSite.PostalAddress.Suburb;
            txtPostalPostcode.Text = currentSite.PostalAddress.Postcode;
            txtPostalState.Text = currentSite.PostalAddress.State;
        }

        if (currentSite.PhysicalAddress != null)
        {
            txtPhysicalAddress1.Text = currentSite.PhysicalAddress.Address1;
            txtPhysicalAddress2.Text = currentSite.PhysicalAddress.Address2;
            txtPhysicalSuburb.Text = currentSite.PhysicalAddress.Suburb;
            txtPhysicalPostcode.Text = currentSite.PhysicalAddress.Postcode;
            txtPhysicalState.Text = currentSite.PhysicalAddress.State;
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            bool nameAvailable = true;
            Site currentSite = null;
            if (!string.IsNullOrEmpty(lblSiteId.Text))
                currentSite = SiteManager.GetSiteById(lblSiteId.Text);

            Site site = SiteManager.GetSiteByName(txtSiteName.Text);
            if (site != null)
            {

                if (currentSite != null)
                {
                    if (site != null && site.Id != currentSite.Id)
                    {
                        nameAvailable = false;
                        ((IFeedback)Page.Master).ShowFeedback(Feedback.Actions.Error, "Site " + txtSiteName.Text + " already exists");
                    }

                }
                else //new site
                {
                    nameAvailable = false;
                    ((IFeedback)Page.Master).ShowFeedback(Feedback.Actions.Error, "Region " + txtSiteName.Text + " already exists");
                }
            }

            if (nameAvailable)
            {
                // Figure out the address.
                Address physicalAddress = CreatePhysicalAddressFromFields();
                //if (physicalAddress == null)
                //    physicalAddress = new Address();
                Address postalAddress = CreatePostalAddressFromFields();
                //if (postalAddress == null)
                //    postalAddress = new Address();

                if (currentSite == null)
                {
                    // Create a new site.
                    currentSite = new Site();
                    currentSite.Name = txtSiteName.Text;
                    // Just get the first available site type. We don't use site types at the moment.
                    currentSite.SiteType = SiteManager.GetAllSiteTypes()[0];
                    currentSite.PhoneNumber = txtPhoneNumber.Text;
                    currentSite.AltPhoneNumber = txtAltPhoneNumber.Text;
                    currentSite.Email = txtEmail.Text;
                    currentSite.Region = (Region)regions[cmbRegions.SelectedIndex];
                    currentSite.PhysicalAddress = physicalAddress;
                    currentSite.PostalAddress = postalAddress;
                    SiteManager.CreateSite(currentSite);
                    lblSiteId.Text = SiteManager.GetSiteByName(currentSite.Name).Id;
                    RefreshData(SiteManager.GetSiteByName(currentSite.Name).Id);
                    btnUpdate.Text = UpdateSite;

                    ((IFeedback)Master).QueueFeedback(BusiBlocksConstants.Blocks.Administration.LongName,
                        currentSite.GetType().Name, Feedback.Actions.Created, currentSite.Name);
                    
                    Navigation.Admin_ManageLocations().Redirect(this);
                }
                else
                {
                    if (physicalAddress != null)
                    {
                        if (currentSite.PhysicalAddress == null)
                            currentSite.PhysicalAddress = new Address();

                        currentSite.PhysicalAddress.Address1 = physicalAddress.Address1;
                        currentSite.PhysicalAddress.Address2 = physicalAddress.Address2;
                        currentSite.PhysicalAddress.Suburb = physicalAddress.Suburb;
                        currentSite.PhysicalAddress.Postcode = physicalAddress.Postcode;
                        currentSite.PhysicalAddress.State = physicalAddress.State;

                        if (string.IsNullOrEmpty(currentSite.PhysicalAddress.Id))
                            AddressManager.CreateAddress(currentSite.PhysicalAddress);
                        else
                            AddressManager.UpdateAddress(currentSite.PhysicalAddress);
                    }
                    else if (physicalAddress == null && currentSite.PhysicalAddress != null)
                    {
                        // Delete the address.
                        AddressManager.DeleteAddress(currentSite.PhysicalAddress);
                        currentSite.PhysicalAddress = null;
                    }

                    if (postalAddress != null)
                    {
                        if (currentSite.PostalAddress == null)
                            currentSite.PostalAddress = new Address();

                        currentSite.PostalAddress.Address1 = postalAddress.Address1;
                        currentSite.PostalAddress.Address2 = postalAddress.Address2;
                        currentSite.PostalAddress.Suburb = postalAddress.Suburb;
                        currentSite.PostalAddress.Postcode = postalAddress.Postcode;
                        currentSite.PostalAddress.State = postalAddress.State;

                        if (string.IsNullOrEmpty(currentSite.PostalAddress.Id))
                            AddressManager.CreateAddress(currentSite.PostalAddress);
                        else
                            AddressManager.UpdateAddress(currentSite.PostalAddress);
                    }
                    else if (postalAddress == null && currentSite.PostalAddress != null)
                    {
                        // Delete the address.
                        AddressManager.DeleteAddress(currentSite.PostalAddress);
                        currentSite.PostalAddress = null;
                    }

                    currentSite.Name = txtSiteName.Text;
                    currentSite.PhoneNumber = txtPhoneNumber.Text;
                    currentSite.AltPhoneNumber = txtAltPhoneNumber.Text;
                    currentSite.Email = txtEmail.Text;
                    currentSite.Region = (Region)regions[cmbRegions.SelectedIndex];
                    SiteManager.UpdateSite(currentSite);

                    ((IFeedback)Master).QueueFeedback(BusiBlocksConstants.Blocks.Administration.LongName,
                        currentSite.GetType().Name, Feedback.Actions.Saved, currentSite.Name);

                    Navigation.Admin_ManageLocations().Redirect(this);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageLocations().Redirect(this);
    }

    private Address CreatePhysicalAddressFromFields()
    {
        var address = new Address() {
            Address1 = txtPhysicalAddress1.Text,
            Address2 = txtPhysicalAddress2.Text,
            Suburb = txtPhysicalSuburb.Text,
            Postcode = txtPhysicalPostcode.Text,
            State = txtPhysicalState.Text
        };

        if (AddressManager.AddressExists(address))
        {
            Address currentAddress = 
                AddressManager.GetAddressByDetails(address.Address1, address.Address2, address.Suburb, address.Postcode, address.State);
            address.Id = currentAddress.Id;
        }

        if (string.IsNullOrEmpty(address.Address1))
            return null;
        
        return address;
    }

    private Address CreatePostalAddressFromFields()
    {
        var address = new Address() {
            Address1 = txtPostalAddress1.Text,
            Address2 = txtPostalAddress2.Text,
            Suburb = txtPostalSuburb.Text,
            Postcode = txtPostalPostcode.Text,
            State = txtPostalState.Text
        };

        if (AddressManager.AddressExists(address))
        {
            Address currentAddress =
                AddressManager.GetAddressByDetails(address.Address1, address.Address2, address.Suburb, address.Postcode, address.State);
            address.Id = currentAddress.Id;
        }

        if (string.IsNullOrEmpty(address.Address1))
            return null;
        
        return address;
    }
}