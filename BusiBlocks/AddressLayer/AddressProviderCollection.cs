using System;
using System.Configuration.Provider;

namespace BusiBlocks.AddressLayer
{
    public class AddressProviderCollection : ProviderCollection
    {
        public new AddressProvider this[string name]
        {
            get { return (AddressProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is AddressProvider))
                throw new ArgumentException("The provider parameter must be of type AddressProvider.");

            base.Add(provider);
        }

        public void CopyTo(AddressProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}