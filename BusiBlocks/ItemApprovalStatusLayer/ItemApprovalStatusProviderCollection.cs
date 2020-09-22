using System;
using System.Configuration.Provider;

namespace BusiBlocks.ItemApprovalStatusLayer
{
    public class ItemApprovalStatusProviderCollection : ProviderCollection
    {
        public new ItemApprovalStatusProvider this[string name]
        {
            get { return (ItemApprovalStatusProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is ItemApprovalStatusProvider))
                throw new ArgumentException("The provider parameter must be of type ItemApprovalStatusProvider.");

            base.Add(provider);
        }

        public void CopyTo(ItemApprovalStatusProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}