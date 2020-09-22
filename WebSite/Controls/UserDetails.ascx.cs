using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using BusiBlocks;
using BusiBlocks.SiteLayer;
using Microsoft.JScript;

public partial class Controls_UserDetails : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public void DisplayUserDetails(Person person, List<SiteAccessDetail> listSiteAccess, List<PersonRegionAccessDetail> listRegionAccess, List<PersonType> listGroupDetails)
    {
        if (person == null)
            throw new ArgumentNullException("person");
      
        lblFirstName.Text = person.FirstName;
        lblLastName.Text = person.LastName;
        lblPosition.Text = person.Position;
        lblWorkPhone.Text = person.WorkPhone;
        lblWorkFax.Text = person.WorkFax;
        lblWorkEmail.Text = person.Email;
        lblWorkMobile.Text = person.WorkMobile;
        lblUserId.Text = person.Id;
        lblSites.Text = string.Empty;
        lblRegions.Text = string.Empty;
        lblIsAdmin.Text = string.Empty;
        lblGroups.Text = string.Empty;

        if (listSiteAccess != null)
        {
            foreach (SiteAccessDetail item in listSiteAccess)
            {
                lblSites.Text +=GlobalObject.unescape(item.Name) + ", ";
                if (item.IsAdmin)
                {
                    lblIsAdmin.Text += GlobalObject.unescape(item.Name) + ", ";
                }
            }
        }
        if (listRegionAccess != null)
        {
            foreach (PersonRegionAccessDetail item in listRegionAccess)
            {
                lblRegions.Text += GlobalObject.unescape(item.Name) + ", ";
            }
        }
        if (listGroupDetails != null)
        {
            foreach (PersonType item in listGroupDetails)
            {
                lblGroups.Text += item.Name + ", ";
            }
        }

        char[] trims = new char[] { ',', ' ' };
        lblSites.Text = lblSites.Text.TrimEnd(trims);
        lblRegions.Text = lblRegions.Text.TrimEnd(trims);
        lblGroups.Text = lblGroups.Text.TrimEnd(trims);
        lblIsAdmin.Text = lblIsAdmin.Text.TrimEnd(trims);
    }
}