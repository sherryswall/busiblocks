using System.Collections.Generic;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;

namespace BusiBlocks.SiteLayer
{
    public abstract class SiteProvider : ProviderBase
    {
        public abstract void CreateSite(Site site);

        public abstract void CreateRegion(Region region);

        public abstract void CreateRegionType(RegionType regionType);

        public abstract void CreateSiteType(SiteType siteType);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<Site> GetAllSites();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<SiteType> GetAllSiteTypes();

        public abstract IList<Site> GetAllSitesByRegion(Region region, bool recursive);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<Region> GetAllRegions();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<RegionType> GetAllRegionTypes();

        public abstract IList<Region> GetAllRegionsBelow(Region region);

        public abstract IList<RegionType> GetAllRegionTypesBelow(RegionType regionType);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract Region GetTopLevelRegion();

        public abstract IList<Site> GetSitesByUser(string userName, bool isAssigned);

        public abstract Region GetRegionByName(string name);
        
        public abstract IList<Region> GetAllRegionsByName(string name);

        public abstract IList<Region> GetAllRegionsByRegionType(RegionType regionType);

        public abstract Region GetRegionById(string id);

        public abstract SiteType GetSiteTypeByName(string siteTypeName);

        public abstract Site GetSiteById(string siteId);

        public abstract Site GetSiteByName(string siteName);

        public abstract Site GetSiteByLikeName(string siteName);

        public abstract IList<Site> GetSitesBySiteName(string siteName);

        public abstract IList<Site> GetSitesByRegionName(string regionName);

        public abstract IList<Site> GetSitesByRegionType(RegionType regionType);

        public abstract void UpdateSite(Site site);

        public abstract void UpdateRegion(Region region);

        public abstract void DeleteSite(Site site);

        public abstract void DeleteRegion(Region region);

        public abstract bool SiteExists(string name, string regionId, string siteTypeId, string addressId);

        public abstract bool SiteTypeExists(SiteType siteType);

        public abstract bool IsRegionUnique(Region region);

        public abstract bool RegionExists(Region region);

        public abstract bool RegionTypeExists(RegionType regionType);
    }
}