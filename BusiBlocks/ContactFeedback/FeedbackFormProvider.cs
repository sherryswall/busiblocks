using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.ContactFeedback
{
    public abstract class FeedbackFormProvider : ProviderBase
    {
        public abstract void CreateFeedbackFormRequest(FeedbackForm form);
    }
}
