using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Net.Mail;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using System.Xml.XPath;
using BusiBlocks.Membership;
using BusiBlocks.ContactFeedback;
using System.Xml;

namespace BusiBlocks.Notification
{
    /// <summary>
    /// Implementation of NotificationProvider using email smtp notifications.
    /// Configuration:
    /// template = the file that contains the template to use. Typically a path like: App_Data\MailTemplate.xml . If the path is relative is considered to be inside the ApplicationBase directory.
    /// enabled = true to enabled the provider (default true)
    /// 
    /// Check if the user can receive notifications using the 'ReceiveNotification' properties of the user profiles.
    /// </summary>
    public class BusiBlocksSmtpNotificationProvider : NotificationProvider
    {
        private const string PROFILE_RECEIVENOTIFICATION = "ReceiveNotification";

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksNotificationProvider";

            base.Initialize(name, config);

            ProviderName = name;

            //Read the configurations
            string templatePath = ExtractConfigValue(config, "template", null);
            if (templatePath == null ||
                templatePath.Length == 0)
            {
                throw new BusiBlocksException("template cannot be empty.");
            }

            templatePath = PathHelper.LocateServerPath(templatePath);
            mTemplate = new XPathDocument(templatePath);


            Enabled = bool.Parse(ExtractConfigValue(config, "enabled", "true"));

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " +
                                                attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;
            else
            {
                config.Remove(key);

                return val;
            }
        }

        private MailMessage CreateMailMessageFromTemplate(XPathDocument template,
                                                          MembershipUser user,
                                                          Dictionary<string, string> parameters)
        {
            XPathNavigator navigator = template.CreateNavigator();

            // Specify the e-mail sender.
            var from = new MailAddress((string)navigator.Evaluate("string(template/sender)"));
            // Set destinations for the e-mail message.
            var to = new MailAddress(user.Email);

            var message = new MailMessage(from, to);

            message.IsBodyHtml = bool.Parse((string)navigator.Evaluate("string(template/body/@html)"));
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            var body = (string)navigator.Evaluate("string(template/body)");
            message.Body = ReplaceParameters(body, parameters);

            var subject = (string)navigator.Evaluate("string(template/subject)");
            message.Subject = ReplaceParameters(subject, parameters);

            return message;
        }

        private string ReplaceParameters(string source, Dictionary<string, string> parameters)
        {
            string destination = source;
            foreach (string key in parameters.Keys)
            {
                destination = destination.Replace(key, parameters[key]);
            }

            return destination;
        }

        #region Properties

        private XPathDocument mTemplate;
        public string ProviderName { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the xml template to use.
        /// </summary>
        public XPathDocument Template
        {
            get { return mTemplate; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send an email notification to an user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="parameters">A set of parameters</param>
        public override void NotifyUser(MembershipUser user, Dictionary<string, string> parameters)
        {
            if (Enabled == false)
                return;

            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Email))
                throw new EmailRequiredException(user.UserName);

            // Specify the message content.
            using (MailMessage message = CreateMailMessageFromTemplate(Template, user, parameters))
            {
                //For now I don't use async method because I don't known exactly how to handle the message
                // dispose and for MSDN seems that you cannot execute 2 or more SendAsync without waiting that the first mail is completed
                //// Set the method that is called back when the send operation ends.
                //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                //// The userState can be any object that allows your callback 
                //// method to identify this send operation.
                //// For this example, the userToken is a string constant.
                //string userState = "Notification to " + user.Email + " subject: " + subject;
                //SmtpClient client = new SmtpClient();
                //client.SendAsync(message, userState);

                try
                {
                    var client = new SmtpClient();
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    throw new SmtpNotificationException(user.UserName, ex);
                }
            }
        }

        public override bool UserCanReceiveNotification(MembershipUser user)
        {
            if (Enabled == false)
                return false;

            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Email))
                return false;
            else
            {
                ProfileBase profile = ProfileBase.Create(user.UserName);

                object profReceiveNotification = profile[PROFILE_RECEIVENOTIFICATION];
                if (profReceiveNotification is bool)
                    return (bool)profReceiveNotification;
                else
                    return false;
            }
        }
        
        public override void SendMail(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        #endregion

        //#region Smtp Async methods
        //private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        //{
        //    // Get the unique identifier for this asynchronous operation.
        //    String token = (string)e.UserState;

        //    if (e.Cancelled)
        //    {
        //        Console.WriteLine("[{0}] Send canceled.", token);
        //    }
        //    if (e.Error != null)
        //    {
        //        Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
        //    }
        //    else
        //    {
        //        Console.WriteLine("Message sent.");
        //    }
        //    mailSent = true;
        //}
        //#endregion
    }
}