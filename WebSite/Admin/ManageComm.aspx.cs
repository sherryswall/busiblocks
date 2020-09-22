using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

public partial class Admin_ManageComm : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            tv.EnableDragAndDrop = true;
        }
    }
}