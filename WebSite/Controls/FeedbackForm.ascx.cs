using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusiBlocks.ContactFeedback;
using System.Net.Mail;
using System.Xml;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;
using System.Configuration;
using System.Web.Configuration;

public partial class Controls_FeedbackForm : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSubmit_OnClick(object sender, EventArgs e)
    {

        Configuration configuration =
        WebConfigurationManager.OpenWebConfiguration("~/");

        PagesSection pagesSection = (PagesSection)configuration.GetSection("system.web/pages");

        BusiBlocks.Notification.BusiBlocksSmtpNotificationProvider bb = new BusiBlocks.Notification.BusiBlocksSmtpNotificationProvider();

        FeedbackForm form = new FeedbackForm()
        {
            Type = ddlFeedbackType.SelectedValue,
            Subject = txtSubject.Text,
            Comments = txtComments.Text,
            Theme = pagesSection.Theme.ToString(),
            Browser = Request.Browser.Type,
            Page = this.Page.ToString(),
            Time = DateTime.Now,
            UserId = Page.User.Identity.Name
        };

        bb.SendMail(CreateMessageFromTemplate(form));
        FeedbackFormManager.CreateFeedbackFormRequest(form);
        ClearFields();
    }

    private void ClearFields()
    {
        ddlFeedbackType.SelectedIndex = 0;
        txtComments.Text = string.Empty;
        txtSubject.Text = string.Empty;
    }

    private MailMessage CreateMessageFromTemplate(FeedbackForm form)
    {
        MailMessage message = new MailMessage();
        var xmlDocument = new XmlDocument();

        string xmlFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/MailTemplate_Feedback.xml");

        xmlDocument.Load(xmlFile);
        XmlNodeList blockNodes = xmlDocument.GetElementsByTagName("body");


        foreach (XmlNode node in blockNodes)
        {
            message.Body = node.InnerText;
        }

        User user = MembershipManager.GetUserByName(form.UserId);
        Person person = PersonManager.GetPersonByUserId(user.Id);

        message.From = new MailAddress((!string.IsNullOrEmpty(person.Email)) ? person.Email : "no-reply@busiblocks.com");
        message.To.Add(new MailAddress("support@busiblocks.com"));
        message.Subject = form.Type + "-" + form.Subject;
        message.Body = ReplaceTokens(message.Body, form, user, person);

        return message;
    }

    private string ReplaceTokens(string messageBody, FeedbackForm form, User user, Person person)
    {
        //get the theme name
        messageBody = messageBody.Replace("$type", form.Type);
        messageBody = messageBody.Replace("$subject", form.Subject);
        messageBody = messageBody.Replace("$comments", form.Comments);
        messageBody = messageBody.Replace("$time", form.Time.ToString());
        messageBody = messageBody.Replace("$theme", form.Theme);
        messageBody = messageBody.Replace("$page", form.Page);
        messageBody = messageBody.Replace("$browser", form.Browser);
        messageBody = messageBody.Replace("$username", user.Name);
        messageBody = messageBody.Replace("$email", person.Email);
        messageBody = messageBody.Replace("$phonenumber", person.PhoneNumber);
        return messageBody;
    }
}