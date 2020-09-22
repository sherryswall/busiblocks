using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.ContactFeedback
{
    public class FeedbackFormProviderCollection : ProviderCollection 
    {
        public new FeedbackFormProvider this[string name]
        {
            get { return (FeedbackFormProvider)base[name]; }
        }
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is FeedbackFormProvider))
                throw new ArgumentException("The provider parameter must be of type FeedbackFormProvider.", "provider");

            base.Add(provider);
        }
    }
}
