using System;
using System.Collections.Generic;
using System.Text;
using BusiBlocks.Membership;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    public class PrivateMessage
    {
        public virtual string Id { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Recipient { get; set; }

        public virtual string Recipients { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual DateTime SentDate { get; set; }
        public virtual DateTime? ReadDate { get; set; }
        public virtual DateTime? DeletedDate { get; set; }

        public virtual PrivateMessage ParentPrivateMessage { get; set; }

        public virtual bool Deleted { get; set; }
    }
}
