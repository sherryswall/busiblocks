using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using BusiBlocks.CommsBlock.PrivateMessages;
using BusiBlocks.Membership;
using System.Text;
using BusiBlocks.PersonLayer;
using BusiBlocks;


public partial class Communication_DefaultPrivateMessages : System.Web.UI.UserControl
{
    #region Constants

    private const string ErrorIllFormedRecipientString = "Private Message not sent - recipient (To) list was ill formed or entered users don't exist";
    private const string ErrorPermissionDenied = "Permission denied";
    private const string ErrorUnableToOpen = "Unable to open selected private message";
    private const string ErrorSubjectEmpty = "Private message subject is required";
    private const string ErrorBodyEmpty = "Private message content is required";
 
    private const string TargetOpenPrivateMessageId = "OpenPrivateMessage";

    private const string RequestEventTargetId = "__EVENTTARGET";
    private const string RequestEventArgumentId = "__EVENTARGUMENT";
    
    private const string ReplyOrigionalHeading = "-----Original Message-----";
    private const string ReplySubjectPrefix = "RE: ";

    private const string ForwardOrigionalHeading = "-----Forwarded Message-----";
    private const string ForwardSubjectPrefix = "FW: ";

    private const string RecyledPrivateMessage = "\n\n\n{0}\nFrom: {1}\nSent: {2}\nTo: {3}\nSubject: {4}\n\n{5}";

    private const string JavascriptOnMouseOver  = "javascript:MessageHover(true, this);";
    private const string JavascriptOnMouseOut = "javascript:MessageHover(false, this);";
    private const string JavascriptViewMessage = "javascript:ViewMessage(\"{0}\")";
    
    private const string HtmlAttributeOnMouseOver = "onmouseover";
    private const string HtmlAttributeOnMouseOut = "onmouseout";
    private const string HtmlAttributeOnMouseClick =  "onclick";
    private const string HtmlAttributeSortDirection = "SortDirection";

    private const string GvColumnId = "Id";
    private const string GvColumnReadDate = "ReadDate";
    private const string GvColumnTo = "To";
    private const string GvColumnFrom = "From";

    private const string GvColumnSentdateHeader = "Date Sent";

    private const string NullDisplayText = "null";


    #endregion

