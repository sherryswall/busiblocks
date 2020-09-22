using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.CommsBlock.Forums
{
    public class ForumsProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is ForumsProvider))
                throw new ArgumentException("The provider parameter must be of type ForumsProvider.");

            base.Add(provider);
        }

        new public ForumsProvider this[string name]
        {
            get { return (ForumsProvider)base[name]; }
        }

        public void CopyTo(ForumsProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
