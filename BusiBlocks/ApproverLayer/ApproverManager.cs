using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using BusiBlocks.Membership;
using BusiBlocks.ApproverLayer;

namespace BusiBlocks.ApproverLayer
{
    public static class ApproverManager
    {
        //Public feature API
        private static readonly ApproverProviderCollection providerCollection = InitialiseProviderCollection();

        static ApproverManager()
        {
        }

        private static ApproverProvider defaultProvider
        {
            get
            {
                if (providerCollection != null)
                {
                    //Get the feature's configuration info
                    var ac =
                        (ApproverProviderConfiguration)ConfigurationManager.GetSection("approverManager");

                    if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                        throw new ProviderException("You must specify a valid default provider for approverManager.");

                    return providerCollection[ac.DefaultProvider];
                }
                return null;
            }
        }

        public static ApproverProvider Provider
        {
            get { return defaultProvider; }
        }

        public static ApproverProviderCollection Providers
        {
            get { return providerCollection; }
        }

        private static ApproverProviderCollection InitialiseProviderCollection()
        {
            //Get the feature's configuration info
            var ac =
                (ApproverProviderConfiguration)ConfigurationManager.GetSection("approverManager");

            if (ac == null || ac.DefaultProvider == null || ac.Providers == null || ac.Providers.Count < 1)
                throw new ProviderException("You must specify a valid default provider for approverManager.");

            var providerCollection = new ApproverProviderCollection();
            ProvidersHelper.InstantiateProviders(ac.Providers, providerCollection, typeof(ApproverProvider));
            providerCollection.SetReadOnly();
            return providerCollection;
        }

        public static void AddApprover(Approver approver)
        {
            Provider.CreateApprover(approver);
        }

        public static void AddApprovers(List<Approver> approvers)
        { 
            foreach(Approver approver in approvers)
                Provider.   CreateApprover(approver);
        }

        public static void RemoveApproversbyItem(string itemId)
        {
            Provider.DeleteApproversByItem(itemId);
        }

        public static IList<Approver> GetApproversByItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("path");

            return Provider.GetApproversByItem(itemId);
        }

        public static bool IsApprover(string username, string ItemId)
        {
            bool isApprover = false;

            if (username == "admin")
            {
                isApprover = true;
            }
            else
            {
                string userId = MembershipManager.GetUserByName(username).Id;

                IList<Approver> approvers = ApproverManager.GetApproversByItem(ItemId);
                foreach (Approver approver in approvers)
                {
                    if ((!string.IsNullOrEmpty(approver.UserId)) && (approver.UserId == userId))
                    {
                        isApprover = true;
                    }


                    if (string.IsNullOrEmpty(approver.UserId))
                    {
                        if (!string.IsNullOrEmpty(approver.CategoryId))
                        {
                            if (SecurityHelper.CanUserEdit(username, approver.CategoryId))
                            {
                                isApprover = true;
                            }
                        }
                    }
                }
            }
            return isApprover;
        }
    }
}