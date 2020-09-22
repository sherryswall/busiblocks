using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using BusiBlocks.AddressLayer;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;

namespace BusiBlocks.SiteLayer
{
    public class BusiBlocksSiteProvider : SiteProvider
    {
        private ConnectionParameters _configuration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksSiteProvider";

            base.Initialize(name, config);

            _providerName = name;
            _applicationName = ExtractConfigValue(config, "applicationName", ConnectionParameters.DefaultApp);
            //System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath

            string connName = ExtractConfigValue(config, "connectionStringName", null);
            _configuration = ConnectionParameters.Create(connName);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " +
                                                attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private static string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;

            config.Remove(key);
            return val;
        }

        public override IList<Site> GetAllSites()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new SiteDataStore(transaction);
                return store.FindAll();
            }
        }

        public override IList<SiteType> GetAllSiteTypes()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new SiteTypeDataStore(transaction);
                return store.FindAll();
            }
        }

        public override IList<Site> GetAllSitesByRegion(Region region, bool recursive)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                IList<Site> sites = sDs.FindAllByRegion(region.Id);
                if (recursive)
                {
                    // Find children of this parent region.
                    var rDs = new RegionDataStore(transaction);
                    IList<Region> childRegions = rDs.FindByParentRegion(region.Id);
                    foreach (Region subRegion in childRegions)
                    {
                        IList<Site> subSites = GetAllSitesByRegion(subRegion, true);
                        foreach (Site subSite in subSites)
                        {
                            sites.Add(subSite);
                        }
                    }
                }
                return sites;
            }
        }

        public override IList<Region> GetAllRegions()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                IList<Region> regions = store.FindAll();
                // Order by breadcrumb.
                IEnumerable<Region> ordered = regions.OrderBy(x => x.Breadcrumb);
                return ordered.ToList();
            }
        }

        public override IList<RegionType> GetAllRegionTypes()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionTypeDataStore(transaction);
                return store.FindAll();
            }
        }

        public override IList<Region> GetAllRegionsBelow(Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                return store.FindAllBelow(region);
            }
        }

        public override IList<RegionType> GetAllRegionTypesBelow(RegionType regionType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionTypeDataStore(transaction);
                return store.FindAllBelow(regionType);
            }
        }

        public override Region GetTopLevelRegion()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                IList<Region> regions = store.FindByParentRegion(string.Empty);
                if (regions.Count == 0)
                    return null;
                return regions.First();
            }
        }

        public override Region GetRegionByName(string name)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                return store.FindByName(name);
            }
        }
                
        public override IList<Region> GetAllRegionsByName(string name)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                return store.FindAllByName(name);
            }
        }

        public override IList<Region> GetAllRegionsByRegionType(RegionType regionType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDs = new RegionDataStore(transaction);
                return rDs.FindAllByRegionType(regionType);
            }
        }

        public override Region GetRegionById(string id)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var store = new RegionDataStore(transaction);
                return store.FindByKey(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isAssigned">A site can be not assigned to a user, but still exists as a record, 
        /// this flag can suppress those that are not assigned.</param>
        /// <returns></returns>
        public override IList<Site> GetSitesByUser(string userName, bool isAssigned)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                // Find all the Persons who match the MembershipUser id of 'username'.
                var userDs = new UserDataStore(transaction);
                User user = userDs.FindByName(_applicationName, userName);

                // Then, find all the sites via PersonSite that match the personIds.
                IList<Site> runningList = new List<Site>();
                var ppDs = new PersonSiteDataStore(transaction);
                if (user.Person != null)
                {
                    IList<PersonSite> pps = ppDs.FindSitesByPerson(user.Person.Id, isAssigned);
                    foreach (PersonSite pSite in pps)
                    {
                        IEnumerable<Site> q = from x in runningList where x.Id.Equals(pSite.Site.Id) select x;
                        if (!q.Any())
                        {
                            runningList.Add(pSite.Site);
                        }
                    }
                    return runningList;
                }
            }
            return new List<Site>();
        }

        public override void CreateSite(Site site)
        {
            if (site.Region == null)
                throw new ArgumentNullException("site", "Region is null");
            if (site.SiteType == null)
                throw new ArgumentNullException("site", "SiteType is null");
            if (string.IsNullOrEmpty(site.Name))
                throw new ProviderException("Site name cannot be empty or null.");
            if (site.Name.IndexOf(',') > 0)
                throw new ArgumentException("Site names cannot contain commas.");
            if (GetSiteByName(site.Name) != null)
                throw new ProviderException("Site name already exists.");

            using (var transaction = new TransactionScope(_configuration))
            {
                if (!RegionExists(site.Region))
                    CreateRegion(site.Region);
                else
                    site.Region = GetRegionByName(site.Region.Name);

                if (!SiteTypeExists(site.SiteType))
                    CreateSiteType(site.SiteType);
                else
                    site.SiteType = GetSiteTypeByName(site.SiteType.Name);

                //if (site.PhysicalAddress == null || string.IsNullOrEmpty(site.PhysicalAddress.Address1))
                //    site.PhysicalAddress = new Address.Address();
                //if (site.PostalAddress == null || string.IsNullOrEmpty(site.PostalAddress.Address1))
                //    site.PostalAddress = new Address.Address();
                if (site.PhysicalAddress != null)
                {
                    if (string.IsNullOrEmpty(site.PhysicalAddress.Id))
                    {
                        if (!string.IsNullOrEmpty(site.PhysicalAddress.Address1))
                        {
                            Address currentAddress = AddressManager.GetAddressByDetails(site.PhysicalAddress.Address1,
                                                                                        site.PhysicalAddress.Address2,
                                                                                        site.PhysicalAddress.Suburb,
                                                                                        site.PhysicalAddress.Postcode,
                                                                                        site.PhysicalAddress.State);
                            if (currentAddress == null)
                            {
                                AddressManager.CreateAddress(site.PhysicalAddress);
                            }
                            else
                            {
                                site.PhysicalAddress = currentAddress;
                            }
                        }
                    }
                }
                if (site.PostalAddress != null)
                {
                    if (string.IsNullOrEmpty(site.PostalAddress.Id))
                    {
                        if (!string.IsNullOrEmpty(site.PostalAddress.Address1))
                        {
                            Address currentAddress = AddressManager.GetAddressByDetails(site.PostalAddress.Address1,
                                                                                        site.PostalAddress.Address2,
                                                                                        site.PostalAddress.Suburb,
                                                                                        site.PostalAddress.Postcode,
                                                                                        site.PostalAddress.State);
                            if (currentAddress == null)
                            {
                                AddressManager.CreateAddress(site.PostalAddress);
                            }
                            else
                            {
                                site.PostalAddress = currentAddress;
                            }
                        }
                    }
                }

                var ptDs = new SiteDataStore(transaction);
                ptDs.Insert(site);
                transaction.Commit();
            }
        }

        public override void CreateRegion(Region region)
        {
            if (region.RegionType == null)
                throw new ArgumentNullException("region", "RegionType is null");
            if (string.IsNullOrEmpty(region.Name))
                throw new ProviderException("Region name cannot be empty or null.");
            if (region.Name.IndexOf(',') > 0)
                throw new ArgumentException("Region names cannot contain commas.", "region");

            using (var transaction = new TransactionScope(_configuration))
            {
                if (!RegionTypeExists(region.RegionType))
                    CreateRegionType(region.RegionType);

                var rDs = new RegionDataStore(transaction);
                rDs.Insert(region);
                transaction.Commit();
            }
        }

        public override void CreateRegionType(RegionType regionType)
        {
            if (string.IsNullOrEmpty(regionType.Name))
                throw new ProviderException("Region Type name cannot be empty or null.");
            if (regionType.Name.IndexOf(',') > 0)
                throw new ArgumentException("Region Type names cannot contain commas.");

            using (var transaction = new TransactionScope(_configuration))
            {
                var rtDs = new RegionTypeDataStore(transaction);
                rtDs.Insert(regionType);
                transaction.Commit();
            }
        }

        public override void CreateSiteType(SiteType siteType)
        {
            if (string.IsNullOrEmpty(siteType.Name))
                throw new ProviderException("Site Type name cannot be empty or null.");
            if (siteType.Name.IndexOf(',') > 0)
                throw new ArgumentException("Site Type names cannot contain commas.");

            using (var transaction = new TransactionScope(_configuration))
            {
                var stDs = new SiteTypeDataStore(transaction);
                stDs.Insert(siteType);
                transaction.Commit();
            }
        }

        public override bool SiteExists(string name, string regionId, string siteTypeId, string addressId)
        {
            throw new NotImplementedException();
            //using (TransactionScope transaction = new TransactionScope(_configuration))
            //{
            //    SiteDataStore sDs = new SiteDataStore(transaction);
            //    if (sDs.FindByName(siteName) != null)
            //        return true;
            //    else
            //        return false;
            //}
        }

        public override bool IsRegionUnique(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            bool unique = false;

            // A region must be unique within an parent region.
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new RegionDataStore(transaction);
                IList<Region> siblings = ds.FindByParentRegion(region.ParentRegion.Id);
                Region match = siblings.FirstOrDefault(x => x.Name.Equals(region.Name));
                if (match == null)
                    unique = true;
            }
            return unique;
        }

        public override bool RegionExists(Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDs = new RegionDataStore(transaction);
                Region r = rDs.FindByName(region.Name);

                if (r != null)
                    return true;
            }
            return false;
        }

        public override bool RegionTypeExists(RegionType regionType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rtDs = new RegionTypeDataStore(transaction);
                RegionType rt = rtDs.FindByName(regionType.Name);

                if (rt != null)
                    return true;
            }
            return false;
        }

        public override bool SiteTypeExists(SiteType siteType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteTypeDataStore(transaction);
                SiteType st = sDs.FindByName(siteType.Name);

                if (st != null)
                    return true;
            }

            return false;
        }

        public override SiteType GetSiteTypeByName(string siteTypeName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteTypeDataStore(transaction);
                return sDs.FindByName(siteTypeName);
            }
        }

        public override Site GetSiteById(string siteId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                Site site = sDs.FindByKey(siteId);

                return (site.Deleted ? null : site);
            }
        }

        public override Site GetSiteByName(string siteName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                IList<Site> sites = sDs.FindAllByName(siteName);
                return sites.SingleOrDefault(x => x.Name.Equals(siteName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public override Site GetSiteByLikeName(string siteName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                return sDs.FindByName(siteName);
            }
        }

        public override void UpdateSite(Site site)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                sDs.Update(site);
                transaction.Commit();
            }
        }

        public override void UpdateRegion(Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDs = new RegionDataStore(transaction);
                rDs.Update(region);
                transaction.Commit();
            }
        }

        public override void DeleteSite(Site site)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                site.Deleted = true;
                site.Name += DateTimeHelper.GetCurrentTimestamp();
                sDs.Update(site);
                transaction.Commit();
            }
        }

        public override void DeleteRegion(Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDs = new RegionDataStore(transaction);
                region.Deleted = true;
                region.Name += DateTimeHelper.GetCurrentTimestamp();

                DeleteSubRegion(region);

                rDs.Update(region);
                transaction.Commit();
            }
        }
        public void DeleteSubRegion(Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDs = new RegionDataStore(transaction);
                IList<Region> children = rDs.FindAllBelow(region);
                foreach (Region child in children)
                {
                    child.Deleted = true;
                    child.Name += DateTimeHelper.GetCurrentTimestamp();
                    rDs.Update(child);
                }
                transaction.Commit();
            }
        }

        public override IList<Site> GetSitesBySiteName(string siteName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var sDs = new SiteDataStore(transaction);
                IList<Site> sites = sDs.FindAllByName(siteName);
                return sites;
            }
        }

        public override IList<Site> GetSitesByRegionName(string regionName)
        {
            IList<Region> regions = GetAllRegionsByName(regionName);
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<Site> sites = null;
                foreach (Region region in regions)
                {
                    var sDs = new SiteDataStore(transaction);
                    IList<Site> subSites = sDs.FindAllByRegion(region.Id);
                    if (sites == null)
                        sites = subSites;
                    else
                    {
                        foreach (Site site in subSites)
                            sites.Add(site);
                    }
                }
                return sites;
            }
        }

        public override IList<Site> GetSitesByRegionType(RegionType regionType)
        {
            IList<Region> regions = GetAllRegionsByRegionType(regionType);
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<Site> sites = null;
                foreach (Region region in regions)
                {
                    var sDs = new SiteDataStore(transaction);
                    IList<Site> subSites = sDs.FindAllByRegion(region.Id);
                    if (sites == null)
                        sites = subSites;
                    else
                    {
                        foreach (Site site in subSites)
                            sites.Add(site);
                    }
                }
                return sites;
            }
        }

        #region Properties

        private string _applicationName;
        private string _providerName;

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        #endregion
    }
}