using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.FormsBlock
{
    public class FormInstanceDataStore : EntityDataStoreBase<FormInstance, string>
    {
        public FormInstanceDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public FormInstance FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));

            return FindUnique(criteria, false);
        }

        new public IList<FormInstance> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("CreatedBy"));

            return base.Find(criteria, false);
        }
    }
}
