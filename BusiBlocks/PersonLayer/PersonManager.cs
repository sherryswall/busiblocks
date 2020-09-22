using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;
using System.Web.Security;
using BusiBlocks.AddressLayer;
using BusiBlocks.Membership;
using BusiBlocks.SiteLayer;

namespace BusiBlocks.PersonLayer
{
    public static class PersonManager
    {
        //Public feature API
        private static readonly PersonProviderCollection providerCollection = InitialiseProviderCollection();

        static PersonManager()
        {
        }

        private static PersonProvider defaultProvider
        {
            get
            {
                if (providerCollection != null)
                {
                    //Get the feature's configuration info
                    var ac =
                        (PersonProviderConfiguration)ConfigurationManager.GetSection("personManager");

                    if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                        throw new ProviderException("You must specify a valid default provider for personManager.");

                    return providerCollection[ac.DefaultProvider];
                }
                return null;
            }
        }

        public static PersonProvider Provider
        {
            get { return defaultProvider; }
        }

        public static PersonProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static PersonProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (PersonProviderConfiguration)ConfigurationManager.GetSection("personManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for personManager.");

            var providerCollection = new PersonProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(PersonProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<Person> GetAllPersons()
        {
            return Provider.GetAllPersons();
        }

        public static IList<Person> GetAllPersonsInRegion(Region region, bool recursive)
        {
            return Provider.GetAllPersonsInRegion(region, recursive);
        }

        public static IList<Person> GetAllPersonsInSite(Site site)
        {
            return Provider.GetAllPersonsInSite(site);
        }

        public static IList<Person> GetAllPersonsByPersonType(PersonType personType)
        {
            return Provider.GetAllPersonsByPersonType(personType);
        }

        public static IList<PersonType> GetPersonTypesByPerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetPersonTypesByPerson(person);
        }

        public static IList<PersonType> GetPersonTypesByUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return Provider.GetPersonTypesByUser(userName);
        }

