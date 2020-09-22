using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.FormsBlock
{
    public class FormDefinitionDataStore : EntityDataStoreBase<FormDefinition, string>
    {
        public FormDefinitionDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public FormDefinition FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));

            return FindUnique(criteria, false);
        }

        new public IList<FormDefinition> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Name"));

            return base.Find(criteria, false);
        }
    }
}
