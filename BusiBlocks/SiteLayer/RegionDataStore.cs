using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.SiteLayer
{
    public class RegionDataStore : EntityDataStoreBase<Region, string>
    {
        public RegionDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<Region> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Name"));
            criteria.SetFetchMode("RegionType", FetchMode.Eager);
            return base.Find(criteria, false);
        }

        public Region FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Exact));
            return FindUnique(criteria, false);
        }

        public IList<Region> FindAllByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Region> FindAllByRegionType(RegionType regionType)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("RegionType", regionType));
            return Find(criteria, false);
        }

        public IList<Region> FindAllBelow(Region region)
        {
            IList<Region> runningList = new List<Region>();
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("ParentRegion.Id", region.Id, MatchMode.Exact));
            IList<Region> children = Find(criteria, false);
            foreach (Region child in children)
            {
                if (!runningList.Contains(child))
                    runningList.Add(child);
                IList<Region> grandChildren = FindAllBelow(child);
                foreach (Region grandchild in grandChildren)
                {
                    if (!runningList.Contains(grandchild))
                        runningList.Add(grandchild);
                }
            }
            return runningList;
        }

        public IList<Region> FindByParentRegion(string id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("ParentRegion.Id", id, MatchMode.Exact));
            criteria.AddOrder(Order.Asc("Id"));
            return base.Find(criteria, false);
        }
    }
}