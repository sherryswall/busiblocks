using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks;
using BusiBlocks.PersonLayer;
using BusiBlocks.Membership;
using System.Web.Security;
using System.Web.Services;

public partial class Controls_PersonalDetailsView : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request["id"];
        if (!string.IsNullOrEmpty(id))
        {
            RefreshData(id);
        }
    }

    private void RefreshData(string personId)
    {
        Person currentPerson = PersonManager.GetPersonById(personId);

        if (currentPerson != null)
        {
            User user1 = MembershipManager.GetUserByPerson(currentPerson);
            MembershipUser membershipUser = Membership.GetUser(user1.Name);
            if (membershipUser != null) txtUserId.Text = membershipUser.UserName;
            txtFirstName.Text = currentPerson.FirstName;
            txtLastName.Text = currentPerson.LastName;
            txtPosition.Text = currentPerson.Position;
            txtWorkPhone.Text = currentPerson.WorkPhone;
            txtWorkFax.Text = currentPerson.WorkFax;
            txtWorkEmail.Text = currentPerson.Email;
            txtWorkMobile.Text = currentPerson.WorkMobile;
        }
    }    
}