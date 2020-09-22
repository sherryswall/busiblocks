using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.ApproverLayer
{
    public class ApproverDataStore : EntityDataStoreBase<Approver, string>
    {
        public ApproverDataStore(TransactionScope transactionScope) : base(transactionScope) {}

        public IList<Approver> FindAll()
        {
            ICriteria criteria = CreateCriteria();

            return Find(criteria, false);
        }

        public IList<Approver> FindAllByItem(string itemId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ItemId", itemId));

            return Find(criteria, false);
        }
    }
}