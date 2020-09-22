using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FormDefinitions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            string formId = Request["id"];

            //Edit
            if (formId != null)
            {
                BusiBlocks.FormsBlock.FormDefinition formDef = BusiBlocks.FormsBlock.FormsManager.GetFormDefinition(formId);

                //txtName.Enabled = false;

                //txtName.Text = forum.Name;
                //txtDisplayName.Text = forum.DisplayName;
                //txtEditPermissions.Text = forum.EditPermissions;
                //txtReadPermissions.Text = forum.ReadPermissions;
                //txtDeletePermissions.Text = forum.DeletePermissions;
                //txtInsertPermissions.Text = forum.InsertPermissions;
                //txtDescription.Text = forum.Description;

                //chkEnabledAttach.Checked = forum.AttachEnabled;
                //txtAttachExtensions.Text = forum.AttachExtensions;
                //txtAttachMaxSize.Text = forum.AttachMaxSize.ToString();
                ////LoadGroupList(forum.Groups);
            }
            else //New
            {
                // Load a form definition, or select from a list of form definitions.
                // For now, just load a form definition.

                IList<BusiBlocks.FormsBlock.FormDefinition> formDefinitions = BusiBlocks.FormsBlock.FormsManager.GetAllFormDefinitions();

                // Locate all the form properties for this form, and create text boxes for each one.
                formDefinitionList.DataSource = formDefinitions;
                formDefinitionList.DataBind();
            }
        }
    }
    
    protected string GetFormLink(string name)
    {
        return Navigation.Form_ViewForm(name).GetClientUrl(this, true);
    }
}