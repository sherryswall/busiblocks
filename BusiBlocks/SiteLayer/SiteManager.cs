using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Configuration;
using NHibernate;

namespace BusiBlocks.SiteLayer
{
    public static class SiteManager
    {
        //Public feature API
        private static SiteProviderCollection providerCollection = InitialiseProviderCollection();

        private static SiteProvider defaultProvider
        {
            get
            {
                var ac =
                    (SiteProviderConfiguration) ConfigurationManager.GetSection("siteManager");

                if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                    throw new ProviderException("You must specify a valid default provider for siteManager.");

                return providerCollection[ac.DefaultProvider];
            }
        }

        public static SiteProvider Provider
        {
            get { return defaultProvider; }
        }

        public static SiteProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static SiteProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (SiteProviderConfiguration) ConfigurationManager.GetSection("siteManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for siteManager.");

            //Instantiate the providers
            providerCollection = new SiteProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof (SiteProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Site> GetAllSites()
        {
            return Provider.GetAllSites();
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<SiteType> GetAllSiteTypes()
        {
            return Provider.GetAllSiteTypes();
        }

        public static IList<Site> GetSitesByUser(string userName, bool isAssigned)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return Provider.GetSitesByUser(userName, isAssigned);
        }

        public static void CreateSite(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            Provider.CreateSite(site);
        }

        public static void CreateRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            Provider.CreateRegion(region);
        }

        public static Site GetSiteById(string siteId)
        {
            if (string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException("siteId");

            return Provider.GetSiteById(siteId);
        }

        public static Site GetSiteByName(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new ArgumentNullException("siteName");

            return Provider.GetSiteByName(siteName);
        }

        public static Site GetSiteByLikeName(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new ArgumentNullException("siteName");

            return Provider.GetSiteByLikeName(siteName);
        }


        public static IList<Site> GetAllSitesByRegion(Region region, bool recursive)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            return Provider.GetAllSitesByRegion(region, recursive);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Region> GetAllRegions()
        {
            return Provider.GetAllRegions();
        }

        public static IList<Region> GetAllRegionsBelow(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            return Provider.GetAllRegionsBelow(region);
        }

        public static IList<Region> GetAllRegionsByRegionType(RegionType regionType)
        {
            if (regionType == null)
                throw new ArgumentNullException("regionType");

            return Provider.GetAllRegionsByRegionType(regionType);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<RegionType> GetAllRegionTypes()
        {
            return Provider.GetAllRegionTypes();
        }

        public static IList<RegionType> GetAllRegionTypesBelow(RegionType regionType)
        {
            if (regionType == null)
                throw new ArgumentNullException("regionType");

            return Provider.GetAllRegionTypesBelow(regionType);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static Region GetTopLevelRegion()
        {
            return Provider.GetTopLevelRegion();
        }

        public static Region GetRegionByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Provider.GetRegionByName(name);
        }
        
        public static IList<Region> GetAllRegionsByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Provider.GetAllRegionsByName(name);
        }

        public static Region GetRegionById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            return Provider.GetRegionById(id);
        }

        public static void UpdateSite(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            Provider.UpdateSite(site);
        }

        public static void UpdateRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            Provider.UpdateRegion(region);
        }

        public static void DeleteSite(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            Provider.DeleteSite(site);
        }

        public static void DeleteRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            Provider.DeleteRegion(region);
        }

        public static IList<Site> SearchSitesBySiteName(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new ArgumentNullException("siteName");

            return Provider.GetSitesBySiteName(siteName);
        }

        public static IList<Site> SearchSitesByRegionName(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException("regionName");

            return Provider.GetSitesByRegionName(regionName);
        }

        public static IList<Site> SearchSitesByRegionType(RegionType regionType)
        {
            if (regionType == null)
                throw new ArgumentNullException("regionType");

            return Provider.GetSitesByRegionType(regionType);
        }

        public static bool IsRegionUnique(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            return Provider.IsRegionUnique(region);
        }

        /// <summary>
        /// Determine whether one of the parents of site, is in the regions list.
        /// </summary>
        /// <param name="site">The site to query on.</param>
        /// <param name="regions">The list of potential parent regions.</param>
        /// <returns>True if one of the regions is a parent region of the site.</returns>
        public static bool IsChildOf(Site site, IList<Region> regions)
        {
            Region parentRegion = site.Region;
            return IsChildOf(parentRegion, regions);
        }

        /// <summary>
        /// Determines if the region is a sub region of at least one region in the regions list.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="regions"></param>
        /// <returns></returns>
        public static bool IsChildOf(Region region, IList<Region> regions)
        {
            if (region == null)
                return false;

            int breaker = 50;
            while (region.ParentRegion != null && breaker > 0)
            {
                IEnumerable<int> exists = from x in regions
                                          where x.Id.Equals(region.ParentRegion.Id)
                                          select 1;
                if (exists.Count() > 0)
                {
                    return true;
                }
                else
                {
                    if (TestLazyInitialization(region))
                    {
                        region = region.ParentRegion;
                    }
                    else
                    {
                        region = GetRegionById(region.Id);
                        region = region.ParentRegion;
                    }
                }
                breaker--;
                if (TestLazyInitialization(region))
                {
                    region = GetRegionById(region.Id);
                }
            }
            return false;
        }

        /// <summary>
        /// Tests whether the object has been loaded by NHibernate, or not.
        /// This is preferable than modifying the NHibernate loading method.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private static bool TestLazyInitialization(Region region)
        {
            try
            {
                return (region.ParentRegion == null) ? true : true;
            }
            catch (LazyInitializationException)
            {
                return false;
            }
        }
    }
}