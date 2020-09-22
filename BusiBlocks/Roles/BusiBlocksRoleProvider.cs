using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using BusiBlocks.Membership;
using BusiBlocks.PersonLayer;
using BusiBlocks.SiteLayer;

namespace BusiBlocks.Roles
{
    /// <summary>
    /// A implementation of a System.Web.Security.RoleProvider class that use the BusiBlocks classes.
    /// See MSDN System.Web.Security.RoleProvider documentation for more informations about RoleProvider.
    /// </summary>
    public class BusiBlocksRoleProvider : System.Web.Security.RoleProvider
    {
        private ConnectionParameters mConfiguration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "BusiBlocksRoleProvider";

            base.Initialize(name, config);


            mProviderName = name;
            mApplicationName = ExtractConfigValue(config, "applicationName", ConnectionParameters.DefaultApp);
                //System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath

            string connName = ExtractConfigValue(config, "connectionStringName", null);
            mConfiguration = ConnectionParameters.Create(connName);


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
        private string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
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

        #region Properties

        private string mApplicationName;
        private string mProviderName;

        public string ProviderName
        {
            get { return mProviderName; }
            set { mProviderName = value; }
        }

        public override string ApplicationName
        {
            get { return mApplicationName; }
            set { mApplicationName = value; }
        }

        #endregion

        #region Methods

        private void LogException(Exception exception, string action)
        {
            Log.Error(GetType(), "Exception on " + action, exception);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException("Not implementing AddUsersToRoles");
            //using (TransactionScope transaction = new TransactionScope(mConfiguration))
            //{
            //    RoleDataStore roleStore = new RoleDataStore(transaction);

            //    //Find the roles
            //    Role[] roles = new Role[roleNames.Length];
            //    for (int i = 0; i < roleNames.Length; i++)
            //    {
            //        //Find the role
            //        Role role = roleStore.FindByName(ApplicationName, roleNames[i]);
            //        if (role == null)
            //            throw new RoleNotFoundException(roleNames[i]);

            //        roles[i] = role;
            //    }

            //    UserInRoleDataStore usersInRolesStore = new UserInRoleDataStore(transaction);
            //    foreach (string userName in usernames)
            //    {
            //        foreach (Role role in roles)
            //        {
            //            UserInRole userInRole = new UserInRole(ApplicationName, user.Name, role.Name);

            //            usersInRolesStore.Insert(userInRole);
            //        }
            //    }

            //    transaction.Commit();
            //}
        }

        public override void CreateRole(string roleName)
        {
            //Check required for MSDN
            if (roleName == null || roleName == "")
                throw new ProviderException("Role name cannot be empty or null.");
            if (roleName.IndexOf(',') > 0)
                throw new ArgumentException("Role names cannot contain commas.");
            if (RoleExists(roleName))
                throw new ProviderException("Role name already exists.");

            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleStore = new RoleDataStore(transaction);

                roleStore.Insert(new Role(ApplicationName, roleName));

                transaction.Commit();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleStore = new RoleDataStore(transaction);
                var userInRoleDataStore = new UserInRoleDataStore(transaction);

                //Find role
                Role role = roleStore.FindByName(ApplicationName, roleName);
                if (role == null)
                    throw new RoleNotFoundException(roleName);

                IList<UserInRole> listUserInRole = userInRoleDataStore.FindForRole(ApplicationName, role);

                if (throwOnPopulatedRole && listUserInRole.Count > 0)
                    throw new ProviderException("Cannot delete a populated role.");

                foreach (UserInRole ur in listUserInRole)
                {
                    ur.Deleted = true;
                    userInRoleDataStore.Update(ur);
                }

                role.Deleted = true;
                role.Name += DateTimeHelper.GetCurrentTimestamp();
                roleStore.Update(role);

                transaction.Commit();
            }

            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var userNames = new List<string>();

