// -----------------------------------------------------------------------
// <copyright file="RegionVisibilityHelper.cs" company="BusiBlocks">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

namespace BusiBlocks
{
    /// <summary>
    /// Filters data sets by the region visibility for a user.
    /// </summary>
    public static class RegionVisibilityHelper
    {
        public static IList<Person> FilterPersonList(string userName, IList<Person> persons)
        {
            IList<Person> retVal = new List<Person>();
            // Can the userPerson see this person?
            IList<Person> visiblePersons = GetPersonsForUser(userName);
            foreach (Person person in persons)
            {
                if (visiblePersons.FirstOrDefault(x => x.Id.Equals(person.Id)) != null)
                {
                    retVal.Add(person);
                }
            }
            return retVal;
        }

        public static IList<Person> GetPersonsForUser(string userName)
        {
            IList<Person> persons = new List<Person>();
            Person person = PersonManager.GetPersonByUserName(userName);

            // If the person is an administrator, allow everything.
            if (PersonManager.IsPersonInPersonTypeAdmin(person))
            {
                // Allow all.
                persons = PersonManager.GetAllPersons();
            }
            else
            {
                persons = GetPersonsForPerson(person);
            }
            return persons;
        }

        public static IList<Site> FilterSiteList(string userName, IList<Site> sites)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (sites == null)
                throw new ArgumentNullException("sites");

            IList<Site> retVal = new List<Site>();
            // Can the userPerson see this person?
            IList<Site> visibleSites = GetSitesForUser(userName);
            foreach (Site site in sites)
            {
                if (visibleSites.FirstOrDefault(x => x.Id.Equals(site.Id)) != null)
                {
                    retVal.Add(site);
                }
            }
            return retVal;
        }

        public static IList<Site> GetSitesForUser(string userName)
        {
            IList<Site> sites = new List<Site>();
            Person person = PersonManager.GetPersonByUserName(userName);

            // If the person is an administrator, allow everything.
            if (PersonManager.IsPersonInPersonTypeAdmin(person))
            {
                // Allow all.
                sites = SiteManager.GetAllSites();
            }
            else
            {
                // Allow those users who are in the sites which this person has visibility of.
                IList<Region> regions = PersonManager.GetAdminRegionsByPerson(person, false);
                foreach (Region region in regions)
                {
                    IList<Site> newSites = SiteManager.GetAllSitesByRegion(region, true);
                    // All the sites who fall under this region should be added to the list.
                    foreach (Site site in newSites)
                    {
                        if (sites.FirstOrDefault(x => x.Id.Equals(site.Id)) == null)
                        {
                            sites.Add(site);
                        }
                    }
                }
                // Find the sites.
                IList<Site> userSites = SiteManager.GetSitesByUser(userName, true);
                foreach (Site site in userSites)
                {
                    if (sites.FirstOrDefault(x => x.Id.Equals(site.Id)) == null)
                    {
                        sites.Add(site);
                    }
                }
            }
            return sites;
        }

        private static IList<Person> GetPersonsForPerson(Person person)
        {
            IList<Person> persons = new List<Person>();
            // Allow those users who are in the regions which this person has visibility of.
            IList<Region> regions = PersonManager.GetAdminRegionsByPerson(person, false);
            foreach (Region region in regions)
            {
                // All the people who fall under this region should be added to the list.
                IList<Person> personsInRegion = PersonManager.GetAllPersonsInRegion(region, true);
                foreach (Person newPerson in personsInRegion)
                {
                    if (persons.FirstOrDefault(x => x.Id.Equals(newPerson.Id)) == null)
                    {
                        persons.Add(newPerson);
                    }
                }
            }
            IList<Site> sites = PersonManager.GetAdminSitesByPerson(person);
            foreach (Site site in sites)
            {
                IList<Person> personsInSite = PersonManager.GetAllPersonsInSite(site);
                foreach (Person newPerson in personsInSite)
                {
                    if (persons.FirstOrDefault(x => x.Id.Equals(newPerson.Id)) == null)
                    {
                        persons.Add(newPerson);
                    }
                }
            }
            return persons;
        }
    }
}