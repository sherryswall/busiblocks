using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.FormsBlock
{
    public class FormPropertyDataStore : EntityDataStoreBase<FormProperty, string>
    {
        public FormPropertyDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public FormProperty FindByName(string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Name", name));

            return FindUnique(criteria, false);
        }

        public IList<FormProperty> FindAll(FormDefinition formDefinition)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("FormDefinition.Id", formDefinition.Id));
            criteria.AddOrder(Order.Asc("SequenceNo"));
            return base.Find(criteria, false);
        }
    }
}
