using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.FormsBlock
{
    public class FormsProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is FormsProvider))
                throw new ArgumentException("The provider parameter must be of type FormsProvider.");

            base.Add(provider);
        }

        new public FormsProvider this[string name]
        {
            get { return (FormsProvider)base[name]; }
        }

        public void CopyTo(FormsProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
