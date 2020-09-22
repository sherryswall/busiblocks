using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Configuration;
using System.Linq;
using BusiBlocks.Membership;
using System.Text;
using System.Globalization;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    public class PrivateMessagesManager
    {
        static PrivateMessagesManager()
        {
            //Get the feature's configuration info
            PrivateMessagesProviderConfiguration qc = (PrivateMessagesProviderConfiguration)ConfigurationManager.GetSection("privateMessagesManager");

            if (qc == null || qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for privateMessagesManager.");

            //Instantiate the providers
            providerCollection = new PrivateMessagesProviderCollection();
            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(PrivateMessagesProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[qc.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the privateMessagesManager.",
                    qc.ElementInformation.Properties["defaultProvider"].Source,
                    qc.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        //Public feature API
        private static PrivateMessagesProvider defaultProvider;
        private static PrivateMessagesProviderCollection providerCollection;

        public static PrivateMessagesProvider Provider
        {
            get{return defaultProvider;}
        }

        public static PrivateMessagesProviderCollection Providers
        {
            get{return providerCollection;}
        }

        public static IList<PrivateMessage> GetAllPrivateMessages(User User)
        {
            if (User == null)
                throw new ArgumentNullException("User");

            return Provider.FindByRecipient(User);
        }

        public static IList<PrivateMessage> GetAllSentPrivateMessages(User User)
        {
            if (User == null)
                throw new ArgumentNullException("User");

            return Provider.FindBySender(User);
        }

        public static PrivateMessage GetPrivateMessage(string PrivateMessageId)
        {
            if (string.IsNullOrEmpty(PrivateMessageId))
                throw new ArgumentNullException("PrivateMessageId");

            return Provider.FindById(PrivateMessageId);
        }

        public static void DeletePrivateMessage(string PrivateMessageId)
        {
            PrivateMessage message = GetPrivateMessage(PrivateMessageId);

            if (message == null)
                throw new ArgumentNullException("PrivateMessage");
            
            message.DeletedDate = DateTime.Now;

            Provider.Update(message);
        }

        public static void Send(PrivateMessage PrivateMessage)
        {
            if (PrivateMessage == null)
                throw new ArgumentNullException("PrivateMessage");
            
            PrivateMessage.SentDate = DateTime.Now;
            Provider.Insert(PrivateMessage);
        }


        public static void MarkAsRead(PrivateMessage PrivateMessage)
        {
            if (PrivateMessage == null)
                throw new ArgumentNullException("PrivateMessage");

            PrivateMessage.ReadDate = DateTime.Now;
            Provider.Update(PrivateMessage);
        }

        public static class Folders
        {
            public const string Inbox = "Inbox";
            public const string Sent = "Sent";
            //TO DO: public static const string Sent = "Trash"; //N.B. Nobody ever wants to take out the trash.
        }

        public static string FormatDateForDisplay(DateTime Date)
        {
            StringBuilder formattedDate = new StringBuilder();
            if (Date.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                formattedDate.Append("Today");
            }
            else if (Date.ToShortDateString() == DateTime.Now.AddDays(-1).ToShortDateString())
            {
                formattedDate.Append("Yesterday");
            }
            else
            {
                formattedDate.Append(Date.DayOfWeek);

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.Month).Substring(0, 3);

                formattedDate.Append(", ");
                formattedDate.Append(Date.Day);
                formattedDate.Append(" ");
                formattedDate.Append(monthName);
                formattedDate.Append(", ");
                formattedDate.Append(Date.Year);
            }

            formattedDate.Append(" at ");

            formattedDate.Append(Date.ToString("HH:mm"));

            return formattedDate.ToString();
        }
    }
}
