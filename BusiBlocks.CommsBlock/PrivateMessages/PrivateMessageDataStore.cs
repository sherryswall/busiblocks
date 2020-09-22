using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;
using BusiBlocks.Membership;
using NHibernate.Transform;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    public class PrivateMessageDataStore : EntityDataStoreBase<PrivateMessage, string>
    {
        public PrivateMessageDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PrivateMessage> FindByRecipient(User user)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Recipient", user));
            criteria.Add(Expression.IsNull("DeletedDate"));
            criteria.AddOrder(Order.Desc("SentDate"));
            return Find(criteria, false);
        }

        public IList<PrivateMessage> FindBySender(User user)
        {

            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Sender", user));
            criteria.Add(Expression.IsNull("DeletedDate"));
            criteria.AddOrder(Order.Desc("SentDate"));
            return Find(criteria, false);
        }


        public PrivateMessage FindById(string Id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", Id));
            criteria.Add(Expression.IsNull("DeletedDate"));
            return FindUnique(criteria, false);
        }

    }
}
