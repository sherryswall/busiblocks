using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using BusiBlocks.Membership;
using BusiBlocks.SiteLayer;
using System.Data;

namespace BusiBlocks.PersonLayer
{
    public class BusiBlocksPersonProvider : PersonProvider
    {
        private ConnectionParameters _configuration;

        #region Properties

        private string _applicationName;
        private string _providerName;

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        #endregion

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksPersonProvider";

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
            else
            {
                config.Remove(key);
                return val;
            }
        }

        private IList<string> GetAdminIds()
        {
            // First get all the system admin ids.
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds2 = new PersonTypeDataStore(transaction);
                PersonType sysAdminPersonType = ds2.FindByName(BusiBlocksConstants.AdministratorsGroup);

                var ds1 = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> sysAdmins = ds1.FindPersonByPersonType(sysAdminPersonType.Id);
                IList<string> sysAdminIds = sysAdmins.Select(x => x.Person.Id).ToList<string>();
                return sysAdminIds;
            }
        }

        public override IList<Person> GetAllPersons()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonDataStore(transaction);
                IList<string> sysAdmins = GetAdminIds();
                return ds.FindAll().Where(x => !sysAdmins.Contains(x.Id)).ToList<Person>();
            }
        }

        public override IList<Person> GetAllPersonsInRegion(Region region, bool recursive)
        {
            IList<Person> persons = new List<Person>();
            using (var transaction = new TransactionScope(_configuration))
            {
                // Find all the sites for this region, and sub regions.
                IList<Site> sites = SiteManager.GetAllSitesByRegion(region, recursive);
                foreach (Site site in sites)
                {
                    var ds = new PersonSiteDataStore(transaction);
                    IList<PersonSite> personSites = ds.FindPersonsBySite(site.Id, false);
                    IList<string> sysAdmins = GetAdminIds();
                    foreach (PersonSite personSite in personSites)
                    {
                        // Only consider if this person is assigned to this site.
                        if (personSite.IsAssigned)
                        {
                            if (persons.FirstOrDefault(x => x.Id.Equals(personSite.Person.Id)) == null)
                            {
                                if (!sysAdmins.Contains(personSite.Person.Id))
                                    persons.Add(personSite.Person);
                            }
                        }
                    }
                }
            }
            return persons;
        }

        public override IList<Person> GetAllPersonsInSite(Site site)
        {
            IList<Person> persons = new List<Person>();
            using (var transaction = new TransactionScope(_configuration))
            {
                // Find all the sites for this region, and sub regions.
                var ds = new PersonSiteDataStore(transaction);
                IList<PersonSite> personSites = ds.FindPersonsBySite(site.Id, false);
                IList<string> sysAdmins = GetAdminIds();
                foreach (PersonSite personSite in personSites)
                {
                    if (persons.FirstOrDefault(x => x.Id.Equals(personSite.Person.Id)) == null)
                    {
                        if (!sysAdmins.Contains(personSite.Person.Id))
                            persons.Add(personSite.Person);
                    }
                }
            }
            return persons;
        }

        public override IList<Person> GetAllPersonsByPersonType(PersonType personType)
        {
            IList<Person> personList = new List<Person>();
            using (var transaction = new TransactionScope(_configuration))
            {
                // Find all the persons in the person person type table.
                var ds = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> personPersonTypes = ds.FindPersonByPersonType(personType.Id);
                personList = personPersonTypes.Select(x => x.Person).ToList<Person>();
            }
            return personList;
        }

        public override Person GetPersonById(string personId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pDS = new PersonDataStore(transaction);
                return pDS.FindByKey(personId);
            }
        }

        public override IList<PersonType> GetPersonTypesByPerson(Person person)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<PersonType> runningList = new List<PersonType>();
                var ppDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> pps = ppDS.FindPersonTypesByPerson(person.Id);
                foreach (PersonPersonType ppt in pps)
                {
                    IEnumerable<PersonType> q = from x in runningList where x.Id.Equals(ppt.PersonType.Id) select x;
                    if (q.Count() == 0)
                    {
                        runningList.Add(ppt.PersonType);
                    }
                }
                return runningList;
            }
        }

        public override IList<Site> GetAdminSitesByPerson(Person person)
        {
            return GetSitesByPerson(person, true, false);
        }

        public override IList<Site> GetManagerSitesByPerson(Person person)
        {
            return GetSitesByPerson(person, false, true);
        }

        private IList<Site> GetSitesByPerson(Person person, bool isAdmin, bool isManager)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<Site> runningList = new List<Site>();
                var ppDS = new PersonSiteDataStore(transaction);
                IList<PersonSite> pps = ppDS.FindSitesByPerson(person.Id, false);
                foreach (PersonSite ppt in pps)
                {
                    if (isAdmin)
                        if (!ppt.IsAdministrator)
                            continue;
                    if (isManager)
                        if (!ppt.IsManager)
                            continue;

                    IEnumerable<Site> q = from x in runningList where x.Id.Equals(ppt.Site.Id) select x;
                    if (q.Count() == 0)
                    {
                        runningList.Add(ppt.Site);
                    }
                }
                return runningList;
            }
        }

        public override Site GetDefaultSiteByPerson(Person person)
        {
            using (TransactionScope transaction = new TransactionScope(_configuration))
            {
                PersonSiteDataStore ppDS = new PersonSiteDataStore(transaction);
                IList<PersonSite> pps = ppDS.FindSitesByPerson(person.Id, true);
                PersonSite defaultSite = pps.FirstOrDefault(x => x.IsDefault);
                if (defaultSite != null)
                    return defaultSite.Site;
                else return null;
            }
        }

        public override IList<PersonType> GetPersonTypesByUser(string userName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                // Find all the Persons who match the MembershipUser id of 'username'.
                var userDS = new UserDataStore(transaction);
                User user = userDS.FindByName(_applicationName, userName);

                // If there is no matching person, then do not continue.
                if (user.Person == null)
                    throw new PersonNotFoundException(user.Name);

                // Then, find all the PersonTypes via PersonPersonType that match the personIds.
                IList<PersonType> runningList = new List<PersonType>();
                var ppDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> pps = ppDS.FindPersonTypesByPerson(user.Person.Id);
                foreach (PersonPersonType ppt in pps)
                {
                    IEnumerable<PersonType> q = from x in runningList where x.Id.Equals(ppt.PersonType.Id) select x;
                    if (q.Count() == 0)
                    {
                        runningList.Add(ppt.PersonType);
                    }
                }
                return runningList;
            }
        }

        public override IList<Region> GetAdminRegionsByPerson(Person person, bool recursive)
        {
            return GetRegionsByPerson(person, true, false, recursive);
        }

        public override IList<Region> GetManagerRegionsByPerson(Person person, bool recursive)
        {
            return GetRegionsByPerson(person, false, true, recursive);
        }

        private IList<Region> GetRegionsByPerson(Person person, bool isAdmin, bool isManager, bool recursive)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var runningList = new List<Region>();
                var ppDS = new PersonRegionDataStore(transaction);
                IList<PersonRegion> pps = ppDS.FindRegionsByPerson(person.Id);
                foreach (PersonRegion ppt in pps)
                {
                    if (isAdmin)
                        if (!ppt.IsAdministrator)
                            continue;
                    if (isManager)
                        if (!ppt.IsManager)
                            continue;

                    IEnumerable<Region> q = from x in runningList where x.Id.Equals(ppt.Region.Id) select x;
                    if (q.Count() == 0)
                    {
                        runningList.Add(ppt.Region);
                    }
                }
                if (recursive)
                {
                    // Find the sub regions, and add them to the runningList.
                    var toConcat = new List<Region>();
                    foreach (Region region in runningList)
                    {
                        foreach (Region childRegion in SiteManager.GetAllRegionsBelow(region))
                        {
                            if (runningList.FirstOrDefault(x => x.Id.Equals(childRegion.Id)) == null)
                                toConcat.Add(childRegion);
                        }
                    }
                    runningList.AddRange(toConcat);
                }
                runningList.Sort(new KeyComparer<Region>(x => x.Breadcrumb));
                return runningList;
            }
        }

        public override bool IsUserInPersonType(string userName, string personTypeId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pptDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> peoples = pptDS.FindPersonByPersonType(personTypeId);

                // Find the person who corresponds to username.
                var uDS = new UserDataStore(transaction);
                User user = uDS.FindByName(_applicationName, userName);

                // If there is no matching person, then do not continue.
                if (user.Person == null)
                    throw new PersonNotFoundException(user.Name);

                IEnumerable<PersonPersonType> q = from x in peoples where x.Person.Id.Equals(user.Person.Id) select x;

                if (q.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonType(Person person, PersonType personType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pptDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> peoples = pptDS.FindPersonByPersonType(personType.Id);

                IEnumerable<PersonPersonType> q = from x in peoples where x.Person.Id.Equals(person.Id) select x;

                if (q.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonSite(Person person, Site site)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonSiteDataStore(transaction);
                IList<PersonSite> peoples = ds.FindSitesByPerson(person.Id, false);

                IEnumerable<PersonSite> q = from x in peoples where x.Site.Id.Equals(site.Id) select x;

                if (q.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonSite(string personId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonSiteDataStore(transaction);
                IList<PersonSite> peoples = ds.FindSitesByPerson(personId, false);

                if (peoples.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonAdminOrManagerInPersonSite(Person person, Site site)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonSiteDataStore(transaction);
                IList<PersonSite> peoples = ds.FindSitesByPerson(person.Id, false);

                IEnumerable<PersonSite> q = from x in peoples
                                            where x.Site.Id.Equals(site.Id) & (x.IsAdministrator || x.IsManager)
                                            select x;

                if (q.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonTypeInRole(string personTypeId, string roleId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonTypeRoleDataStore(transaction);
                IList<PersonTypeRole> peoples = ds.FindPersonTypeRolesByPersonType(personTypeId);
                IEnumerable<int> exists = from x in peoples
                                          where x.Role.Id.Equals(roleId)
                                          select 1;

                if (exists.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonType(string personId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pptDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> peoples = pptDS.FindPersonTypesByPerson(personId);

                if (peoples.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonTypeAdmin(Person person)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonTypeDataStore(transaction);
                PersonType adminPersonType = ds.FindByName(BusiBlocksConstants.AdministratorsGroup);

                if (adminPersonType == null)
                    return false;

                var pds = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> personTypes = pds.FindByPersonAndPersonType(person.Id, adminPersonType.Id);

                if (personTypes.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool IsPersonInPersonRegion(string personId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonRegionDataStore(transaction);
                IList<PersonRegion> peoples = ds.FindRegionsByPerson(personId);

                if (peoples.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Find all the person types.
        /// </summary>
        /// <returns></returns>
        public override IList<PersonType> GetAllPersonTypes(bool includeAdmin)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                IList<PersonType> list = ptDS.FindAll();
                // Remove the System Administrators group from the list.
                if (!includeAdmin)
                    list.Remove(list.FirstOrDefault(x => x.Name.Equals(BusiBlocksConstants.AdministratorsGroup)));
                return list;
            }
        }

        public override IList<PersonTypeRole> GetAllPersonTypeRoles()
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptrDS = new PersonTypeRoleDataStore(transaction);
                return ptrDS.FindAll();
            }
        }

        public override IList<PersonTypeRole> GetPersonTypeRolesByPersonType(PersonType personType)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptrDS = new PersonTypeRoleDataStore(transaction);
                return ptrDS.FindPersonTypeRolesByPersonType(personType.Id);
            }
        }

        public override bool PersonTypeExists(string personTypeName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                if (ptDS.FindByName(personTypeName) != null)
                    return true;
                else
                    return false;
            }
        }

        public override bool PersonTypeHasRoles(string personTypeId)
        {
            bool personHasRoles = false;
            using (var transaction = new TransactionScope(_configuration))
            {
                var rDS = new PersonTypeRoleDataStore(transaction);
                IList<PersonTypeRole> roles = rDS.FindPersonTypeRolesByPersonType(personTypeId);
                if (roles.Count > 0)
                    personHasRoles = true;
            }
            return personHasRoles;
        }

        public override Person GetPersonByUserId(string userId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var uDS = new UserDataStore(transaction);
                User user = uDS.FindByKey(userId);

                if (user != null)
                    return user.Person;
            }
            return null;
        }

        public override Person GetPersonByUserName(string userName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var uDS = new UserDataStore(transaction);
                User user = uDS.FindByName(_applicationName, userName);

                if (user != null)
                    return user.Person;
            }
            return null;
        }

        public override void CreatePerson(Person person, User user)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pDS = new PersonDataStore(transaction);

                user.Person = pDS.Insert(person);

                var uDS = new UserDataStore(transaction);
                
                uDS.Update(user);

                transaction.Commit();
            }
        }

        public override void DeletePerson(string personId)
        { 
            // If person regions exist.
            if (IsPersonInPersonRegion(personId))
                throw new SchemaIntegrityException("Unable to delete. Person exists in a Region");

            // If person persontypes exist.
            if (IsPersonInPersonType(personId))
                throw new SchemaIntegrityException("Unable to delete. Person exists in a PersonType");

            // If person personsite exist.
            if (IsPersonInPersonSite(personId))
                throw new SchemaIntegrityException("Unable to delete. Person exists at a Site");

            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonDataStore(transaction);
                Person person = ptDS.FindByKey(personId);
                User user = MembershipManager.GetUserByPerson(person);
                System.Web.Security.Membership.DeleteUser(user.Name);
                person.Deleted = true;
                ptDS.Update(person);
                transaction.Commit();
            }
        }

        public override void CreatePersonType(PersonType personType)
        {
            //Check required for MSDN
            if (string.IsNullOrEmpty(personType.Name))
                throw new ProviderException("PersonType name cannot be empty or null.");
            if (personType.Name.IndexOf(',') > 0)
                throw new ArgumentException("PersonType names cannot contain commas.");
            if (PersonTypeExists(personType.Name))
                throw new ProviderException("PersonType name already exists.");

            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                ptDS.Insert(personType);
                transaction.Commit();
            }
        }

        public override void UpdatePersonType(PersonType personType)
        {
            //Check required for MSDN
            if (string.IsNullOrEmpty(personType.Name))
                throw new ProviderException("PersonType name cannot be empty or null.");
            if (personType.Name.IndexOf(',') > 0)
                throw new ArgumentException("PersonType names cannot contain commas.");            

            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                ptDS.Update(personType);
                transaction.Commit();
            }
        }

        public override void DeletePersonType(string personTypeId)
        {
            if (PersonTypeHasRoles(personTypeId))
                throw new ProviderException("Unable to delete. PersonType has Roles assigned to it");

            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                PersonType personType = ptDS.FindByKey(personTypeId);
                personType.Deleted = true;
                personType.Name += DateTimeHelper.GetCurrentTimestamp();
                ptDS.Update(personType);
                transaction.Commit();
            }
        }

        public override void CreatePersonTypeRole(PersonTypeRole personTypeRole)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptrDS = new PersonTypeRoleDataStore(transaction);
                ptrDS.Insert(personTypeRole);
                transaction.Commit();
            }
        }

        public override void DeletePersonTypeRole(PersonTypeRole personTypeRole)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptrDS = new PersonTypeRoleDataStore(transaction);
                personTypeRole.Deleted = true;
                ptrDS.Update(personTypeRole);
                transaction.Commit();
            }
        }

        public override void UpdatePerson(Person person)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pDS = new PersonDataStore(transaction);
                pDS.Update(person);
                transaction.Commit();
            }
        }

        public override void CreatePersonPersonType(string personId, string personTypeId)
        {
            // Create a new record in the PersonPersonType table.
            using (var transaction = new TransactionScope(_configuration))
            {
                var pptDS = new PersonPersonTypeDataStore(transaction);
                var ppt = new PersonPersonType();
                var pDS = new PersonDataStore(transaction);
                ppt.Person = pDS.FindByKey(personId);
                var ptDS = new PersonTypeDataStore(transaction);
                ppt.PersonType = ptDS.FindByKey(personTypeId);
                pptDS.Insert(ppt);
                transaction.Commit();
            }
        }

        public override void DeletePersonFromPersonType(string personId, string personTypeId)
        {
            // Delete a record in the PersonPersonType table.
            using (var transaction = new TransactionScope(_configuration))
            {
                var pptDS = new PersonPersonTypeDataStore(transaction);
                IList<PersonPersonType> ppts = pptDS.FindByPersonAndPersonType(personId, personTypeId);

                foreach (PersonPersonType item in ppts)
                {
                    item.Deleted = true;
                    pptDS.Update(item);
                }

                transaction.Commit();
            }
        }

        public override void CreatePersonSite(string personId, string siteId, bool? isAdministrator, bool? isManager,
                                              bool? isAssigned, bool isDefault)
        {
            // Create a new record in the PersonSite table.
            using (var transaction = new TransactionScope(_configuration))
            {
                var psDS = new PersonSiteDataStore(transaction);
                var ps = new PersonSite();
                var pDS = new PersonDataStore(transaction);
                ps.Person = pDS.FindByKey(personId);
                var sDS = new SiteDataStore(transaction);
                ps.Site = sDS.FindByKey(siteId);
                if (isAdministrator != null)
                    ps.IsAdministrator = (bool) isAdministrator;
                if (isManager != null)
                    ps.IsManager = (bool) isManager;
                if (isAssigned != null)
                    ps.IsAssigned = (bool) isAssigned;
                ps.IsDefault = isDefault;
                psDS.Insert(ps);

                transaction.Commit();
            }
        }

        public override void DeletePersonFromSite(string personId, string siteId)
        {
            // Delete a record in the PersonSite table.
            using (var transaction = new TransactionScope(_configuration))
            {
                var psDS = new PersonSiteDataStore(transaction);
                IList<PersonSite> pss = psDS.FindByPersonAndSite(personId, siteId, false);

                foreach (PersonSite item in pss)
                {
                    item.Deleted = true;
                    psDS.Update(item);
                }

                transaction.Commit();
            }
        }

        public override void CreatePersonRegion(string personId, string regionId, bool? isAdministrator, bool? isManager)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var psDS = new PersonRegionDataStore(transaction);
                var pr = new PersonRegion();
                var pDS = new PersonDataStore(transaction);
                pr.Person = pDS.FindByKey(personId);
                var rDS = new RegionDataStore(transaction);
                pr.Region = rDS.FindByKey(regionId);
                if (isAdministrator != null)
                    pr.IsAdministrator = (bool) isAdministrator;
                if (isManager != null)
                    pr.IsManager = (bool) isManager;
                psDS.Insert(pr);

                transaction.Commit();
            }
        }

        public override void UpdatePersonRegion(PersonRegion personRegion)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var prDS = new PersonRegionDataStore(transaction);
                prDS.Update(personRegion);
                transaction.Commit();
            }
        }

        public override void UpdatePersonSite(PersonSite personSite)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var prDS = new PersonSiteDataStore(transaction);
                prDS.Update(personSite);
                transaction.Commit();
            }
        }

        /// <summary>
        /// Modifies the PersonRegion record. 
        /// If one of personId or regionId are null, then it is implied that all records for the not null item are to be updated.
        /// If one of isAdministrator or isManager are null, then it is implied that the not null item is to be updated.
        /// If a record is updated so that both isAdministrator and isManager are false, then that record is deleted.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="regionId"></param>
        /// <param name="isAdministrator"></param>
        /// <param name="isManager"></param>
        public override void UpdatePersonRegion(string personId, bool? isAdministrator, bool? isManager)
        {
            if (isAdministrator == null && isManager == null)
                throw new ArgumentNullException("isAdministrator", "Both isAdministrator and isManager cannot be null");

            using (var transaction = new TransactionScope(_configuration))
            {
                var prDS = new PersonRegionDataStore(transaction);
                IList<PersonRegion> prs = prDS.FindRegionsByPerson(personId);

                foreach (PersonRegion pr in prs)
                {
                    // A value of null for isAdministrator and isManager is equivalent to false.
                    if (!isAdministrator.HasValue)
                    {
                        // Only modify the manager values
                        pr.IsManager = (bool) isManager;

                        if (!pr.IsAdministrator && !pr.IsManager)
                            prDS.Delete(pr.Id);
                        else
                            prDS.Update(pr);
                    }
                    else if (!isManager.HasValue)
                    {
                        // Only modify the manager values
                        pr.IsAdministrator = (bool) isAdministrator;

                        if (!pr.IsAdministrator && !pr.IsManager)
                            prDS.Delete(pr.Id);
                        else
                            prDS.Update(pr);
                    }
                }
                transaction.Commit();
            }
        }

        public override void UpdatePersonSite(string personId, bool? isAdministrator, bool? isManager)
        {
            if (isAdministrator == null && isManager == null)
                throw new ArgumentNullException("isAdministrator", "Both isAdministrator and isManager cannot be null");

            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonSiteDataStore(transaction);
                IList<PersonSite> prs = ds.FindSitesByPerson(personId, false);

                foreach (PersonSite pr in prs)
                {
                    // A value of null for isAdministrator and isManager is equivalent to false.
                    if (!isAdministrator.HasValue)
                    {
                        // Only modify the manager values
                        pr.IsManager = (bool) isManager;

                        if (!pr.IsAdministrator && !pr.IsManager)
                            ds.Delete(pr.Id);
                        else
                            ds.Update(pr);
                    }
                    else if (!isManager.HasValue)
                    {
                        // Only modify the manager values
                        pr.IsAdministrator = (bool) isAdministrator;

                        if (!pr.IsAdministrator && !pr.IsManager)
                            ds.Delete(pr.Id);
                        else
                            ds.Update(pr);
                    }
                }
                transaction.Commit();
            }
        }

        public override void DeletePersonFromRegion(string personId, string regionId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var prDS = new PersonRegionDataStore(transaction);
                IList<PersonRegion> prs = prDS.FindByPersonAndRegion(personId, regionId);

                foreach (PersonRegion item in prs)
                {
                    item.Deleted = true;
                    prDS.Update(item);
                }

                transaction.Commit();
            }
        }

        public override PersonType GetPersonTypeById(string personTypeId)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                return ptDS.FindByKey(personTypeId);
            }
        }

        public override PersonType GetPersonTypeByName(string personTypeName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ptDS = new PersonTypeDataStore(transaction);
                return ptDS.FindByName(personTypeName);
            }
        }

        public override PersonRegion GetPersonRegionByPersonAndRegion(Person person, Region region)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var prDS = new PersonRegionDataStore(transaction);
                return prDS.FindByPersonAndRegion(person.Id, region.Id).FirstOrDefault();
            }
        }

        public override PersonSite GetPersonSiteByPersonAndSite(Person person, Site site, bool isAssigned)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var ds = new PersonSiteDataStore(transaction);
                return ds.FindByPersonAndSite(person.Id, site.Id, isAssigned).FirstOrDefault();
            }
        }

        public override IList<Person> GetPersonsByLastName(string lastName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pDS = new PersonDataStore(transaction);
                return pDS.FindAllByLastName(lastName);
            }
        }

        public override IList<Person> GetPersonsByFirstName(string firstName)
        {
            using (var transaction = new TransactionScope(_configuration))
            {
                var pDS = new PersonDataStore(transaction);
                return pDS.FindAllByFirstName(firstName);
            }
        }

        public override IList<Person> GetPersonsByRegionName(string regionName, bool recursive)
        {
            IList<Region> regions = SiteManager.GetAllRegionsByName(regionName);
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<Person> persons = new List<Person>();
                foreach (Region region in regions)
                {
                    IList<Site> sites = SiteManager.GetAllSitesByRegion(region, recursive);

                    // Find all persons at these sites.
                    var psDS = new PersonSiteDataStore(transaction);
                    foreach (Site site in sites)
                    {
                        IList<PersonSite> personAtSites = psDS.FindPersonsBySite(site.Id, false);
                        if (persons == null)
                        {
                            IEnumerable<Person> ppl =
                                from x in personAtSites
                                select x.Person;

                            persons = ppl.ToList<Person>();
                        }
                        else
                        {
                            foreach (PersonSite personSite in personAtSites)
                                persons.Add(personSite.Person);
                        }
                    }

                    if (recursive)
                    {
                        // Find all child regions.
                        var rDS = new RegionDataStore(transaction);
                        IList<Region> childRegions = rDS.FindByParentRegion(region.Id);
                        foreach (Region subRegion in childRegions)
                        {
                            IList<Person> subPersons = GetPersonsByRegionName(subRegion.Name, recursive);
                            foreach (Person subPerson in subPersons)
                            {
                                persons.Add(subPerson);
                            }
                        }
                    }
                }
                return persons;
            }
        }

        public override IList<Person> GetPersonsByRegionType(RegionType regionType)
        {
            IList<Region> regions = SiteManager.GetAllRegionsByRegionType(regionType);
            using (var transaction = new TransactionScope(_configuration))
            {
                IList<Person> persons = new List<Person>();
                foreach (Region region in regions)
                {
                    IList<Site> sites = SiteManager.GetAllSitesByRegion(region, false);

                    // Find all persons at these sites.
                    var psDS = new PersonSiteDataStore(transaction);
                    foreach (Site site in sites)
                    {
                        IList<PersonSite> personAtSites = psDS.FindPersonsBySite(site.Id, false);
                        if (persons == null)
                        {
                            IEnumerable<Person> ppl =
                                from x in personAtSites
                                select x.Person;

                            persons = ppl.ToList<Person>();
                        }
                        else
                        {
                            foreach (PersonSite personSite in personAtSites)
                                persons.Add(personSite.Person);
                        }
                    }
                }
                return persons;
            }
        }
    }
}