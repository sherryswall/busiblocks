using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.DocoBlock
{
    public class ChapterDataStore : EntityDataStoreBase<Chapter, string>
    {
        public ChapterDataStore(TransactionScope transaction):base(transaction)
        { }

        public IList<Chapter> FindAllItems()
        {
            ICriteria criteria = CreateCriteria();
            //criteria.AddOrder(Order.Asc("Sequence"));

            return base.Find(criteria, false);
        }
        public IList<Chapter> FindAllItems(string docId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("DocId", docId));
            //criteria.AddOrder(Order.Asc("Sequence"));
            return base.Find(criteria, false);
        }
    }
}
