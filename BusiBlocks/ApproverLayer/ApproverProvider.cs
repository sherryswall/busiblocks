using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.ApproverLayer
{
    public abstract class ApproverProvider : ProviderBase
    {
        public abstract void CreateApprover(Approver approver);

        public abstract void DeleteApproversByItem(string itemId);

        public abstract IList<Approver> GetApproversByItem(string itemId);
    }
}