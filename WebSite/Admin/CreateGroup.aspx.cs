using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using BusiBlocks;
using BusiBlocks.Membership;
using System.Web.Services;

public partial class Admin_CreateGroup : System.Web.UI.Page
{
    public List<string> UserNamesList { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            UserNamesList = new List<string>();
    }
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        PersonType newPersonType = new PersonType();
        newPersonType = ctrlManageGroupCreate.GetGroupDetails();

        UserNamesList = ctrlManageGroupCreate.GetList();

        if (!string.IsNullOrEmpty(newPersonType.Name))
        {
            PersonManager.CreatePersonType(newPersonType);

            foreach (string username in UserNamesList)
            {
                Person person = PersonManager.GetPersonByUserName(username);

                bool isPersonType = PersonManager.IsPersonInPersonType(person, newPersonType);

                if (!isPersonType)
                {
                    PersonManager.AddPersonToPersonType(person.Id, newPersonType.Id);
                }
            }
        }
        //Adding feedback item to session to be displayed on group creation.
        string groupnameLink = "<a href=../Admin/EditGroup.aspx?id=" + newPersonType.Id + ">" + newPersonType.Name + "</a>";
        Feedback feedBack = new Feedback(BusiBlocksConstants.Blocks.Administration.BlockName, "Group", Feedback.Actions.Created, groupnameLink);
        Session["feedback"] = feedBack;

        Navigation.Admin_ManageRolesGroups().Redirect(this);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_ManageRolesGroups().Redirect(this);
    }
}
