using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.DocoBlock
{
    public class DocoProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is DocoProvider))
                throw new ArgumentException("The provider parameter must be of type DocoProvider.");

            base.Add(provider);
        }

        new public DocoProvider this[string name]
        {
            get { return (DocoProvider)base[name]; }
        }

        public void CopyTo(DocoProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
