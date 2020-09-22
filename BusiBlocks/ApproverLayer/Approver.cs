using System;
using System.Collections.Generic;

namespace BusiBlocks.ApproverLayer
{
    [Serializable]
    public class Approver
    {
        public virtual string Id { get; set; }
        public virtual string ItemId { get; set; }
        public virtual string UserId { get; set; }
        public virtual string CategoryId { get; set; }
        public virtual bool Deleted { get; set; } 
    }

    public class ApproverList
    {
        public List<Approver> Approvers { get; set; }

        public ApproverList()
        {
            Approvers = new List<Approver>();
        }

        
    }
}