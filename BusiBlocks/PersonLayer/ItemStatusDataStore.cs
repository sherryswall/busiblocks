using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class ItemStatusDataStore : EntityDataStoreBase<ItemStatus, string>
    {
        public ItemStatusDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<ItemStatus> FindByPerson(string personId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            return Find(criteria, false);
        }

        public IList<ItemStatus> FindByPersonAndVersion(string personId, string version)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", MatchMode.Exact));
            if (!string.IsNullOrEmpty(version))
                criteria.Add(Expression.Eq("Version", true));
            return Find(criteria, false);
        }
    }
}