using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.PersonLayer;
using System.Web.UI.HtmlControls;

public partial class Controls_GroupDetails : System.Web.UI.UserControl
{
    private string Id { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            Id = Request.QueryString["id"];

        PopulatePersonTypeTable();

        if (!Page.IsPostBack && !string.IsNullOrEmpty(Id))
        {
            PopulateUserGroups(Id);
        }
    }

    protected void PopulatePersonTypeTable()
    {
        IList<PersonType> personTypes = PersonManager.GetAllPersonTypes(false);

        foreach (PersonType item in personTypes)
        {
            TableRow row = new TableRow();

            TableCell cellCheckBox = new TableCell();
            HtmlInputCheckBox personTypeCheck = new HtmlInputCheckBox();

            string pageType = (string.IsNullOrEmpty(Id) == true) ? "Create" : "Edit";

            personTypeCheck.Attributes.Add("id", item.Id);
            personTypeCheck.Attributes.Add("onclick", "javascript:setPersonTypes(this,'" + item.Name + "','" + pageType + "');");
            personTypeCheck.Attributes.Add("value", item.Name);
            cellCheckBox.CssClass = "center";
            cellCheckBox.Controls.Add(personTypeCheck);

            TableCell cellGroupName = new TableCell();
            Label lblGroupName = new Label();
            lblGroupName.ID = "lbl" + item.Id;
            lblGroupName.Text = item.Name;
            cellGroupName.Controls.Add(lblGroupName);

            row.Controls.Add(cellCheckBox);
            row.Controls.Add(cellGroupName);

            tablePersonType.Rows.Add(row);
        }
    }

    public void PopulateUserGroups(string id)
    {
        Person queryPerson = PersonManager.GetAllPersons().First<Person>(x => x.Id.Equals(id));

        List<string> UserGroups = new List<string>();
        IList<PersonType> personTypes = PersonManager.GetPersonTypesByPerson(queryPerson);
        foreach (PersonType item in personTypes)
        {
            UserGroups.Add(item.Name);
        }

        foreach (TableRow row in tablePersonType.Rows)
        {
            foreach (TableCell control in row.Controls)
            {
                foreach (Control ctrl in control.Controls)
                {
                    if (ctrl is HtmlInputCheckBox)
                    {
                        HtmlInputCheckBox cbx = (HtmlInputCheckBox)ctrl;

                        if (UserGroups.Contains(cbx.Value))
                            cbx.Checked = true;
                    }
                }
            }
        }
    }
}