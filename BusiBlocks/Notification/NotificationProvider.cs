using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Security;
using BusiBlocks.ContactFeedback;
using System.Net.Mail;

namespace BusiBlocks.Notification
{
    /// <summary>
    /// NotificationProvider abstract class. 
    /// Defines the contract that BusiBlocks implements to provide notification services using custom notification providers.
    /// You can use this provider as a generic way to define notifications.
    /// You can use the BusiBlocksSmtpNotificationProvider to use EMail notifications.
    /// </summary>
    public abstract class NotificationProvider : ProviderBase
    {
        /// <summary>
        /// Send a notification to an user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="parameters">A set of parameters</param>
        public abstract void NotifyUser(MembershipUser user, Dictionary<string, string> parameters);

        /// <summary>
        /// Returns true if the user can receive notification, otherwise false.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public abstract bool UserCanReceiveNotification(MembershipUser user);

        public abstract void SendMail(MailMessage message);
    }
}