using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonPersonTypeDataStore : EntityDataStoreBase<PersonPersonType, string>
    {
        public PersonPersonTypeDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PersonPersonType> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            return base.Find(criteria, false);
        }

        public IList<PersonPersonType> FindPersonTypesByPerson(string personId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));

            return Find(criteria, false);
        }

        public IList<PersonPersonType> FindPersonByPersonType(string personTypeId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("PersonType.Id", personTypeId, MatchMode.Exact));

            return Find(criteria, false);
        }

        public IList<PersonPersonType> FindByPersonAndPersonType(string personId, string personTypeId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("PersonType.Id", personTypeId, MatchMode.Exact));

            return Find(criteria, false);
        }
    }
}