using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.ItemStatusLayer
{
    public abstract class ItemStatusProvider : ProviderBase
    {
        public abstract ItemStatus GetItemStatus(string id);

        public abstract ItemStatus GetItemStatusByName(string name);
    }
}