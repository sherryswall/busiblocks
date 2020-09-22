namespace BusiBlocks.SiteLayer
{
    public class Region
    {
        private Region parentRegion;
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }

        public virtual Region ParentRegion
        {
            get { return parentRegion; }
            set
            {
                // This block of code is part of an infinite loop if there is a circular reference 
                // between regions and parentRegions.
                // todo fix it

                parentRegion = value;
                string val = Name;
                int breaker = 0;

                Region pRegion = parentRegion;
                while (pRegion != null && breaker < 30)
                {
                    // Sometimes, the only property loaded is the Id. So we have to retrieve the full object. 
                    // todo Figure out why only the Id is loaded.
                    if (string.IsNullOrEmpty(pRegion.Name))
                    {
                        // Warning, this will call the next regions 'setter', and potentially be a infinite loop if there
                        // is a circular reference.
                        Region theRegion = SiteManager.GetRegionById(pRegion.Id);
                        pRegion = theRegion;
                    }
                    val = pRegion.Name + @"\" + val;
                    breaker++;
                    pRegion = pRegion.ParentRegion;
                }
                Breadcrumb = val;
            }
        }

        public virtual RegionType RegionType { get; set; }
        public virtual string Breadcrumb { get; private set; }
        public virtual bool Deleted { get; set; }
    }
}