            using (var transaction = new TransactionScope(mConfiguration))
            {
                var rDS = new RoleDataStore(transaction);
                Role role = rDS.FindByName(mApplicationName, roleName);
                var uDS = new UserDataStore(transaction);
                User user = uDS.FindByName(mApplicationName, usernameToMatch);
                var userInRoleDataStore = new UserInRoleDataStore(transaction);

                IList<UserInRole> listUserInRole = userInRoleDataStore.FindForUserAndRole(ApplicationName, user, role);

                foreach (UserInRole ur in listUserInRole)
                {
                    userNames.Add(ur.User.Name);
                }
            }

            return userNames.ToArray();
        }

        public override string[] GetAllRoles()
        {
            var rolesNames = new List<string>();

            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleDataStore = new RoleDataStore(transaction);

                IList<Role> list = roleDataStore.FindAll(ApplicationName);

                foreach (Role ur in list)
                {
                    rolesNames.Add(ur.Name);
                }
            }

            return rolesNames.ToArray();
        }

        /// <summary>
        /// Determines the roles which are valid for this user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public override string[] GetRolesForUser(string userName)
        {
            var roleNames = new List<string>();

            // 0. Find the person represented by the userName.
            // 1. Find all person types associated with the person.
            // 2. Find all person type / role combos for the person.

            // 3. If the user is a region administrator, then add the "core:regionadmin" role.
            //    If the user is a site administrator, then add the "core:siteadmin" role.

            // 4. Return roles.

            using (var transaction = new TransactionScope(mConfiguration))
            {
                Person person = PersonManager.GetPersonByUserName(userName);
                IList<PersonType> personTypes = PersonManager.GetPersonTypesByPerson(person);
                foreach (PersonType personType in personTypes)
                {
                    IList<PersonTypeRole> ppt = PersonManager.GetPersonTypeRoleByPerson(personType);
                    foreach (PersonTypeRole ptr in ppt)
                    {
                        if (!roleNames.Contains(ptr.Role.Name))
                            roleNames.Add(ptr.Role.Name);
                    }
                }

                // Find region and/or site admin roles.
                IList<Region> personRegions = PersonManager.GetAdminRegionsByPerson(person, false);
                IList<Site> personSites = PersonManager.GetAdminSitesByPerson(person);
                if (personRegions.Count > 0)
                    roleNames.Add("core:regionadmin");
                if (personSites.Count > 0)
                    roleNames.Add("core:siteadmin");
            }

            return roleNames.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var userNames = new List<string>();

            using (var transaction = new TransactionScope(mConfiguration))
            {
                var rDS = new RoleDataStore(transaction);
                Role role = rDS.FindByName(mApplicationName, roleName);
                var userInRoleDataStore = new UserInRoleDataStore(transaction);

                IList<UserInRole> listUserInRole = userInRoleDataStore.FindForRole(ApplicationName, role);

                foreach (UserInRole ur in listUserInRole)
                {
                    userNames.Add(ur.User.Name);
                }
            }

            return userNames.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var rDS = new RoleDataStore(transaction);
                Role role = rDS.FindByName(mApplicationName, roleName);
                var uDS = new UserDataStore(transaction);
                User user = uDS.FindByName(mApplicationName, username);
                var userInRoleDataStore = new UserInRoleDataStore(transaction);

                if (userInRoleDataStore.Find(ApplicationName, user, role) != null)
                    return true;
                else
                    return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var usersInRolesStore = new UserInRoleDataStore(transaction);

                foreach (string userName in usernames)
                {
                    foreach (string roleName in roleNames)
                    {
                        var rDS = new RoleDataStore(transaction);
                        Role role = rDS.FindByName(mApplicationName, roleName);
                        var uDS = new UserDataStore(transaction);
                        User user = uDS.FindByName(mApplicationName, userName);
                        UserInRole userInRole = usersInRolesStore.Find(ApplicationName, user, role);
                        if (userInRole == null)
                            throw new UserInRoleNotFoundException(userName, roleName);

                        userInRole.Deleted = true;
                        usersInRolesStore.Update(userInRole);
                    }
                }

                transaction.Commit();
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var store = new RoleDataStore(transaction);
                if (store.FindByName(ApplicationName, roleName) != null)
                    return true;
                else
                    return false;
            }
        }

        #endregion
    }
}