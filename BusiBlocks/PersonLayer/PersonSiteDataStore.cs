using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonSiteDataStore : EntityDataStoreBase<PersonSite, string>
    {
        public PersonSiteDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PersonSite> FindSitesByPerson(string personId, bool isAssigned)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            if (isAssigned)
                criteria.Add(Expression.Eq("IsAssigned", true));
            return Find(criteria, false);
        }

        public IList<PersonSite> FindPersonsBySite(string siteId, bool isAssigned)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Site.Id", siteId, MatchMode.Exact));
            if (isAssigned)
                criteria.Add(Expression.Eq("IsAssigned", true));
            return Find(criteria, false);
        }

        public IList<PersonSite> FindByPersonAndSite(string personId, string siteId, bool isAssigned)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("Site.Id", siteId, MatchMode.Exact));
            if (isAssigned)
                criteria.Add(Expression.Eq("IsAssigned", true));
            return Find(criteria, false);
        }
    }
}