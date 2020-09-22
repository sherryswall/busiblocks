using System;
using System.Configuration.Provider;

namespace BusiBlocks.ItemStatusLayer
{
    public class ItemStatusProviderCollection : ProviderCollection
    {
        public new ItemStatusProvider this[string name]
        {
            get { return (ItemStatusProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is ItemStatusProvider))
                throw new ArgumentException("The provider parameter must be of type ItemStatusProvider.");

            base.Add(provider);
        }

        public void CopyTo(ItemStatusProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}