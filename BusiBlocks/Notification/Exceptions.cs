using System;

namespace BusiBlocks.Notification
{
    [Serializable]
    public class SmtpNotificationException : BusiBlocksException
    {
        public SmtpNotificationException(string destinationUser, Exception innerException)
            : base("Failed to send Smtp notification to " + destinationUser, innerException)
        {
        }
    }
}