        public static IList<Site> GetAdminSitesByPerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetAdminSitesByPerson(person);
        }

        public static IList<Site> GetManagerSitesByPerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetManagerSitesByPerson(person);
        }

        public static Site GetDefaultSiteByPerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetDefaultSiteByPerson(person);
        }

        public static IList<Region> GetAdminRegionsByPerson(Person person, bool recursive)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetAdminRegionsByPerson(person, recursive);
        }

        public static IList<Region> GetManagerRegionsByPerson(Person person, bool recursive)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetManagerRegionsByPerson(person, recursive);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<PersonType> GetAllPersonTypes(bool includeAdmin)
        {
            return Provider.GetAllPersonTypes(includeAdmin);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<PersonTypeRole> GetAllPersonTypeRoles()
        {
            return Provider.GetAllPersonTypeRoles();
        }

        public static IList<PersonTypeRole> GetPersonTypeRoleByPerson(PersonType personType)
        {
            if (personType == null)
                throw new ArgumentNullException("personType");

            return Provider.GetPersonTypeRolesByPersonType(personType);
        }

        public static bool IsUserInPersonType(string userName, string personTypeId)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");

            return Provider.IsUserInPersonType(userName, personTypeId);
        }

        public static bool IsPersonInPersonType(Person person, PersonType personType)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (personType == null)
                throw new ArgumentNullException("personType");
            if (string.IsNullOrEmpty(person.Id))
                throw new ArgumentNullException("person.Id");
            if (string.IsNullOrEmpty(personType.Id))
                throw new ArgumentNullException("personType.Id");

            return Provider.IsPersonInPersonType(person, personType);
        }

        public static bool IsPersonInPersonTypeAdmin(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.IsPersonInPersonTypeAdmin(person);
        }

        public static bool IsPersonInPersonSite(Person person, Site site)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (site == null)
                throw new ArgumentNullException("site");

            return Provider.IsPersonInPersonSite(person, site);
        }

        public static bool IsPersonAdminOrManagerInPersonSite(Person person, Site site)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (site == null)
                throw new ArgumentNullException("site");

            return Provider.IsPersonAdminOrManagerInPersonSite(person, site);
        }

        public static bool IsPersonInPersonSite(string personId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            return Provider.IsPersonInPersonSite(personId);
        }

        public static bool IsPersonTypeInRole(string personTypeId, string roleId)
        {
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException("roleId");

            return Provider.IsPersonTypeInRole(personTypeId, roleId);
        }

        public static bool IsPersonInPersonType(string personId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            return Provider.IsPersonInPersonType(personId);
        }

        public static bool IsPersonInPersonRegion(string personId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            return Provider.IsPersonInPersonRegion(personId);
        }

        public static Person GetPersonById(string personId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            return Provider.GetPersonById(personId);
        }

        public static Person GetPersonByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            return Provider.GetPersonByUserId(userId);
        }

        public static Person GetPersonByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return Provider.GetPersonByUserName(userName);
        }

        public static void CreatePerson(User user, string personName, Address address)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(personName))
                throw new ArgumentNullException("personName");
            if (address == null)
                throw new ArgumentNullException("address");

            var person = new Person();
            person.LastName = personName;
            person.FirstName = personName;
            person.Address = address;
            Provider.CreatePerson(person, user);
        }

        public static Person CreatePerson(User newUser, Person newPerson)
        {
            if (newUser == null)
                throw new ArgumentNullException("user");
            if (newPerson == null)
                throw new ArgumentNullException("address");
            if (string.IsNullOrEmpty(newPerson.FirstName))
                throw new ArgumentNullException("firstName");
            if (string.IsNullOrEmpty(newPerson.LastName))
                throw new ArgumentNullException("lastName");

            if (string.IsNullOrEmpty(newPerson.Address.Id))
            {
                if (string.IsNullOrEmpty(newPerson.Address.Address1))
                    newPerson.Address.Address1 = "-";
                AddressManager.CreateAddress(newPerson.Address);
                newPerson.Address = AddressManager.GetAddressByDetails(newPerson.Address.Address1, newPerson.Address.Address2, newPerson.Address.Suburb,
                                                             newPerson.Address.Postcode, newPerson.Address.State);
            }

            Person person = new Person();
            person.Address = newPerson.Address;
            person.Email = newPerson.Email;
            person.FirstName = newPerson.FirstName;
            person.LastName = newPerson.LastName;
            person.PhoneNumber = newPerson.PhoneNumber;
            person.Position = newPerson.Position;
            person.WorkFax = newPerson.WorkFax;
            person.WorkMobile = newPerson.WorkMobile;
            person.WorkPhone = newPerson.WorkPhone;

            Provider.CreatePerson(person, newUser);

            return GetPersonByUserName(newUser.Name);
        }

        public static void CreatePerson(string lastName, string firstName, Address address)
        {
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException("lastName");
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException("firstName");
            if (address == null)
                throw new ArgumentNullException("address");

            if (string.IsNullOrEmpty(address.Id))
            {
                AddressManager.CreateAddress(address);
                address = AddressManager.GetAddressByDetails(address.Address1, address.Address2, address.Suburb,
                                                             address.Postcode, address.State);
            }

            var person = new Person();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Address = address;

            // todo figure out a better default username.
            string defaultName = firstName + lastName;
            MembershipUser membershipUser = System.Web.Security.Membership.CreateUser(defaultName, defaultName);
            if (membershipUser.ProviderUserKey != null)
            {
                User user = MembershipManager.GetUser(membershipUser.ProviderUserKey.ToString());
                Provider.CreatePerson(person, user);
            }
        }

        public static void DeletePerson(string personId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            Provider.DeletePerson(personId);
        }

        public static void CreatePersonType(PersonType personType)
        {
            if (personType == null)
                throw new ArgumentNullException("personType");

            Provider.CreatePersonType(personType);
        }

        public static void UpdatePersonType(PersonType personType)
        {
            if (personType == null)
                throw new ArgumentNullException("personType");

            Provider.UpdatePersonType(personType);
        }

        public static void DeletePersonType(string personTypeId)
        {
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");

            Provider.DeletePersonType(personTypeId);
        }

        public static void CreatePersonTypeRole(PersonTypeRole personTypeRole)
        {
            if (personTypeRole == null)
                throw new ArgumentNullException("personTypeRole");

            Provider.CreatePersonTypeRole(personTypeRole);
        }

        public static void DeletePersonTypeRole(PersonTypeRole personTypeRole)
        {
            if (personTypeRole == null)
                throw new ArgumentNullException("personTypeRole");

            Provider.DeletePersonTypeRole(personTypeRole);
        }

        public static void UpdatePerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            Provider.UpdatePerson(person);
        }

        public static void AddPersonToPersonType(string personId, string personTypeId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");

            Provider.CreatePersonPersonType(personId, personTypeId);
        }

        public static void DeletePersonFromPersonType(string personId, string personTypeId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");

            Provider.DeletePersonFromPersonType(personId, personTypeId);
        }

        public static void AddPersonToSite(string personId, string siteId, bool? isAdministrator, bool? isManager,
                                           bool? isAssigned, bool isDefault)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException("siteId");

            Provider.CreatePersonSite(personId, siteId, isAdministrator, isManager, isAssigned, isDefault);
        }

        public static void DeletePersonFromSite(string personId, string siteId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException("siteId");

            Provider.DeletePersonFromSite(personId, siteId);
        }

        public static void AddPersonToRegion(string personId, string regionId, bool? isAdministrator, bool? isManager)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(regionId))
                throw new ArgumentNullException("regionId");

            Provider.CreatePersonRegion(personId, regionId, isAdministrator, isManager);
        }

        public static void UpdatePersonRegion(PersonRegion personRegion)
        {
            if (personRegion == null)
                throw new ArgumentNullException("personRegion");

            Provider.UpdatePersonRegion(personRegion);
        }

        public static void UpdatePersonSite(PersonSite personSite)
        {
            if (personSite == null)
                throw new ArgumentNullException("personSite");

            Provider.UpdatePersonSite(personSite);
        }

        public static void UpdatePersonRegion(string personId, bool? isAdministrator, bool? isManager)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            Provider.UpdatePersonRegion(personId, isAdministrator, isManager);
        }

        public static void UpdatePersonSite(string personId, bool? isAdministrator, bool? isManager)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");

            Provider.UpdatePersonSite(personId, isAdministrator, isManager);
        }

        public static void DeletePersonFromRegion(string personId, string regionId)
        {
            if (string.IsNullOrEmpty(personId))
                throw new ArgumentNullException("personId");
            if (string.IsNullOrEmpty(regionId))
                throw new ArgumentNullException("regionId");

            Provider.DeletePersonFromRegion(personId, regionId);
        }

        public static PersonType GetPersonTypeById(string personTypeId)
        {
            if (string.IsNullOrEmpty(personTypeId))
                throw new ArgumentNullException("personTypeId");

            return Provider.GetPersonTypeById(personTypeId);
        }

        public static PersonType GetPersonTypeByName(string personTypeName)
        {
            return Provider.GetPersonTypeById(personTypeName);
        }

        public static PersonRegion GetPersonRegionByPersonAndRegion(Person person, Region region)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (region == null)
                throw new ArgumentNullException("region");

            return Provider.GetPersonRegionByPersonAndRegion(person, region);
        }

        public static PersonSite GetPersonSiteByPersonAndSite(Person person, Site site, bool isAssigned)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (site == null)
                throw new ArgumentNullException("site");

            return Provider.GetPersonSiteByPersonAndSite(person, site, isAssigned);
        }

        public static IList<Person> SearchPersonsByLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException("lastName");

            return Provider.GetPersonsByLastName(lastName);
        }

        public static IList<Person> SearchPersonsByFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException("firstName");

            return Provider.GetPersonsByFirstName(firstName);
        }

        public static IList<Person> SearchPersonsByRegionName(string regionName, bool recursive)
        {
            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException("regionName");

            return Provider.GetPersonsByRegionName(regionName, recursive);
        }

        public static IList<Person> SearchPersonsByRegionType(RegionType regionType)
        {
            if (regionType == null)
                throw new ArgumentNullException("regionType");

            return Provider.GetPersonsByRegionType(regionType);
        }
    }
}