    #region Page Method

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            OpenInbox();
        }
        else
        {
            HandlePostBack();
        }
    }
    
    #endregion

    #region General Methods

    private void HandlePostBack()
    {
        string target = Request[RequestEventTargetId];

        if (target == TargetOpenPrivateMessageId)
        {
            string pmId = Request[RequestEventArgumentId];
            if (!string.IsNullOrEmpty(pmId))
            {
                OpenPrivateMessage(pmId);
            }
            else
            {
                OpenInbox();
            }
        }
    }

    private void OpenInbox()
    {
        OpenInbox(string.Empty);
    }

    private void OpenInbox(string sortExpression)
    {
        ResetAll();
        hidFolder.Value = PrivateMessagesManager.Folders.Inbox;
        BindInbox(sortExpression);
        divList.Visible = true;
    }

    private void OpenSentbox()
    {
        OpenSentbox(string.Empty);
    }

    private void OpenSentbox(string orderBy)
    {
        ResetAll();
        hidFolder.Value = PrivateMessagesManager.Folders.Sent;
        BindSentbox(orderBy);
        divList.Visible = true;
    }

    private void BindInbox()
    {
        BindInbox(string.Empty);
    }
    
    private void BindInbox(string sortExpression)
    {
        User currentUser = MembershipManager.GetUserByName(Parent.Page.User.Identity.Name);
        List<PrivateMessage> pms = PrivateMessagesManager.GetAllPrivateMessages(currentUser).ToList<PrivateMessage>();
        BindList(pms, sortExpression);
    }

    private void BindSentbox()
    {
        BindSentbox(string.Empty);
    }

    private void BindSentbox(string sortExpression)
    {
        User currentUser = MembershipManager.GetUserByName(Parent.Page.User.Identity.Name);
        List<PrivateMessage> pms = PrivateMessagesManager.GetAllSentPrivateMessages(currentUser).ToList<PrivateMessage>();
        BindList(pms, sortExpression);
    }

    private void BindList(List<PrivateMessage> privateMessages, string sortExpression)
    {
        if (!string.IsNullOrEmpty(sortExpression))
        {            
            SortDirection direction;
            if (grdMessages.Attributes[HtmlAttributeSortDirection] == SortDirection.Ascending.ToString())
                direction = SortDirection.Descending;
                
            else
                direction = SortDirection.Ascending;

            grdMessages.Attributes[HtmlAttributeSortDirection] = direction.ToString();
            var comparer = new DynamicComparer<PrivateMessage>(sortExpression, direction);
            privateMessages.Sort(comparer.Compare);
        }

        grdMessages.DataSource = privateMessages;
        grdMessages.DataBind();
    }

    private void OpenPrivateMessage(string privateMessageId)
    {
        ResetAll();
        divList.Visible = false;
        hidPrivateMessageId.Value = privateMessageId;

        PrivateMessage msg = PrivateMessagesManager.GetPrivateMessage(privateMessageId);

        if (msg != null)
        {
            User currentUser = MembershipManager.GetUserByName(Page.User.Identity.Name);
            if (currentUser.Id == msg.Recipient.Id || currentUser.Id == msg.Sender.Id)
            {
                if (msg.ReadDate == null)
                    PrivateMessagesManager.MarkAsRead(msg);

                divViewPrivateMessage.Visible = true;

                lblDateSent.Text = PrivateMessagesManager.FormatDateForDisplay(msg.SentDate);
                lblTo.Text = msg.Recipients;
                lblFrom.Text = msg.Sender.Name;
                lblSubject.Text = msg.Subject;
                txtBody.Text = msg.Body;
            }
            else
            {
                ((IFeedback)Page.Master).SetError(GetType(), ErrorPermissionDenied);
            }
        }
        else
        {
            ((IFeedback)Page.Master).SetError(GetType(), ErrorUnableToOpen);
        }
    }

    private void SendPrivateMessage()
    {
        if (string.IsNullOrEmpty(txtNewSubject.Text.Trim()))
        {
            ((IFeedback)Page.Master).SetError(GetType(), ErrorSubjectEmpty);
        }
        if (string.IsNullOrEmpty(txtNewBody.Text.Trim()))
        {
            ((IFeedback)Page.Master).SetError(GetType(), ErrorBodyEmpty);
        }
        else
        {
            bool illFormedRecipientString = false;

            List<User> users = new List<User>();

            foreach (string recipient in txtNewTo.Text.Split(','))
            {
                if (!string.IsNullOrEmpty(recipient.Trim()))
                {
                    if ((recipient.IndexOf("(") < 0 || recipient.IndexOf(")") < 0)
                    || (recipient.LastIndexOf("(") != recipient.IndexOf("("))
                    || (recipient.LastIndexOf(")") != recipient.IndexOf(")")))
                    {
                        illFormedRecipientString = true;
                    }
                    else
                    {
                        string username = (recipient.Split('(')[1]).Split(')')[0]; ;
                        try
                        {
                            User user = MembershipManager.GetUserByName(username);
                            if (user == null)
                                illFormedRecipientString = true;
                            else if (!users.Contains(user))
                                users.Add(user);
                        }
                        catch (Exception)
                        {
                            illFormedRecipientString = true;
                        }
                    }
                }
            }

            if (illFormedRecipientString || users.Count == 0)
            {
                ((IFeedback)Page.Master).SetError(GetType(), ErrorIllFormedRecipientString);
            }
            else
            {
                var recipients = new StringBuilder();
                bool first = true;
                foreach (User user in users)
                {
                    if (first)
                        first = false;
                    else
                        recipients.Append(", ");

                    recipients.Append(user.Person.FirstName);
                    recipients.Append(" ");
                    recipients.Append(user.Person.LastName);
                    recipients.Append(" (");
                    recipients.Append(user.Name).Append(")");
                }

                foreach (User user in users)
                {
                    var pMsg = new PrivateMessage();
                    pMsg.Recipient = user;
                    pMsg.Sender = MembershipManager.GetUserByName(Page.User.Identity.Name);
                    pMsg.Recipients = recipients.ToString();
                    pMsg.Subject = txtNewSubject.Text;
                    pMsg.Body = txtNewBody.Text;
                    string parentPrivateMessageId = hidPrivateMessageId.Value;
                    if (!string.IsNullOrEmpty(parentPrivateMessageId))
                    {
                        PrivateMessage parentMessage = PrivateMessagesManager.GetPrivateMessage(parentPrivateMessageId);
                        if (parentMessage != null)
                        {
                            pMsg.ParentPrivateMessage = parentMessage;
                        }
                    }
                    PrivateMessagesManager.Send(pMsg);
                }
                OpenInbox();
            }
        }
    }

    private void ResetAll()
    {
        txtNewTo.Text = "";
        txtNewSubject.Text = "";
        txtNewBody.Text = "";
        hidPrivateMessageId.Value = "";

        txtBody.Text = "";
        lblTo.Text = "";
        lblFrom.Text = "";
        lblSubject.Text = "";
        lblDateSent.Text = "";
        btnReplyOpenedMessage.CommandArgument = "";

        divList.Visible = false;
        divNewPrivateMessage.Visible = false;
        divViewPrivateMessage.Visible = false;

        for (int i = 0; i < grdMessages.Columns.Count; i++)
            grdMessages.Columns[i].Visible = true;

        for (int i = 0; i < grdMessages.Rows.Count; i++)
            grdMessages.Rows[i].Cells.Clear();

        grdMessages.Attributes[HtmlAttributeSortDirection] = SortDirection.Descending.ToString();
    }

    private void RecyclePrivateMessage(string divider, string subjectPrefix)
    {
        string subject = lblSubject.Text;
        if (subject.IndexOf(subjectPrefix) != 0)
            subject = subjectPrefix + subject;

        PrivateMessage pm = PrivateMessagesManager.GetPrivateMessage(hidPrivateMessageId.Value);

        string parentPrivateMessage = hidPrivateMessageId.Value;
        User fromUser = MembershipManager.GetUserByName(lblFrom.Text);

        string from = pm.Sender.Person.FirstName + " " + pm.Sender.Person.LastName + " (" + pm.Sender.Name + ")";
        
        ResetAll();

        hidPrivateMessageId.Value = parentPrivateMessage;
        txtNewTo.Text = fromUser.Person.FirstName + " " + fromUser.Person.LastName + " (" + fromUser.Name + "), "; ;
        txtNewSubject.Text = subject;
        txtNewBody.Text = string.Format(RecyledPrivateMessage, divider, from, pm.SentDate.ToString(), pm.Recipients, txtNewSubject.Text, pm.Body);

        divNewPrivateMessage.Visible = true;
    }

    private void HideGridViewColumnByHeader(GridView gridView, string columnName)
    {
        DataControlFieldCollection columns = gridView.Columns;

        int indexOfColumn = GetGridViewColumnIndex(columns, columnName);

        if (indexOfColumn >= 0)
            columns[indexOfColumn].Visible = false;
    }

    private int GetGridViewColumnIndex(DataControlFieldCollection columns, string columnName)
    {
        int index = -1;
        foreach (DataControlField field in columns)
        {
            if (index == -1)
            {
                string headerName = "";
                if (field is BoundField)
                    headerName = ((BoundField)field).HeaderText;
                else if (field is TemplateField)
                    headerName = ((TemplateField)field).HeaderText;
                if (headerName == columnName)
                    index = columns.IndexOf(field);
            }
        }
        return index;
    }

    #endregion

    #region Web Methods

    public static object GetUsernames()
    {
        List<string> usernames = new List<string>();

        foreach (User user in MembershipManager.GetAllUsers())
        {
            Person person = PersonManager.GetPersonByUserId(user.Id);
            if(person != null)
                usernames.Add(person.FirstName + " " + person.LastName + " (" + user.Name + ")");
        }

        return usernames;
    }

    #endregion

    #region Event Methods


    protected void lnkNew_Click(object sender, EventArgs e)
    {
        ResetAll();
        divNewPrivateMessage.Visible = true;
    }

    protected void lnkInbox_Click(object sender, EventArgs e)
    {
        OpenInbox();
    }

    protected void lnkSentMessages_Click(object sender, EventArgs e)
    {
        OpenSentbox();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string messageId = ((Button)sender).CommandArgument;

        PrivateMessagesManager.DeletePrivateMessage(messageId);

        OpenInbox();
    }

    protected void btnOpen_Click(object sender, EventArgs e)
    {
        string messageId = ((Button)sender).CommandArgument;

        OpenPrivateMessage(messageId);
    }

    protected void btnNewSend_Click(object sender, EventArgs e)
    {
        SendPrivateMessage();
    }

    protected void btnReplyOpenedMessage_Click(object sender, EventArgs e)
    {
        RecyclePrivateMessage(ReplyOrigionalHeading, ReplySubjectPrefix);
    }

    protected void btnNewCancel_Click(object sender, EventArgs e)
    {
        OpenInbox();
    }

    protected void btnForwardMessage_Click(object sender, EventArgs e)
    {
        RecyclePrivateMessage(ForwardOrigionalHeading, ForwardSubjectPrefix);
        txtNewTo.Text = "";
    }

    protected void btnDeleteOpenedMessage_Click(object sender, EventArgs e)
    {
        PrivateMessagesManager.DeletePrivateMessage(hidPrivateMessageId.Value);
        OpenInbox();
    }

    protected void grdMessages_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int colIndex = GetGridViewColumnIndex(grdMessages.Columns, GvColumnReadDate);
            if (hidFolder.Value == PrivateMessagesManager.Folders.Inbox)
            {
                if (colIndex >= 0)
                {
                    if (e.Row.Cells[colIndex].Text == NullDisplayText)
                        foreach (TableCell cell in e.Row.Cells)
                            cell.Font.Bold = true;
                }
            }

            e.Row.Attributes[HtmlAttributeOnMouseOver] = JavascriptOnMouseOver;
            e.Row.Attributes[HtmlAttributeOnMouseOut] = JavascriptOnMouseOut;
            
            colIndex = GetGridViewColumnIndex(grdMessages.Columns, GvColumnId);
            if (colIndex >= 0)
            {
                e.Row.Attributes[HtmlAttributeOnMouseClick] = string.Format(JavascriptViewMessage, e.Row.Cells[colIndex].Text);
            }

            colIndex = GetGridViewColumnIndex(grdMessages.Columns, GvColumnSentdateHeader);
            if (colIndex >= 0)
            {
                DateTime sentDate = DateTime.Parse(e.Row.Cells[colIndex].Text);
                e.Row.Cells[colIndex].Text = PrivateMessagesManager.FormatDateForDisplay(sentDate);
            }
        }
    }

    protected void grdMessages_DataBound(object sender, EventArgs e)
    {
        GridView gridView = (GridView)sender;

        HideGridViewColumnByHeader(gridView, GvColumnId);
        HideGridViewColumnByHeader(gridView, GvColumnReadDate);

        if (hidFolder.Value == PrivateMessagesManager.Folders.Inbox)
        {
            HideGridViewColumnByHeader(gridView, GvColumnTo);
        }
        else if (hidFolder.Value == PrivateMessagesManager.Folders.Sent)
        {
            HideGridViewColumnByHeader(gridView, GvColumnFrom);
        }
    }

    protected void grdMessages_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMessages.PageIndex = e.NewPageIndex;

        if (hidFolder.Value == PrivateMessagesManager.Folders.Inbox)
        {
            BindInbox();
        }
        else if (hidFolder.Value == PrivateMessagesManager.Folders.Sent)
        {
            BindSentbox();
        }
    }

    protected void grdMessages_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;

        if (hidFolder.Value == PrivateMessagesManager.Folders.Inbox)
        {
            BindInbox(sortExpression);
        }
        else if (hidFolder.Value == PrivateMessagesManager.Folders.Sent)
        {
            BindSentbox(sortExpression);
        }
    }

    #endregion

}
