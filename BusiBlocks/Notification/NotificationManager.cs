using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace BusiBlocks.Notification
{
    public static class NotificationManager
    {
        private static NotificationProviderCollection providerCollection;

        public static NotificationProviderCollection Providers
        {
            get { return ReturnProviderCollection(); }
        }

        public static NotificationProviderCollection ReturnProviderCollection()
        {
            //Get the feature's configuration info
            var qc =
                (NotificationProviderConfiguration) ConfigurationManager.GetSection("notificationManager");

            if (qc == null || qc.Providers == null)
                throw new ProviderException("Providers for notificationManager not valid, null returned.");

            //Instantiate the providers
            providerCollection = new NotificationProviderCollection();
            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof (NotificationProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }
    }
}