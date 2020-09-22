using System;
using System.Configuration.Provider;

namespace BusiBlocks.ApproverLayer
{
    public class ApproverProviderCollection : ProviderCollection
    {
        public new ApproverProvider this[string name]
        {
            get { return (ApproverProvider)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is ApproverProvider))
                throw new ArgumentException("The provider parameter must be of type AccessProvider.");

            base.Add(provider);
        }

        public void CopyTo(ApproverProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}