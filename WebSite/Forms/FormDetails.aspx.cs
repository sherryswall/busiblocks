using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FormDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string formId = Request["form"];

            BusiBlocks.FormsBlock.FormDefinition formDef = null;

            //Edit
            if (formId != null)
            {
                formDef = BusiBlocks.FormsBlock.FormsManager.GetFormDefinition(formId);

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

                formDef = BusiBlocks.FormsBlock.FormsManager.GetFormDefinition("1346598346923345");
            }

            this.lblFormDefTitle.InnerText = formDef.Name;

            // Locate all the form properties for this form, and create text boxes for each one.
            IList<BusiBlocks.FormsBlock.FormProperty> allProperties = BusiBlocks.FormsBlock.FormsManager.GetAllFormProperties(formDef);
            formPropertyList.DataSource = allProperties;
            formPropertyList.DataBind();
        }
    }

    protected void btSave_Click(object sender, EventArgs e)
    {
        // Create a form instance (with form property instances)
        try
        {
            string formId = Request["form"];

            //BusiBlocks.FormsBlock.FormInstance formInstance;

            ////Edit
            //if (forumId != null)
            //{
            //    forum = BusiBlocks.CommsBlock.Forums.CommsManager.GetCategory(forumId);

            //    forum.DisplayName = txtDisplayName.Text;
            //}
            //else //New
            //{
            //    forum = BusiBlocks.CommsBlock.Forums.CommsManager.CreateCategory(txtName.Text, txtDisplayName.Text);
            //}


            // THIS PART IS DOESN't WORK
            foreach (RepeaterItem ri in formPropertyList.Items)
            {
                
            }

            //BusiBlocks.FormsBlock.FormsManager.CreateFormInstance(formDefinition, "blah");


            Navigation.Admin_FormList().Redirect(this);

        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Navigation.Admin_FormList().Redirect(this);
    }
}