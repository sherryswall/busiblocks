using System.Web.UI.WebControls;
using NHibernate;

namespace BusiBlocks.SiteLayer
{
    public class SiteTreeNode : TreeNode
    {
        public SiteTreeNode(Site site)
        {
            Site = site;
            Text = site.Name;

            try
            {
                if (site.SiteType != null)
                    ToolTip = Text + " (" + site.SiteType.Name + ")";
                ImageToolTip = "site" + Site.Id;
            }
            catch (LazyInitializationException)
            {
            }
        }

        public Site Site { get; set; }
    }
}