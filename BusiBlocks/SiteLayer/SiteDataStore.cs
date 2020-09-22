using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.SiteLayer
{
    public class SiteDataStore : EntityDataStoreBase<Site, string>
    {
        public SiteDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<Site> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Id"));
            criteria.SetFetchMode("Region", FetchMode.Eager);
            criteria.SetFetchMode("Region.RegionType", FetchMode.Eager);
            return base.Find(criteria, false);
        }

        public IList<Site> FindAnywhere(string regionId, string siteName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", siteName, MatchMode.Anywhere));
            criteria.Add(Expression.Eq("Region.Id", regionId));
            return Find(criteria, false);
        }

        public Site FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Anywhere));
            return FindUnique(criteria, false);
        }

        public IList<Site> FindAllByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Site> FindAllByRegion(string regionId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Region.Id", regionId));
            return Find(criteria, false);
        }
    }
}