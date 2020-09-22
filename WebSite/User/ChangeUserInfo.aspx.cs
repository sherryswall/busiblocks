using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using BusiBlocks.AccessLayer;
using BusiBlocks;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

public partial class User_ChangeUserInfo : System.Web.UI.Page
{
    private const string DefaultPage = "~/User/Default.aspx";

    public List<Access> AccessList
    {
        get
        {
            object val = ViewState["bbVisiblity"];
            return val == null ? null : (List<Access>)val;
        }
        set { ViewState["bbVisiblity"] = value; }
    }

    private void BindLists()
    {
        rdoListGroups.DataSource = PersonManager.GetAllPersonTypes(false);
        rdoListGroups.DataTextField = "Name";
        rdoListGroups.DataValueField = "Id";
        rdoListGroups.DataBind();

        rdoListLocations.DataSource = SiteManager.GetAllSites();
        rdoListLocations.DataTextField = "Name";
        rdoListLocations.DataValueField = "Id";
        rdoListLocations.DataBind();

        chkListUsers.DataSource = Membership.GetAllUsers();
        chkListUsers.DataTextField = "UserName";
        chkListUsers.DataValueField = "UserName";
        chkListUsers.DataBind();

        chkListLocations.DataSource = SiteManager.GetAllSites();
        chkListLocations.DataTextField = "Name";
        chkListLocations.DataValueField = "Id";
        chkListLocations.DataBind();

        var myLocations = new List<Site>(SiteManager.GetSitesByUser(Page.User.Identity.Name, true));

        foreach (ListItem item in chkListLocations.Items)
        {
            item.Selected = (myLocations.Exists(delegate(Site l) { return l.Id == item.Value; }));
        }

        AccessList = Profile.ManagedGroups.Accesses;
        BindSummary();
    }

    protected void btnAddGroupLocation_Click(object sender, EventArgs e)
    {
        var v = new Access();

        v.AccessType = AccessType.View;

        if (chkAllGroups.Checked)
            v.AllPersonTypes = true;
        else
            v.PersonTypeId = rdoListGroups.SelectedValue;

        if (chkAllLocations.Checked)
            v.AllSites = true;
        else
            v.SiteId = rdoListLocations.SelectedValue;

        AccessList.Add(v);
        BindSummary();
    }

    protected void btnAddUser_Click(object sender, EventArgs e)
    {
        if (chkAllUsers.Checked)
        {
            var v = new Access();
            v.AccessType = AccessType.View;
            v.AllUsers = true;
            AccessList.Add(v);
        }
        else
        {
            foreach (ListItem item in chkListUsers.Items)
            {
                if (item.Selected)
                {
                    var v = new Access();
                    v.AccessType = AccessType.View;
                    v.UserId = item.Value;
                    AccessList.Add(v);
                }
            }
        }

        BindSummary();
    }

    public void BindSummary()
    {
        lstSummary.Items.Clear();

        foreach (Access v in AccessList)
        {
            string personType = string.Empty;
            string site = string.Empty;
            string user = string.Empty;

            if (v.AllPersonTypes)
                personType = "All Groups";
            else
            {
                if (v.PersonTypeId != null)
                    personType = PersonManager.GetPersonTypeById(v.PersonTypeId).Name;
            }

            if (v.AllSites)
                site = "all Locations";
            else
            {
                if (v.SiteId != null)
                    site = SiteManager.GetSiteById(v.SiteId).Name;
            }

            if (v.AllUsers)
                user = "All Users";
            else
            {
                if (v.UserId != null)
                    user = v.UserId;
            }

            if (personType.Length > 0)
            {
                lstSummary.Items.Add(string.Format("{0} from {1}", personType, site));
            }
            else
            {
                lstSummary.Items.Add(user);
            }
        }

        chkAllGroups.Checked = false;
        chkAllLocations.Checked = false;
        chkAllUsers.Checked = false;
        rdoListLocations.Enabled = true;
        rdoListGroups.Enabled = true;
        chkListUsers.Enabled = true;
    }

    protected void chkAllGroups_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllGroups.Checked)
        {
            foreach (ListItem item in rdoListGroups.Items)
            {
                item.Selected = false;
            }
        }

        rdoListGroups.Enabled = !chkAllGroups.Checked;
    }

    protected void chkAllLocations_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllLocations.Checked)
        {
            foreach (ListItem item in rdoListLocations.Items)
            {
                item.Selected = false;
            }
        }

        rdoListLocations.Enabled = !chkAllLocations.Checked;
    }

    protected void chkAllUsers_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllUsers.Checked)
        {
            foreach (ListItem item in chkListUsers.Items)
            {
                item.Selected = false;
            }
        }

        chkListUsers.Enabled = !chkAllUsers.Checked;
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        AccessList.RemoveAt(lstSummary.SelectedIndex);
        BindSummary();
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {            
            MembershipUser user = Membership.GetUser((object)Request["id"]);
            if (user == null)
                throw new ApplicationException("User not found " + User.Identity.Name);

            txtEMail.Text = user.Email;

            ProfileCommon profile = Profile.GetProfile(user.UserName);

            chkReceiveNotification.Checked = profile.ReceiveNotification;

            ddlTitle.SelectedValue = profile.Title;
            txtLastName.Text = profile.LastName;
            txtOtherNames.Text = profile.OtherName;
            txtPreferredName.Text = profile.PreferedName;
            txtDateOfBirth.Text = profile.DoB.ToString("dd/MM/yyyy");
            txtPostalAddress.Text = profile.PostalAddress;
            txtPostalCity.Text = profile.PostalCity;
            ddlPostalState.SelectedValue = profile.State;
            txtPostCode.Text = profile.PostCode.ToString();
            txtPhoneBH.Text = profile.PhoneBH;
            txtPhoneAH.Text = profile.PhoneAH;
            txtPhoneMobile.Text = profile.PhoneMobile;
            txtFax.Text = profile.Fax;

            if (AccessList == null)
                AccessList = new List<Access>();
            BindLists();

            //only display for managers
            pnlManageGroups.Visible = (SecurityHelper.IsManager(User));
        }
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        try
        {
            MembershipUser user = Membership.GetUser((object)Request["id"]);

            if (user == null)
                throw new ApplicationException("User not found " + User.Identity.Name);

            user.Email = txtEMail.Text;

            Membership.UpdateUser(user);

            ProfileCommon profile = Profile.GetProfile(user.UserName);

            profile.ReceiveNotification = chkReceiveNotification.Checked;

            profile.Title = ddlTitle.SelectedValue;
            profile.LastName = txtLastName.Text;
            profile.OtherName = txtOtherNames.Text;
            profile.PreferedName = txtPreferredName.Text;
            profile.DoB = Convert.ToDateTime(txtDateOfBirth.Text);
            profile.PostalAddress = txtPostalAddress.Text;
            profile.PostalCity = txtPostalCity.Text;
            profile.State = ddlPostalState.SelectedValue;
            profile.PostCode = Convert.ToInt32(txtPostCode.Text);
            profile.PhoneBH = txtPhoneBH.Text;
            profile.PhoneAH = txtPhoneAH.Text;
            profile.PhoneMobile = txtPhoneMobile.Text;
            profile.Fax = txtFax.Text;
            profile.ManagedGroups.Accesses = AccessList;

            profile.Save();

            Response.Redirect(DefaultPage);
        }
        catch (Exception ex)
        {
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(DefaultPage);
    }
}
