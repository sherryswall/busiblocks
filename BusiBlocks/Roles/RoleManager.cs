using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using BusiBlocks.Membership;

namespace BusiBlocks.Roles
{
    public static class RoleManager
    {
        private const string StandardBlockIdentifier = "block";
        private const string StandardAdminBlockName = "core:administrator";
        private const string CustomAdminDisplay = "Admin";

        //Public feature API
        private static readonly RoleProvider defaultProvider;
        private static readonly RoleProviderCollection providerCollection;

        static RoleManager()
        {
            //Get the feature's configuration info
            var ac =
                (RoleProviderConfiguration) ConfigurationManager.GetSection("roleManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for roleManager.");

            //Instantiate the providers
            providerCollection = new RoleProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof (RoleProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[ac.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the roleManager.",
                    ac.ElementInformation.Properties["defaultProvider"].Source,
                    ac.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        public static RoleProvider Provider
        {
            get { return defaultProvider; }
        }

        public static RoleProviderCollection Providers
        {
            get { return providerCollection; }
        }

        public static void AddUserToRole(User user, Role role)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (role == null)
                throw new ArgumentNullException("role");

            Provider.CreateUserRole(user, role);
        }

        public static void DeleteUserFromRole(User user, Role role)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (role == null)
                throw new ArgumentNullException("role");

            Provider.DeleteUserFromRole(user, role);
        }

        public static Role GetRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException("roleId");

            return Provider.GetRole(roleId);
        }

        public static Role GetRoleByName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            return Provider.GetRoleByName(roleName);
        }

        public static IList<Role> GetRolesByUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Provider.GetRolesByUser(user);
        }

        public static IList<Role> GetAllRoles()
        {
            return Provider.GetAllRoles();
        }

        public static void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            Provider.CreateRole(roleName);
        }

        public static void DeleteRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("roleName");

            Provider.DeleteRole(roleName);
        }

        public static string CompressRoleName(string roleName)
        {
            string origionalRoleName = roleName;
            try
            {
                string endText = origionalRoleName.Substring(origionalRoleName.Length - 5);

                if (endText.ToUpper().Trim() == StandardBlockIdentifier.ToUpper().Trim()) // if its <SOMETHING>BLOCK
                {
                    roleName = origionalRoleName.Substring(0, origionalRoleName.Length - 5); //remove BLOCK from text
                }
                else if (origionalRoleName.ToUpper().Trim() == StandardAdminBlockName.ToUpper().Trim())
                    //if its the core:administrator
                {
                    roleName = CustomAdminDisplay; //Just display shortened version
                }
            }
            catch (Exception)
            {
                roleName = origionalRoleName;
            }
            return roleName;
        }
    }
}