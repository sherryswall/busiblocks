using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using BusiBlocks.AccessLayer;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

namespace BusiBlocks
{
    /// <summary>
    /// Static class with some helper methods to check security properties of an entity.
    /// This class is used to check the permissions defined in the entity category. 
    /// See the MatchPermissions method for informations about the syntax.
    /// </summary>
    public class SecurityHelper
    {
        public const string ALL_USERS = "?";
        public const string AUTHENTICATED_USERS = "*";
        public const string NEGATIVE = "!";
        public const string NONE = "";

        private static bool MatchRole(IPrincipal user, string role)
        {
            if (role == ALL_USERS)
                return true;
            else if (role == AUTHENTICATED_USERS)
                return user.Identity.IsAuthenticated;
            else
                return user.IsInRole(role);
        }

        private static IEnumerable<string> GetPositiveRoles(string permissions)
        {
            string[] roles = permissions.Split(',');
            for (int i = 0; i < roles.Length; i++)
            {
                string role = roles[i];
                role = role.Trim();

                if (role.Length > 0 &&
                    role.StartsWith(NEGATIVE) == false)
                    yield return role;
            }
        }

        private static IEnumerable<string> GetNegativeRoles(string permissions)
        {
            string[] roles = permissions.Split(',');
            for (int i = 0; i < roles.Length; i++)
            {
                string role = roles[i];
                role = role.Trim();

                //Return the role without the ! character and trimmed
                if (role.Length > 1 &&
                    role.StartsWith(NEGATIVE))
                    yield return role.Substring(1).Trim();
            }
        }
        
        public static bool MatchUser(IPrincipal user, IOwner entity)
        {
            return entity != null && MatchUser(user, entity.Owner);
        }

        public static bool MatchUser(IPrincipal user, string owner)
        {
            return string.Equals(user.Identity.Name, owner, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static bool CanUserEdit(string username, string itemId)
        {
            IList<PersonType> myGroups = PersonManager.GetPersonTypesByUser(username);

            if (myGroups.FirstOrDefault(x => x.Name.Equals(BusiBlocksConstants.AdministratorsGroup)) != null)
                return true;

            IList<Access> accesses = AccessManager.GetItemEdittables(itemId);

            IList<Site> myLocations = SiteManager.GetSitesByUser(username, true);

            foreach (Access access in accesses)
            {
                //all users
                if (access.AllUsers)
                    return true;

                //all groups and all locations
                if (access.AllPersonTypes && access.AllSites)
                    return true;

                //this user
                if (access.UserId == username)
                    return true;

                //all groups and user location
                if (access.AllPersonTypes)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.SiteId == l.Id)
                            return true;
                    }
                }

                //all locations and user group
                if (access.AllSites)
                {
                    foreach (PersonType ug in myGroups)
                    {
                        if (access.PersonTypeId == ug.Id)
                            return true;
                    }
                }

                //user location and user group
                foreach (PersonType ug in myGroups)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.PersonTypeId == ug.Id && access.SiteId == l.Id)
                            return true;
                    }
                }
            }

            //no access
            return false;
        }

        public static bool CanUserView(string username, string itemId)
        {
            IList<PersonType> myGroups = PersonManager.GetPersonTypesByUser(username);

            if (myGroups.FirstOrDefault(x => x.Name.Equals(BusiBlocksConstants.AdministratorsGroup)) != null)
                return true;

            IList<Access> accesses = AccessManager.GetItemVisibilities(itemId);

            IList<Site> myLocations = SiteManager.GetSitesByUser(username, true);

            foreach (Access access in accesses)
            {
                //all users
                if (access.AllUsers)
                    return true;

                //all groups and all locations
                if (access.AllPersonTypes && access.AllSites)
                    return true;

                //this user
                if (access.UserId == username)
                    return true;

                //all groups and user location
                if (access.AllPersonTypes)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.SiteId == l.Id)
                            return true;
                    }
                }

                //all locations and user group
                if (access.AllSites)
                {
                    foreach (PersonType ug in myGroups)
                    {
                        if (access.PersonTypeId == ug.Id)
                            return true;
                    }
                }

                //user location and user group
                foreach (PersonType ug in myGroups)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.PersonTypeId == ug.Id && access.SiteId == l.Id)
                            return true;
                    }
                }
            }

            //no access
            return false;
        }

        public static bool CanUserContribute(string username, string itemId)
        {
            IList<PersonType> myGroups = PersonManager.GetPersonTypesByUser(username);

            if (myGroups.FirstOrDefault(x => x.Name.Equals(BusiBlocksConstants.AdministratorsGroup)) != null)
                return true;

            IList<Access> accesses = AccessManager.GetItemContributions(itemId);

            IList<Site> myLocations = SiteManager.GetSitesByUser(username, true);

            foreach (Access access in accesses)
            {
                //all users
                if (access.AllUsers)
                    return true;

                //all groups and all locations
                if (access.AllPersonTypes && access.AllSites)
                    return true;

                //this user
                if (access.UserId == username)
                    return true;

                //all groups and user location
                if (access.AllPersonTypes)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.SiteId == l.Id)
                            return true;
                    }
                }

                //all locations and user group
                if (access.AllSites)
                {
                    foreach (PersonType ug in myGroups)
                    {
                        if (access.PersonTypeId == ug.Id)
                            return true;
                    }
                }

                //user location and user group
                foreach (PersonType ug in myGroups)
                {
                    foreach (Site l in myLocations)
                    {
                        if (access.PersonTypeId == ug.Id && access.SiteId == l.Id)
                            return true;
                    }
                }
            }

            //no access
            return false;
        }

        public static bool CheckWriteAccess(string username, string itemId)
        {
            bool hasAccess = false;
            if (!string.IsNullOrEmpty(itemId))
            {
                if (SecurityHelper.CanUserEdit(username, itemId))
                    hasAccess = true;
                else
                {
                    if (SecurityHelper.CanUserContribute(username, itemId))
                    {
                        hasAccess = true;
                    }
                }
            }
            return hasAccess;
        }

        public static bool IsManager(IPrincipal user)
        {
            //make dynamic
            return user.IsInRole("managerblock");
        }

        public static bool CanAddNewContainer(IPrincipal user, string block)
        {
            return user.IsInRole(string.Format("{0}block:administrator", block)) ||
                   user.IsInRole(string.Format("{0}block:contributer", block)) || user.IsInRole(BusiBlocksConstants.AdministratorsRole);
        }
    }
}