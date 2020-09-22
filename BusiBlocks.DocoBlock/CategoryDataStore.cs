using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.DocoBlock
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
            criteria.Add(Expression.Eq("DisplayName", name));

            return FindUnique(criteria, false);
        }
        public IList<Category> FindByLikeName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("DisplayName", name, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Category> FindByChildOfCategory(string id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ParentCategory.Id", id));

            return Find(criteria, false);
        }

        new public IList<Category> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("DisplayName"));

            return base.Find(criteria, false);
        }
    }
}
