using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Configuration;

namespace BusiBlocks.ContactFeedback
{
    public static class FeedbackFormManager
    {
        //Public feature API
        private static FeedbackFormProviderCollection providerCollection = InitialiseProviderCollection();

        private static FeedbackFormProvider defaultProvider
        {
            get
            {
                var ac =
                    (FeedbackFormConfiguration)ConfigurationManager.GetSection("feedbackFormManager");

                if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                    throw new ProviderException("You must specify a valid default provider for feedbackFormManager.");

                return providerCollection[ac.DefaultProvider];
            }
        }

        public static FeedbackFormProvider Provider
        {
            get { return defaultProvider; }
        }

        public static FeedbackFormProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static FeedbackFormProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (FeedbackFormConfiguration)ConfigurationManager.GetSection("feedbackFormManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for feedbackFormManager.");

            //Instantiate the providers
            providerCollection = new FeedbackFormProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(FeedbackFormProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        public static void CreateFeedbackFormRequest(FeedbackForm form)
        {
            if (form == null)
                throw new NotImplementedException();
            Provider.CreateFeedbackFormRequest(form);
        }

    }
}
