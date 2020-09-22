using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Controls_ModalPopup : UserControl, INamingContainer
{
    public override string ID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }


    [PersistenceMode(PersistenceMode.InnerProperty),
    TemplateInstance(TemplateInstance.Single),
    TemplateContainer(typeof(FormTemplateContainer))]
    public ITemplate FormTemplateContainer { get; set; }

    public Control MyTemplateContainer { get; private set; }

    private class FormTemplateContainerClass : Control, INamingContainer
    {
        private Controls_ModalPopup parent;
        public FormTemplateContainerClass(Controls_ModalPopup parent)
        {
            this.parent = parent;
        }
    }

    public string InputName { get; set; }
    public string InputMaxLength { get; set; }
    public string AcceptButtonText { get; set; }
    public string CancelButtonText { get; set; }
    public string ReferrerId
    {
        get { return refId.Value; }
        set { refId.Value = value; }
    }

    public string Value
    {
        get
        {
            TextBox textbox = ((TextBox)divInput.FindControl(TextBoxPrefix + InputName.Replace(" ", string.Empty)));
            return (textbox == null ? string.Empty : textbox.Text);
        }
    }

    public string OnClientAcceptClick { get; set; }

    public event EventHandler AcceptClick;

    public Unit Width { get; set; }
    public Unit Height { get; set; }
    
    private const string LiteralContentId = "litContent";
    private const string LiteralTitleId = "litTitle";
    private const string ButtonAcceptId = "btnAccept";
    private const string ButtonCancelId = "btnCancel";
    private const string ButtonClass = "btn";

    private const string LabelPrefix = "lbl";
    private const string TextBoxPrefix = "txt";

    private const string JavascriptPrefix = "javascript:";
    private const string JavascriptHide = ".Hide();";
    private const string JavascriptDontPostback = "return false;";
     
    protected void Page_Load(object sender, EventArgs e)
    {
        Init();
    }

    private void Init()
    {
        InitRadWindow();
        InitTitle();
        InitContent();
        InitCustomContent();
        InitInput();
        InitButtons();
    }



    private void InitRadWindow()
    {
        if (!Width.IsEmpty)
            DialogWindow.Width = DialogWindow.MinWidth = Width;

        if (!Height.IsEmpty)
            DialogWindow.Height = DialogWindow.MinHeight = Height;
    }

    private void InitContent()
    {
        if (!string.IsNullOrEmpty(Content))
            divCustom.Controls.Add(new Literal { Text = Content, ID = LiteralContentId });
    }


    private void InitCustomContent()
    {
        if (FormTemplateContainer != null && MyTemplateContainer == null)
        {
            MyTemplateContainer = new FormTemplateContainer(plhCustomContent);
            FormTemplateContainer.InstantiateIn(MyTemplateContainer);
            plhCustomContent.Controls.Add(MyTemplateContainer);
        }
    }


    private void InitTitle()
    {
        if (!string.IsNullOrEmpty(Title))
            divTitle.Controls.Add(new Literal { Text = Title, ID = LiteralTitleId });
    }

    public Control GetCustomContent(string Id)
    {
        InitCustomContent();
        Control control = Utilities.FindControlRecursive(MyTemplateContainer, Id);
        return control;
    }
   
    public ControlCollection GetContent()
    {
        return plhCustomContent.Controls;
    }

    private void InitInput()
    {
        if (!string.IsNullOrEmpty(InputName))
        {
            divInput.Controls.Add(new Label { ID = LabelPrefix + InputName.Replace(" ", string.Empty), Text = String.Format("<span>{0}:</span>", InputName) });
            
            TextBox tb = new TextBox { ID = TextBoxPrefix + InputName.Replace(" ", string.Empty) };

            if (!string.IsNullOrEmpty(InputMaxLength))
            {
                int tbMaxLength;
                if (Int32.TryParse(InputMaxLength, out tbMaxLength))
                    tb.MaxLength = tbMaxLength;
            }

            divInput.Controls.Add(tb);
        }
    }


    private void InitButtons()
    {
        if (!string.IsNullOrEmpty(AcceptButtonText))
            InitAcceptButton();
        
        if (!string.IsNullOrEmpty(CancelButtonText))
            InitCancelButtons();
    }
    
    private void InitAcceptButton()
    {
        string javascript = JavascriptPrefix;

        if (!string.IsNullOrEmpty(OnClientAcceptClick))
            javascript += OnClientAcceptClick.Replace(JavascriptPrefix, string.Empty) + ";";

        javascript += ID + JavascriptHide;

        if (AcceptClick == null)
            javascript += JavascriptDontPostback;

        Button acceptButton = new Button
        {
            Text = AcceptButtonText,
            ID = ButtonAcceptId,
            CssClass = ButtonClass,
            OnClientClick = javascript
        };

        if (AcceptClick != null)
            acceptButton.Click += AcceptButton_Click;

        divButtons.Controls.Add(acceptButton);
    }
    
    private void InitCancelButtons()
    { 
        divButtons.Controls.Add(new Button
        {
            Text = CancelButtonText,
            ID = ButtonCancelId,
            CssClass = ButtonClass,
            OnClientClick = JavascriptPrefix + ID + JavascriptHide + JavascriptDontPostback
        });
    }
    
    protected void AcceptButton_Click(object sender, EventArgs e)
    {
        AcceptClick.Invoke(this, new EventArgs());
    }
}

