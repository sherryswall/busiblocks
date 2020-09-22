using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.ItemApprovalStatusLayer
{
    public class ItemApprovalStatusDataStore : EntityDataStoreBase<ItemApprovalStatus, string>
    {
        public ItemApprovalStatusDataStore(TransactionScope transactionScope) : base(transactionScope) { }

        public ItemApprovalStatus Find(string id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", id));

            return FindUnique(criteria, null);
        }

        public ItemApprovalStatus FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));
            return FindUnique(criteria, null);
        }
    }
}