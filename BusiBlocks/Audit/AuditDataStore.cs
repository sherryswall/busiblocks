using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

//FxCop Warning:[Avoid Namespaces with few types, to be considered again if we merge Reporting and Audit.]

namespace BusiBlocks.Audit
{
    public class AuditDataStore : EntityDataStoreBase<AuditRecord, string>
    {
        public AuditDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public bool AuditRecordExist(string userName, string itemId, string action)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("UserName", userName));
            criteria.Add(Expression.Eq("Data", itemId));
            criteria.Add(Expression.Eq("Action", action));

            if (Find(criteria, null).Count == 0)
                return false;
            else
                return true;
            //return FindUnique(criteria) != null;
        }

        public IList<AuditRecord> FindAll()
        {
            ICriteria criteria = CreateCriteria();

            return base.Find(criteria, null);
        }

        public IList<AuditRecord> Find( string itemId, string action)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Data", itemId));
            criteria.Add(Expression.Eq("Action", action));

            return Find(criteria, null);
        }

        public IList<AuditRecord> Find(string userName, string itemId, string action)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("UserName", userName));
            criteria.Add(Expression.Eq("Data", itemId));
            criteria.Add(Expression.Eq("Action", action));

            return Find(criteria, null);
        }


        public IList<AuditRecord> FindAllFromDate(DateTime date)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Gt("TimeStamp", date));
            return base.Find(criteria, null);
        }
    }
}