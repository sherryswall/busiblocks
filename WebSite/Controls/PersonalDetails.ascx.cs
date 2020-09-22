using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using BusiBlocks.AddressLayer;
using BusiBlocks.Membership;
using System.Web.Security;

public partial class Controls_PersonalDetails : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public Person GetPersonDetails()
    {
        if (string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            return new Person
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Email = txtWorkEmail.Text,
                Address = new Address(),
                Id = GenerateLogonId(txtFirstName.Text, txtLastName.Text),
                WorkFax = txtWorkFax.Text,
                WorkMobile = txtWorkMobile.Text,
                WorkPhone = txtWorkPhone.Text,
                Position = txtPosition.Text
            };
        }
        else
        {
            return new Person
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Email = txtWorkEmail.Text,
                Id = Request.QueryString["id"].ToString(),
                WorkFax = txtWorkFax.Text,
                WorkMobile = txtWorkMobile.Text,
                WorkPhone = txtWorkPhone.Text,
                Position = txtPosition.Text
            };
        }
    }

    protected string GenerateLogonId(string firstName, string lastName)
    {
        string logonId = string.Empty;

        // Check the txtUserId field first. If not null, then don't generate a new one.
        if (string.IsNullOrEmpty(txtUserId.Text))
        {
            // Auto generate a userid.
            logonId = firstName.Replace(" ", string.Empty) + lastName.Replace(" ", string.Empty);
            if (logonId.Length > 18)
                logonId = logonId.Substring(0, 18);
        }
        else
            logonId = txtUserId.Text;

        int iterator = 1;
        string originalLogonId = logonId;
        const int circuitBreaker = 100;
        while (MembershipManager.GetUserByName(logonId) != null && circuitBreaker > iterator)
        {
            logonId = originalLogonId + iterator.ToString();
            ++iterator;
        }
        return logonId;
    }

    public void DisplayPersonDetails(Person person)
    {
        User user1 = MembershipManager.GetUserByPerson(person);
        MembershipUser membershipUser = Membership.GetUser(user1.Name);

        txtFirstName.Text = person.FirstName;
        txtLastName.Text = person.LastName;
        txtPosition.Text = person.Position;
        txtWorkEmail.Text = person.Email;
        txtWorkFax.Text = person.WorkFax;
        txtWorkMobile.Text = person.WorkMobile;
        txtWorkPhone.Text = person.WorkPhone;
        txtUserId.Text = membershipUser.UserName;
    }

    public string GetPassword()
    {
        return txtPassword.Text;
    }
    public string GetUserId()
    {
        return txtUserId.Text;
    }
}