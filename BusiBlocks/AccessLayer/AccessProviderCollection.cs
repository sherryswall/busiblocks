using System;
using System.Configuration.Provider;

namespace BusiBlocks.AccessLayer
{
    public class AccessProviderCollection : ProviderCollection
    {
        public new AccessProvider this[string name]
        {
            get { return (AccessProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is AccessProvider))
                throw new ArgumentException("The provider parameter must be of type AccessProvider.");

            base.Add(provider);
        }

        public void CopyTo(AccessProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}