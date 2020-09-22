using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.Versioning
{
    public class VersionProviderCollection : ProviderCollection
    {
        public new VersionProvider this[string name]
        {
            get { return (VersionProvider) base[name]; }
        }
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is VersionProvider))
                throw new ArgumentException("The provider parameter must be of type VerionProvider.", "provider");
            
            base.Add(provider);
        }
    }
    
}
