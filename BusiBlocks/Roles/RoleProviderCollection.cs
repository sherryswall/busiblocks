using System;
using System.Configuration.Provider;

namespace BusiBlocks.Roles
{
    public class RoleProviderCollection : ProviderCollection
    {
        public new RoleProvider this[string name]
        {
            get { return (RoleProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is RoleProvider))
                throw new ArgumentException("The provider parameter must be of type RoleProvider.");

            base.Add(provider);
        }

        public void CopyTo(RoleProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}