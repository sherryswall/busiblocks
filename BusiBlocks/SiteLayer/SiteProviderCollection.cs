using System;
using System.Configuration.Provider;

namespace BusiBlocks.SiteLayer
{
    public class SiteProviderCollection : ProviderCollection
    {
        public new SiteProvider this[string name]
        {
            get { return (SiteProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is SiteProvider))
                throw new ArgumentException("The provider parameter must be of type SiteProvider.", "provider");

            base.Add(provider);
        }

        public void CopyTo(SiteProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}