using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.FormsBlock
{
    public class FormPropertyInstanceDataStore : EntityDataStoreBase<FormPropertyInstance, string>
    {
        public FormPropertyInstanceDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public FormPropertyInstance FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));

            return FindUnique(criteria, false);
        }

        new public IList<FormPropertyInstance> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("DisplayName"));

            return base.Find(criteria, false);
        }
    }
}
