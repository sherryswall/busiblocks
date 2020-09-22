using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.SiteLayer
{
    public class SiteTypeDataStore : EntityDataStoreBase<SiteType, string>
    {
        public SiteTypeDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<SiteType> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Name"));

            return base.Find(criteria, false);
        }

        public SiteType FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Exact));
            //criteria.Add(Expression.Eq("GroupId", groupId));

            return FindUnique(criteria, false);
        }
    }
}