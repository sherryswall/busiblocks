using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.ContactFeedback
{
    public class FeedbackForm
    {
        public virtual string Id { get; set; }
        public virtual string Type { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Comments { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual string Theme { get; set; }
        public virtual string Page { get; set; }
        public virtual string Browser { get; set; }
        public virtual string UserId { get; set; }
    }
    
}
