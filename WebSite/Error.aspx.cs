using System;
using System.Web;
using System.Text;
using System.Configuration;
using BusiBlocks.Notification;

public partial class Error : System.Web.UI.Page
{
    private const string EmailErrorFrom = "support@sigmabravo.com";
    private const string Delimiter = "----------------------------------------------------------------------------------";

    protected void Page_Load(object sender, EventArgs e)
    {
        string message = BuildTechnicalMessage();
        
        LogError(message);

#if RELEASE
        EmailError(message);
#endif

#if DEBUG
        ShowTechnicalDetails(message);
#endif

    }


    private string BuildTechnicalMessage()
    {
        StringBuilder message = new StringBuilder();
        Exception exception = null;
        try
        {
            if (!(Session["error"] == null))
            {
                exception = (Exception)Session["error"];
            }
            else
            {
                message.Append("Error retreiving details: No data available\r\n");
            }
        }
        catch (HttpException)
        {
            message.Append("Error retreiving details: Session unavailable\r\n");
        }

        Session["error"] = null;

        if (exception != null && string.IsNullOrEmpty(message.ToString()))
        {
            if (!string.IsNullOrEmpty(exception.Message))
                message.Append("\r\nError Message: " + exception.Message);

            if (!string.IsNullOrEmpty(Request.Url.ToString()))
                message.Append("\r\nPage source: " + Request.Url);

            if (!string.IsNullOrEmpty(exception.StackTrace.ToString()))
                message.Append("\r\nStack Trace:" + Environment.NewLine + exception.StackTrace.ToString());

            message.Append("\r\n");
        }

        return message.ToString();
    }


    private void LogError(string message)
    {
        BusiBlocks.Log.Error(GetType(), message + Delimiter);
    }

    private void EmailError(string message)
    {
        var mail = new System.Net.Mail.MailMessage();

        mail.From = new System.Net.Mail.MailAddress(EmailErrorFrom);
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SupportMailingList"]))
        {
            var SupportMailingList = ConfigurationManager.AppSettings["SupportMailingList"].Split(';');
            foreach (var address in SupportMailingList)
            {
                if (!String.IsNullOrEmpty(address))
                {
                    mail.To.Add(new System.Net.Mail.MailAddress(address));
                }
            }
        }

        if (mail.To.Count > 0)
        {
            mail.IsBodyHtml = false;
            mail.Body = "Unhandled exception was thrown at " + DateTime.Now.ToString("h:m:s tt") + " on " + DateTime.Now.ToString("d/M/yyyy") + Environment.NewLine + message;
            var mNotificationProvider = new BusiBlocksSmtpNotificationProvider();
            mNotificationProvider.SendMail(mail);
        }
    }


    private void ShowTechnicalDetails(string message)
    {
        lbError.Text = message.Replace("\r\n", "<br>");
        pnlDebug.Visible = true;
    }
}
