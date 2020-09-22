using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.Versioning
{
    class VersionItemDataStore : EntityDataStoreBase<VersionItem, string>
    {
        public VersionItemDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }
        public IList<VersionItem> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            return base.Find(criteria, false);
        }
        public VersionItem FindGroupIdByVersionId(string Id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", Id));
            return FindUnique(criteria, false);
        }
        public VersionItem FindGroupIdByItemId(string ItemId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", ItemId));
            return FindUnique(criteria, false);
        }
        public VersionItem FindVersionByGroupId(string GroupId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("GroupId", GroupId));
            criteria.AddOrder(Order.Desc("DateCreated"));
            return Find(criteria, false).First<VersionItem>();
        }
        public IList<VersionItem> FindAllByGroupId(string GroupId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("GroupId", GroupId));
            criteria.AddOrder(Order.Desc("DateCreated"));
            return Find(criteria, false);
        }

        public VersionItem FindPublishedVersion(string GroupId)
        {            
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("GroupId", GroupId));
            criteria.AddOrder(Order.Desc("DateCreated"));
            return Find(criteria, false).First<VersionItem>();
        }
        public IList<VersionItem> FindPublishedVersions(string GroupId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("GroupId", GroupId));
            criteria.AddOrder(Order.Desc("DateCreated"));
            return Find(criteria, false);
        }
    }
}
