using System.Configuration;

namespace BusiBlocks.Configuration
{
    /// <summary>
    /// The BusiBlocks conficuration section handler.
    /// Contains the list of assemblies to use for the mappings.
    /// </summary>
    public class BusiBlocksSection : ConfigurationSection
    {
        public static BusiBlocksSection Section
        {
            get
            {
                var section =
                    (BusiBlocksSection) ConfigurationManager.GetSection("busiblocks");

                return section;
            }
        }

        [ConfigurationProperty("mappings")]
        public AssemblyMappingCollection Mappings
        {
            get { return this["mappings"] as AssemblyMappingCollection; }
        }
    }
}