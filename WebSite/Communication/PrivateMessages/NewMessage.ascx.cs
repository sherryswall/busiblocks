using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BusiBlocks.AccessLayer;
using System.Collections.Generic;

public partial class Communication_NewMessage : System.Web.UI.UserControl
{
    public bool EnabledAttach
    {
        get { return sectionAttach1.Visible; }
        set
        {
            sectionAttach1.Visible = value;
            sectionAttach2.Visible = value;
        }
    }

    public void SetAcceptedExtensions(string extensions)
    {
        lblAcceptedExtensions.InnerText = extensions;
    }

    public void SetMaxAttachSize(int maxSize)
    {
        lblMaxAttachSize.InnerText = maxSize.ToString();
    }

    public string MessageSubject
    {
        get { return txtSubject.Text; }
        set { txtSubject.Text = value; }
    }

    public string MessageBodyHtml
    {
        get 
        {
            BusiBlocks.XHTMLText xhtml = new BusiBlocks.XHTMLText();
            xhtml.Load(BusiBlocks.XHTMLText.FromPlainText(txtBody.Text, BusiBlocks.PlainTextMode.XHtmlConversion));

            return xhtml.RenderHTML(); 
        }
    }


    public FileUpload AttachmentFile
    {
        get {return fAttachment;}
    }


}
