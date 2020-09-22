using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using BusiBlocks.Membership;
using BusiBlocks;
using System.Web.Services;

public partial class Admin_EditGroup : System.Web.UI.Page
{
    public string GroupId { get; set; }
    public static List<string> UserNamesList { get; set; }
    private const string PersonList = "personList";

    protected void Page_Load(object sender, EventArgs e)
    {
        GroupId = Request.QueryString["id"];

        if (!Page.IsPostBack)
        {
            UserNamesList = new List<string>();
            DisplayGroupDetails();
            ViewState[PersonList] = string.Join(",", RegionVisibilityHelper.GetPersonsForUser(Page.User.Identity.Name).Select(x => x.Id).ToArray());
        }
    }

    protected void DisplayGroupDetails()
    {
        PersonType editPersonType = new PersonType();
        editPersonType = PersonManager.GetPersonTypeById(GroupId);

        ctrlManageGroupEdit.DisplayGroupDetails(editPersonType);
        DisplayUsers(editPersonType);
    }

    protected void DisplayUsers(PersonType editPersonType)
    {
        ListBox listUsers = new ListBox();

        IList<Person> persons = RegionVisibilityHelper.GetPersonsForUser(Page.User.Identity.Name);

        foreach (Person person in persons)
        {
            if (PersonManager.IsPersonInPersonType(person, editPersonType) == true)
            {
                User user = MembershipManager.GetUserByPerson(person);
                listUsers.Items.Add(new ListItem(user.Name));
                UserNamesList.Add(user.Name);
            }
        }

        ctrlManageGroupEdit.SetList(listUsers);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        PersonType editPersonType = new PersonType();
        editPersonType = PersonManager.GetPersonTypeById(GroupId);

        PersonType tempPersonType = ctrlManageGroupEdit.GetGroupDetails();

        editPersonType.Name = tempPersonType.Name;
        editPersonType.Description = tempPersonType.Description;
        
        PersonManager.UpdatePersonType(editPersonType);
    
        UserNamesList.Clear();
        List<string> lstUsers = ctrlManageGroupEdit.GetList();
        foreach (string item in lstUsers)
        {
            UserNamesList.Add(item);
        }
        
        object personListObj = ViewState[PersonList];

        if (personListObj != null)
        {
            string[] personList = personListObj.ToString().Split(',');
            foreach (string personId in personList)
            {
                Person editPerson = PersonManager.GetPersonById(personId);
                User user = MembershipManager.GetUserByPerson(PersonManager.GetPersonById(personId));

                if (UserNamesList.Contains(user.Name))
                {
                    bool isPersonType = PersonManager.IsPersonInPersonType(editPerson, editPersonType);
                    if (!isPersonType)
                    {
                        PersonManager.AddPersonToPersonType(editPerson.Id, editPersonType.Id);
                    }
                }
                else
                {
                    bool isPersonType = PersonManager.IsPersonInPersonType(editPerson, editPersonType);
                    if (isPersonType)
                    {
                        PersonManager.DeletePersonFromPersonType(editPerson.Id, editPersonType.Id);
                    }
                }
            }
        }
        string groupnameLink = "<a href=../Admin/EditGroup.aspx?id=" + editPersonType.Id + ">" + editPersonType.Name + "</a>";
        Feedback feedBack = new Feedback(BusiBlocksConstants.Blocks.Administration.BlockName, "Group", Feedback.Actions.Saved, groupnameLink);
        Session["feedback"] = feedBack;

        Navigation.Admin_ManageRolesGroups().Redirect(this);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageRolesGroups().Redirect(this);
    }

    [WebMethod]
    public static void wmAddUserToList(string UserName)
    {
        UserNamesList.Add(UserName);
    }
}