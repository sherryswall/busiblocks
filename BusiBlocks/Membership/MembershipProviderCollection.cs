using System;
using System.Configuration.Provider;

namespace BusiBlocks.Membership
{
    public class MembershipProviderCollection : ProviderCollection
    {
        public new MembershipProvider this[string name]
        {
            get { return (MembershipProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is MembershipProvider))
                throw new ArgumentException("The provider parameter must be of type MembershipProvider.");

            base.Add(provider);
        }

        public void CopyTo(MembershipProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}