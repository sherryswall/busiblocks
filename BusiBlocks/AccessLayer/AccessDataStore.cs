using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.AccessLayer
{
    public class AccessDataStore : EntityDataStoreBase<Access, string>
    {
        public AccessDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<Access> FindAll()
        {
            ICriteria criteria = CreateCriteria();

            return base.Find(criteria, false);
        }

        public IList<Access> FindAllByItem(string itemId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));

            return Find(criteria, false);
        }

        public IList<Access> FindVisibilityByItem(string itemId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("AccessType", AccessType.View));

            return Find(criteria, false);
        }

        public IList<Access> FindEdittableByItem(string itemId)
        {           
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("AccessType", AccessType.Edit));
                        
            return Find(criteria, false);
        }

        public IList<Access> FindContributableByItem(string itemId)
        {           
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("AccessType",AccessType.Contribute));
            
            return Find(criteria, false);
        }



        public IList<Access> FindByLocation(string itemId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            return Find(criteria, false);
        }

        public Access Find(string itemId, ItemType itemType, AccessType accessType, string personTypeId, string siteId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("PersonTypeId", personTypeId));
            criteria.Add(Expression.Eq("AccessType", accessType));
            criteria.Add(Expression.Eq("SiteId", siteId));

            return FindUnique(criteria, false);
        }

        public Access Find(string itemId, ItemType itemType, AccessType accessType, string personTypeId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("PersonTypeId", personTypeId));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return FindUnique(criteria, false);
        }

        public Access Find(string itemId, ItemType itemType, string personTypeId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));
            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("PersonTypeId", personTypeId));

            return FindUnique(criteria, false);
        }

        public IList<Access> FindForUser(ItemType itemType, AccessType accessType, string userId)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("UserId", userId));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForPersonTypeSite(ItemType itemType, AccessType accessType, string personTypeId,
                                                   string locationId)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("PersonTypeId", personTypeId));
            criteria.Add(Expression.Eq("SiteId", locationId));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForAllUsers(ItemType itemType, AccessType accessType)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("AllUsers", true));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForAllPersonTypesPlusSite(ItemType itemType, AccessType accessType, string locationId)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("AllPersonTypes", true));
            criteria.Add(Expression.Eq("SiteId", locationId));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForAllPersonTypesPlusAllSites(ItemType itemType, AccessType accessType)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("AllPersonTypes", true));
            criteria.Add(Expression.Eq("AllSites", true));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForAllSitesPlusPersonType(ItemType itemType, AccessType accessType, string personTypeId)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("AllSites", true));
            criteria.Add(Expression.Eq("PersonTypeId", personTypeId));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }

        public IList<Access> FindForAllSitesPlusAllPersonTypes(ItemType itemType, AccessType accessType)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ItemType", itemType));
            criteria.Add(Expression.Eq("AllSites", true));
            criteria.Add(Expression.Eq("AllPersonTypes", true));
            criteria.Add(Expression.Eq("AccessType", accessType));

            return Find(criteria, false);
        }
    }
}