using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.ItemStatusLayer
{
    public class ItemStatusDataStore : EntityDataStoreBase<ItemStatus, string>
    {
        public ItemStatusDataStore(TransactionScope transactionScope) : base(transactionScope) { }

        public ItemStatus Find(string id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", id));

            return FindUnique(criteria, null);
        }

        public ItemStatus FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));
            return FindUnique(criteria, null);
        }
    }
}