using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonTypeDataStore : EntityDataStoreBase<PersonType, string>
    {
        public PersonTypeDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PersonType> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Name"));
            return base.Find(criteria, false);
        }

        public PersonType FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Exact));
            return FindUnique(criteria, false);
        }
    }
}