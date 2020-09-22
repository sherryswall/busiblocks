using System.Configuration;

namespace BusiBlocks.Configuration
{
    public class AssemblyMappingElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string) this["assembly"]; }
            set { this["assembly"] = value; }
        }
    }

    public class AssemblyMappingCollection : ConfigurationElementCollection
    {
        public AssemblyMappingElement this[int index]
        {
            get { return base.BaseGet(index) as AssemblyMappingElement; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyMappingElement) element).Assembly;
        }
    }
}