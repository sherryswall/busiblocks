using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using System.Web.Security;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;
using System.Windows.Forms;
using BusiBlocks;
using System.Web.UI.HtmlControls;

public partial class Controls_AccessControl : System.Web.UI.UserControl
{
    //APPROVING HAS BEEN COMMENTED OUT FOR SPRINT 2 (RESSLEEP) 11/10/2011. UN-COMMENT FOR VERSIONS AFTER GIVEN TO RESSLEEP.

    #region Constants
    private const string AllGroupsLabel = "All Groups";
    private const string AllLocationsLabel = "All Sites";

    private const string AllIndex = "0";

    private const int GroupIndex = 0;
    private const int LocationIndex = 1;
    #endregion

    public string CategoryId
    {
        set
        {
            BindViewSummaryLists(value);
        }
    }

    public List<Access> AccessList
    {
        get
        {
            List<Access> accessList = new List<Access>();
            accessList.AddRange(GetAccess(hidAccessViewing, AccessType.View));
            accessList.AddRange(GetAccess(hidAccessEditing, AccessType.Edit));
            accessList.AddRange(GetAccess(hidAccessContributing, AccessType.Contribute));

            return accessList;
        }
    }

    private IList<Site> Sites;
    private IList<PersonType> PersonTypes;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ReloadLists();
        }
    }

    public void ReloadLists()
    {
        Sites = SiteManager.GetAllSites().OrderBy(x => x.Name).ToList<Site>();
        PersonTypes = PersonManager.GetAllPersonTypes(false).OrderBy(x => x.Name).ToList<PersonType>();
        BindLists();
    }
    
    private void BindLists()
    {
        BindGroupList(lstGroupsViewing);
        BindLocationList(lstLocationsViewing);

        BindGroupList(lstGroupsEditing);
        BindLocationList(lstLocationsEditing);

        BindGroupList(lstGroupsContributing);
        BindLocationList(lstLocationsContributing);
    }

    private void BindGroupList(System.Web.UI.WebControls.ListBox groupListBox)
    {
        groupListBox.DataSource = PersonTypes;
        groupListBox.DataTextField = "Name";
        groupListBox.DataValueField = "Id";
        groupListBox.DataBind();

        groupListBox.Items.Insert(0, new ListItem(AllGroupsLabel, AllIndex));
        groupListBox.Items.Insert(1, new ListItem("", "-1"));
    }

    private void BindLocationList(System.Web.UI.WebControls.ListBox locationListBox)
    {
        locationListBox.DataSource = Sites;
        locationListBox.DataTextField = "Name";
        locationListBox.DataValueField = "Id";
        locationListBox.DataBind();

        locationListBox.Items.Insert(0, new ListItem(AllLocationsLabel, AllIndex));
        locationListBox.Items.Insert(1, new ListItem("", "-1"));
    }

    public void BindViewSummaryLists(string CategoryId)
    {
        lstSummaryViewing.Items.Clear();
        lstSummaryEditing.Items.Clear();
        lstSummaryApproving.Items.Clear();
        lstSummaryContributing.Items.Clear();

        if (!string.IsNullOrEmpty(CategoryId))
        {
            foreach (Access access in AccessManager.GetItemAccess(CategoryId))
            {
                string personType = string.Empty;
                string site = string.Empty;
                string user = string.Empty;

                if (access.AllPersonTypes)
                    personType = AllGroupsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.PersonTypeId))
                        personType = PersonManager.GetPersonTypeById(access.PersonTypeId).Name;
                }

                if (access.AllSites)
                    site = AllLocationsLabel;
                else
                {
                    if (!string.IsNullOrEmpty(access.SiteId))
                        site = SiteManager.GetSiteById(access.SiteId).Name;
                }

                if (!string.IsNullOrEmpty(personType) && !string.IsNullOrEmpty(site))
                {
                    string id = (access.AllPersonTypes ? "0" : access.PersonTypeId);
                    id += "|" + (access.AllSites ? "0" : access.SiteId);

                    ListItem listItem = new ListItem(string.Format("{0} from {1}", personType, site), id);
                    if (access.AccessType == AccessType.View)
                        lstSummaryViewing.Items.Add(listItem);
                    else if (access.AccessType == AccessType.Edit)
                        lstSummaryEditing.Items.Add(listItem);
                    else if (access.AccessType == AccessType.Contribute)
                        lstSummaryContributing.Items.Add(listItem);
                   // else
                       // lstSummaryApproving.Items.Add(listItem);
                }
            }
        }   
    }

    private List<Access> GetAccess(HtmlInputHidden hidAccess, AccessType accessType)
    {
        List<Access> accessList = new List<Access>();

        foreach (string accessIdPair in hidAccess.Value.Split(','))
        {
            if (!string.IsNullOrEmpty(accessIdPair.Trim()))
            {
                string[] ids = accessIdPair.Split('|');
                Access access = new Access();
                
                if (ids[GroupIndex] == "0")
                    access.AllPersonTypes = true;
                else
                    access.PersonTypeId = ids[GroupIndex];

                if (ids[LocationIndex] == "0")
                    access.AllSites = true;
                else
                    access.SiteId = ids[LocationIndex];

                access.AccessType = accessType;
                accessList.Add(access);
            }
        }
        return accessList;
    }
}