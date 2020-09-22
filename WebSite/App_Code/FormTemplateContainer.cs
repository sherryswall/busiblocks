using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;



public class FormTemplateContainer : Control, INamingContainer
{
    private PlaceHolder parent;
    public FormTemplateContainer(PlaceHolder parent)
    {
        this.parent = parent;
    }
}
