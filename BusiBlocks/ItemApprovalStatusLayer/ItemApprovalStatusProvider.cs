using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.ItemApprovalStatusLayer
{
    public abstract class ItemApprovalStatusProvider : ProviderBase
    {
        public abstract ItemApprovalStatus GetItemApprovalStatus(string id);

        public abstract ItemApprovalStatus GetItemApprovalStatusByName(string name);
    }
}