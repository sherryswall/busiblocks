using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    public class PrivateMessagesProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is PrivateMessagesProvider))
                throw new ArgumentException("The provider parameter must be of type PrivateMessagesProvider.");

            base.Add(provider);
        }

        new public PrivateMessagesProvider this[string name]
        {
            get { return (PrivateMessagesProvider)base[name]; }
        }

        public void CopyTo(PrivateMessagesProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
