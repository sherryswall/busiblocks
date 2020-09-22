using System;
using System.Configuration.Provider;

namespace BusiBlocks.PersonLayer
{
    public class PersonProviderCollection : ProviderCollection
    {
        public new PersonProvider this[string name]
        {
            get { return (PersonProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is PersonProvider))
                throw new ArgumentException("The provider parameter must be of type PersonProvider.");

            base.Add(provider);
        }

        public void CopyTo(PersonProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}