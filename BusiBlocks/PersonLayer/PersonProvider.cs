using System.Collections.Generic;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;
using BusiBlocks.Membership;
using BusiBlocks.SiteLayer;

namespace BusiBlocks.PersonLayer
{
    public abstract class PersonProvider : ProviderBase
    {
        public abstract void CreatePerson(Person person, User user);

        public abstract void CreatePersonType(PersonType personType);

        public abstract void UpdatePersonType(PersonType personType);

        public abstract void CreatePersonTypeRole(PersonTypeRole personTypeRole);

        public abstract void CreatePersonPersonType(string personId, string personTypeId);

        public abstract void CreatePersonSite(string personId, string siteId, bool? isAdministrator, bool? isManager,
                                              bool? isAssigned, bool isDefault);

        public abstract void CreatePersonRegion(string personId, string regionId, bool? isAdministrator, bool? isManager);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<Person> GetAllPersons();

        public abstract IList<Person> GetAllPersonsInRegion(Region region, bool recursive);

        public abstract IList<Person> GetAllPersonsInSite(Site site);

        public abstract IList<Person> GetAllPersonsByPersonType(PersonType personType);

        public abstract IList<PersonType> GetPersonTypesByPerson(Person person);

        public abstract IList<Region> GetAdminRegionsByPerson(Person person, bool recursive);

        public abstract IList<Region> GetManagerRegionsByPerson(Person person, bool recursive);

        public abstract IList<PersonType> GetPersonTypesByUser(string userName);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<PersonType> GetAllPersonTypes(bool includeAdmin);

        public abstract IList<Site> GetAdminSitesByPerson(Person person);

        public abstract IList<Site> GetManagerSitesByPerson(Person person);

        public abstract Site GetDefaultSiteByPerson(Person person);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract IList<PersonTypeRole> GetAllPersonTypeRoles();

        public abstract IList<PersonTypeRole> GetPersonTypeRolesByPersonType(PersonType personType);

        public abstract Person GetPersonById(string personId);

        public abstract Person GetPersonByUserId(string userId);

        public abstract Person GetPersonByUserName(string userName);

        public abstract PersonType GetPersonTypeById(string personTypeId);

        public abstract PersonType GetPersonTypeByName(string personTypeName);

        public abstract PersonRegion GetPersonRegionByPersonAndRegion(Person person, Region region);

        public abstract PersonSite GetPersonSiteByPersonAndSite(Person person, Site site, bool isAssigned);

        public abstract IList<Person> GetPersonsByLastName(string lastName);

        public abstract IList<Person> GetPersonsByFirstName(string firstName);

        public abstract IList<Person> GetPersonsByRegionName(string regionName, bool recursive);

        public abstract IList<Person> GetPersonsByRegionType(RegionType regionType);

        public abstract void UpdatePerson(Person person);

        public abstract void UpdatePersonRegion(PersonRegion personRegion);

        public abstract void UpdatePersonSite(PersonSite personSite);

        public abstract void UpdatePersonRegion(string personId, bool? isAdministrator, bool? isManager);

        public abstract void UpdatePersonSite(string personId, bool? isAdministrator, bool? isManager);

        public abstract void DeletePerson(string personId);

        public abstract void DeletePersonType(string personTypeId);

        public abstract void DeletePersonTypeRole(PersonTypeRole personTypeRole);

        public abstract void DeletePersonFromPersonType(string personId, string personTypeId);

        public abstract void DeletePersonFromSite(string personId, string siteId);

        public abstract void DeletePersonFromRegion(string personId, string regionId);

        public abstract bool IsUserInPersonType(string userName, string personTypeId);

        public abstract bool IsPersonTypeInRole(string personTypeId, string roleId);

        public abstract bool IsPersonInPersonType(Person person, PersonType personType);

        public abstract bool IsPersonInPersonTypeAdmin(Person person);

        public abstract bool IsPersonInPersonSite(Person person, Site site);

        public abstract bool IsPersonAdminOrManagerInPersonSite(Person person, Site site);

        public abstract bool IsPersonInPersonSite(string personId);

        public abstract bool IsPersonInPersonRegion(string personId);

        public abstract bool IsPersonInPersonType(string personId);

        public abstract bool PersonTypeExists(string personTypeName);

        public abstract bool PersonTypeHasRoles(string personTypeId);
    }
}