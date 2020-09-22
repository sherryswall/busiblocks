using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonRegionDataStore : EntityDataStoreBase<PersonRegion, string>
    {
        public PersonRegionDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PersonRegion> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            return base.Find(criteria, false);
        }

        public IList<PersonRegion> FindRegionsByPerson(string personId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            return Find(criteria, false);
        }

        public IList<PersonRegion> FindPersonsByRegion(string regionId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Region.Id", regionId, MatchMode.Exact));
            return Find(criteria, false);
        }

        public IList<PersonRegion> FindByPersonAndRegion(string personId, string regionId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("Region.Id", regionId, MatchMode.Exact));
            return Find(criteria, false);
        }
    }
}