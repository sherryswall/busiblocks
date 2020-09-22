using System;
using System.Web.UI.WebControls;
using BusiBlocks.AccessLayer;
using System.Collections.Generic;

public partial class Controls_NewMessage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public bool EnabledAttach
    {
        get { return sectionAttach1.Visible; }
        set
        {
            sectionAttach1.Visible = value;
            sectionAttach2.Visible = value;
        }
    }

    public List<Access> AccessList
    {
        get { return this.AccessControl1.AccessList; }
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
            var xhtml = new BusiBlocks.XHTMLText();
            xhtml.Load(BusiBlocks.XHTMLText.FromPlainText(txtBody.Text, BusiBlocks.PlainTextMode.XHtmlConversion));

            return xhtml.RenderHTML(); 
        }
    }

    /// <summary>
    /// Content of the attachment. Null if not specified.
    /// </summary>
    public FileUpload AttachmentFile
    {
        get {return fAttachment;}
    }
}
