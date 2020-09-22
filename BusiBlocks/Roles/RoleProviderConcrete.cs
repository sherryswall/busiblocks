using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using BusiBlocks.Membership;

namespace BusiBlocks.Roles
{
    public class RoleProviderConcrete : RoleProvider
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

        public override void CreateUserRole(User user, Role role)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var usersInRolesStore = new UserInRoleDataStore(transaction);
                var userInRole = new UserInRole(ApplicationName, user, role);
                usersInRolesStore.Insert(userInRole);

                transaction.Commit();
            }
        }

        public override void DeleteUserFromRole(User user, Role role)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var uirDS = new UserInRoleDataStore(transaction);
                IList<UserInRole> userInRoles = uirDS.FindForUserAndRole(mApplicationName, user, role);
                if (userInRoles.Count > 0)
                {
                    foreach (UserInRole uir in userInRoles)
                        uirDS.Delete(uir.Id);
                    transaction.Commit();
                }
            }
        }

        public override Role GetRole(string roleId)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleDataStore = new RoleDataStore(transaction);
                return roleDataStore.FindByKey(roleId);
            }
        }

        public override Role GetRoleByName(string roleName)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleDataStore = new RoleDataStore(transaction);
                return roleDataStore.FindByName(mApplicationName, roleName);
            }
        }

        public override IList<Role> GetAllRoles()
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleDataStore = new RoleDataStore(transaction);
                return roleDataStore.FindAll(ApplicationName);
            }
        }

        public override IList<Role> GetRolesByUser(User user)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                IList<Role> runningList = new List<Role>();
                var uirds = new UserInRoleDataStore(transaction);
                IList<UserInRole> listUserInRole = uirds.FindForUser(ApplicationName, user);
                foreach (UserInRole ur in listUserInRole)
                {
                    IEnumerable<Role> q = from x in runningList where x.Id.Equals(ur.User.Id) select x;
                    if (!q.Any())
                    {
                        runningList.Add(RoleManager.GetRole(ur.Role.Id));
                    }
                }
                return runningList;
            }
        }

        public override void CreateRole(string roleName)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var roleDataStore = new RoleDataStore(transaction);
                var role = new Role(ApplicationName, roleName);
                roleDataStore.Insert(role);
                transaction.Commit();
            }
        }

        public override void DeleteRole(string roleName)
        {
            using (var transaction = new TransactionScope(mConfiguration))
            {
                var rDS = new RoleDataStore(transaction);
                Role role = rDS.FindByName(ApplicationName, roleName);
                rDS.Delete(role.Id);
                transaction.Commit();
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

        public string ApplicationName
        {
            get { return mApplicationName; }
            set { mApplicationName = value; }
        }

        #endregion
    }
}