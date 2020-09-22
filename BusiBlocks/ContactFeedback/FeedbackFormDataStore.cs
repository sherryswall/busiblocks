using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.ContactFeedback
{
    public class FeedbackFormDataStore : EntityDataStoreBase<FeedbackForm, string>
    {

        public FeedbackFormDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }
    }
}
