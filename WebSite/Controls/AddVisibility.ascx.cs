using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using System.Web.Security;
using BusiBlocks.SiteLayer;
using BusiBlocks.PersonLayer;

public partial class Controls_AddVisibility : System.Web.UI.UserControl
{

    public List<Access> AccessList
    {
        get
        {
            object val = ViewState["bbVisiblity"];
            return val == null ? null : (List<Access>)val;
        }
        set { ViewState["bbVisiblity"] = value; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (AccessList == null)
                AccessList = new List<Access>();
            BindLists();
        }
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

        rdoListGroups2.DataSource = PersonManager.GetAllPersonTypes(false);
        rdoListGroups2.DataTextField = "Name";
        rdoListGroups2.DataValueField = "Id";
        rdoListGroups2.DataBind();

        rdoListLocations2.DataSource = SiteManager.GetAllSites();
        rdoListLocations2.DataTextField = "Name";
        rdoListLocations2.DataValueField = "Id";
        rdoListLocations2.DataBind();

        chkListUsers2.DataSource = Membership.GetAllUsers();
        chkListUsers2.DataTextField = "UserName";
        chkListUsers2.DataValueField = "UserName";
        chkListUsers2.DataBind();
    }

    protected void btnAddGroupLocation_Click(object sender, EventArgs e)
    {
        Access v = new Access();

        v.AccessType = BusiBlocks.AccessType.View;

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

    protected void btnAddGroupLocation2_Click(object sender, EventArgs e)
    {
        Access v = new Access();

        v.AccessType = BusiBlocks.AccessType.Edit;

        if (chkAllGroups2.Checked)
            v.AllPersonTypes = true;
        else
            v.PersonTypeId = rdoListGroups2.SelectedValue;

        if (chkAllLocations2.Checked)
            v.AllSites = true;
        else
            v.SiteId = rdoListLocations2.SelectedValue;



        AccessList.Add(v);
        BindSummary();

    }

    protected void btnAddUser_Click(object sender, EventArgs e)
    {

        
        if (chkAllUsers.Checked)
        {
            Access v = new Access();
            v.AccessType = BusiBlocks.AccessType.View;
            v.AllUsers = true;
            AccessList.Add(v);
        }
        else
        {
            foreach (ListItem item in chkListUsers.Items)
            {
                if (item.Selected)
                {
                    Access v = new Access();
                    v.AccessType = BusiBlocks.AccessType.View;
                    v.UserId = item.Value;
                    AccessList.Add(v);
                }
            }
        }

        BindSummary();
    }

    protected void btnAddUser2_Click(object sender, EventArgs e)
    {


        if (chkAllUsers2.Checked)
        {
            Access v = new Access();
            v.AccessType = BusiBlocks.AccessType.Edit;
            v.AllUsers = true;
            AccessList.Add(v);
        }
        else
        {
            foreach (ListItem item in chkListUsers2.Items)
            {
                if (item.Selected)
                {
                    Access v = new Access();
                    v.AccessType = BusiBlocks.AccessType.Edit;
                    v.UserId = item.Value;
                    AccessList.Add(v);
                }
            }
        }

        BindSummary();
    }

    public void BindSummary()
    {
        //bltSummary.Items.Clear();

        

        lstSummary.Items.Clear();

        foreach (Access v in AccessList)
        {
            //check nulls

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
                //bltSummary.Items.Add(string.Format("{0} from {1}", group, location));
                if (v.AccessType == BusiBlocks.AccessType.View)
                    lstSummary.Items.Add(string.Format("{0} from {1}", personType, site));
                else
                    lstSummary2.Items.Add(string.Format("{0} from {1}", personType, site));
            }
            else
            {
                //bltSummary.Items.Add(user);
                if (v.AccessType == BusiBlocks.AccessType.View)
                    lstSummary.Items.Add(user);
                else
                    lstSummary2.Items.Add(user);
            }

        }

        chkAllGroups.Checked = false;
        chkAllLocations.Checked = false;
        chkAllUsers.Checked = false;
        rdoListLocations.Enabled = true;
        rdoListGroups.Enabled = true;
        chkListUsers.Enabled = true;


        BindLists();

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

    protected void chkAllGroups2_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllGroups2.Checked)
        {
            foreach (ListItem item in rdoListGroups.Items)
            {
                item.Selected = false;
            }
        }

        rdoListGroups2.Enabled = !chkAllGroups2.Checked;

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

    protected void chkAllLocations2_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllLocations2.Checked)
        {
            foreach (ListItem item in rdoListLocations2.Items)
            {
                item.Selected = false;
            }
        }

        rdoListLocations2.Enabled = !chkAllLocations2.Checked;
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

    protected void chkAllUsers2_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAllUsers2.Checked)
        {
            foreach (ListItem item in chkListUsers2.Items)
            {
                item.Selected = false;
            }
        }

        chkListUsers2.Enabled = !chkAllUsers2.Checked;
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        AccessList.RemoveAt(lstSummary.SelectedIndex);
        BindSummary();
    }

    protected void btnRemove2_Click(object sender, EventArgs e)
    {
        AccessList.RemoveAt(lstSummary2.SelectedIndex);
        BindSummary();
    }
}