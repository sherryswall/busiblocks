using System.Web.UI.WebControls;
using NHibernate;

namespace BusiBlocks.SiteLayer
{
    public class RegionTreeNode : TreeNode
    {
        public RegionTreeNode(Region region)
        {
            Region = region;
            Text = region.Name;

            try
            {
                if (region.RegionType != null)
                    ToolTip = Text + " (" + region.RegionType.Name + ")";
                ImageToolTip = "region" + Region.Id;
            }
            catch (LazyInitializationException)
            {
            }
        }

        public Region Region { get; set; }
    }
}