using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BusiBlocks.SchemaGenerator;
using BusiBlocks.PersonLayer;
using BusiBlocks.Membership;
using BusiBlocks.Roles;
using BusiBlocks.DocoBlock;
using BusiBlocks.AddressLayer;

public partial class Setup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Request.IsLocal)
            throw new BusiBlocks.InvalidPermissionException("run setup");

        if (!IsPostBack)
        {
            LoadConnections();
            LoadListAdvanced(false);
        }
    }

    private void LoadConnections()
    {
        cmbConnections.Items.Clear();
        foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
        {
            ListItem item = new ListItem();
            item.Text = connection.Name;
            item.Value = connection.Name;

            cmbConnections.Items.Add(item);
        }
    }

    private void LoadListAdvanced(bool checkStatus)
    {
        list.Items.Clear();

        BusiBlocks.ConnectionParameters connection = BusiBlocks.ConnectionParameters.Create(cmbConnections.SelectedValue);

        GenericGenerator generator = new GenericGenerator(connection);

        foreach (string section in generator.GetSchemaCategories())
        {
            ListItem item = new ListItem();
            item.Text = section.ToString();
            if (checkStatus)
                item.Text += " - " + generator.GetStatus(section).ToString();
            item.Value = section;

            list.Items.Add(item);
        }
    }


    protected void btCheckStatus_Click(object sender, EventArgs e)
    {
        LoadListAdvanced(true);
    }

    protected void btCreate_Click(object sender, EventArgs e)
    {
        BusiBlocks.ConnectionParameters connection = BusiBlocks.ConnectionParameters.Create(cmbConnections.SelectedValue);

        GenericGenerator generator = new GenericGenerator(connection);

        foreach (ListItem item in list.Items)
        {
            if (item.Selected)
            {
                generator.CreateSchemaTable(item.Value);
            }
        }

        LoadListAdvanced(true);

        lblStatus.InnerText = "Schema created!";
    }

    protected void cmbConnections_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadListAdvanced(false);
    }

    protected void btCreateAdmin_Click(object sender, EventArgs e)
    {
        // Create a person type, and a role, and a persontyperole.
        PersonType adminPersonType = new PersonType();
        adminPersonType.Name = BusiBlocks.BusiBlocksConstants.AdministratorsGroup;
        var personTypes = PersonManager.GetAllPersonTypes(true).FirstOrDefault(x => x.Name.Equals(adminPersonType.Name));
        if (personTypes == null)
            PersonManager.CreatePersonType(adminPersonType);
        adminPersonType = PersonManager.GetAllPersonTypes(true).FirstOrDefault(x => x.Name.Equals(adminPersonType.Name));
        Role role = RoleManager.GetRoleByName(txtAdminRole.Text);
        if (role == null)
            Roles.CreateRole(txtAdminRole.Text);
        PersonTypeRole personTypeRole = new PersonTypeRole();
        personTypeRole.PersonType = adminPersonType;
        Role role1 = RoleManager.GetRoleByName(txtAdminRole.Text);
        personTypeRole.Role = role1;
        var personTypeRole1 = PersonManager.GetAllPersonTypeRoles().FirstOrDefault(x => (x.Role.Name.Equals(role1.Name) && x.PersonType.Name.Equals(personTypeRole.PersonType.Name)));
        if (personTypeRole1 == null)
            PersonManager.CreatePersonTypeRole(personTypeRole);

        string pwd = txtAdminPassword.Text;

        MembershipUser user2 = Membership.GetUser(txtAdminUser.Text);
        if (user2 == null)
            Membership.CreateUser(txtAdminUser.Text, pwd, txtAdminEMail.Text);
        MembershipUserCollection col = Membership.FindUsersByName(txtAdminUser.Text);
        MembershipUser memUser = col[txtAdminUser.Text];
        User user = MembershipManager.GetUser(memUser.ProviderUserKey.ToString());

        var roles1 = RoleManager.GetRolesByUser(user).FirstOrDefault(x => x.Name.Equals(role1.Name));
        if (roles1 == null)
            RoleManager.AddUserToRole(user, role1);

        // Find a Person who matches this User.
        // If none exists, create one.
        string userName = txtAdminUser.Text;
        Person person = PersonManager.GetPersonByUserName(userName);
        if (person == null)
        {
            Address address1 = new Address();
            address1.Address1 = "1 Admin Drive";
            AddressManager.CreateAddress(address1);
            User user1 = MembershipManager.GetUserByName(userName);
            PersonManager.CreatePerson(user1, userName, address1);
            person = PersonManager.GetPersonByUserId(user1.Id);
        }
        var personType1 = PersonManager.GetPersonTypesByPerson(person).FirstOrDefault(x => x.Name.Equals(adminPersonType.Name));
        if (personType1 == null)
            PersonManager.AddPersonToPersonType(person.Id, adminPersonType.Id);

        lblStatus.InnerText = "User created!";
    }
}
