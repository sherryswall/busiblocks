using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.CommsBlock.Forums
{
    /// <summary>
    /// Class that use NHibernate to save the Category data
    /// </summary>
    public class CategoryDataStore : EntityDataStoreBase<Category, string>
    {
        public CategoryDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public Category FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));

            return FindUnique(criteria, false);
        }

        public IList<Category> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("DisplayName"));

            return base.Find(criteria, false);
        }

    }
}
