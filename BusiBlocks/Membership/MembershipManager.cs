using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web.Security;
using BusiBlocks.PersonLayer;

namespace BusiBlocks.Membership
{
    public class MembershipManager
    {
        //Public feature API
        private static readonly MembershipProvider defaultProvider;
        private static readonly MembershipProviderCollection providerCollection;

        static MembershipManager()
        {
            //Get the feature's configuration info
            var ac =
                (MembershipProviderConfiguration) ConfigurationManager.GetSection("membershipManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for membershipManager.");

            //Instantiate the providers
            providerCollection = new MembershipProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof (MembershipProvider));
            providerCollection.SetReadOnly();
            defaultProvider = providerCollection[ac.DefaultProvider];
            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException(
                    "You must specify a default provider for the membershipManager.",
                    ac.ElementInformation.Properties["defaultProvider"].Source,
                    ac.ElementInformation.Properties["defaultProvider"].LineNumber);
            }
        }

        public static MembershipProvider Provider
        {
            get { return defaultProvider; }
        }

        public static MembershipProviderCollection Providers
        {
            get { return providerCollection; }
        }

        public static User CreateUser(string username, string password)
        {
            MembershipUser membershipUser = System.Web.Security.Membership.CreateUser(username, password);
            User user = GetUser(membershipUser.ProviderUserKey.ToString());

            return user;
        }

        public static User GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            return Provider.GetUser(userId);
        }

        public static User GetUserByName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            return Provider.GetUserByName(userName);
        }

        public static User GetUserByPerson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            return Provider.GetUserByPersonId(person.Id);
        }

        public static void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            Provider.UpdateUser(user);
        }

        public static IList<User> GetAllUsers()
        {
            return Provider.GetAllUsers();
        }
    }
}