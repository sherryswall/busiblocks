using System;
using System.Configuration.Provider;

namespace BusiBlocks.Notification
{
    public class NotificationProviderCollection : ProviderCollection
    {
        public new NotificationProvider this[string name]
        {
            get { return (NotificationProvider) base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is NotificationProvider))
                throw new ArgumentException("The provider parameter must be of type NotificationProvider.");

            base.Add(provider);
        }

        public void CopyTo(NotificationProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}