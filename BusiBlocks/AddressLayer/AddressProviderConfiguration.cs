﻿using System.Configuration;

namespace BusiBlocks.AddressLayer
{
    public class AddressProviderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection) base["providers"]; }
        }

        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get { return (string) base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }
}