using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.SiteLayer
{
    public class RegionTypeDataStore : EntityDataStoreBase<RegionType, string>
    {
        public RegionTypeDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<RegionType> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Seq"));

            return base.Find(criteria, false);
        }

        public IList<RegionType> FindAllBelow(RegionType regionType)
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Seq"));
            criteria.Add(Expression.Gt("Seq", regionType.Seq));

            return base.Find(criteria, false);
        }

        public RegionType FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Exact));

            return FindUnique(criteria, false);
        }
    }
}