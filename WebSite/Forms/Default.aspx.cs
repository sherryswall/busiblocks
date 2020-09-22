using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Forms_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HtmlLink link = new HtmlLink();
        //link.Href = Navigation.Forum_ForumRss(null).GetServerUrl(true);
        link.Attributes.Add("rel", "alternate");
        link.Attributes.Add("type", "application/rss+xml");
        link.Attributes.Add("title", "Forum News");
        //Header.Controls.Add(link);


        if (!IsPostBack)
        {
            lnkAddNew.Visible = BusiBlocks.SecurityHelper.CanAddNewContainer(User, "forms");
            lnkAddNew.HRef = Navigation.Admin_FormDetailsNew().GetServerUrl(true);
            //linkSearch.HRef = Navigation.Forum_Search().GetServerUrl(true);

            LoadList();
        }
    }

    protected string GetFormLink(string name)
    {
        return Navigation.Form_ViewForm(name).GetClientUrl(this, true);
    }

    private void LoadList()
    {
        IList<BusiBlocks.FormsBlock.FormInstance> listComplete = BusiBlocks.FormsBlock.FormsManager.GetAllFormInstances();

        //Create a list with only the readable items
        List<BusiBlocks.FormsBlock.FormInstance> listReadable = new List<BusiBlocks.FormsBlock.FormInstance>();
        foreach (BusiBlocks.FormsBlock.FormInstance item in listComplete)
        {
            //if (BusiBlocks.SecurityHelper.CanRead(User, category, null))
            //    listReadable.Add(category);
        }

        //listRepeater.DataSource = listComplete;
        //listRepeater.DataBind();
    }
}