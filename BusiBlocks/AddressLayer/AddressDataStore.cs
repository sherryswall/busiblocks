using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.AddressLayer
{
    public class AddressDataStore : EntityDataStoreBase<Address, string>
    {
        private TransactionScope _transactionScope;

        public AddressDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
            _transactionScope = transactionScope;
        }

        public IList<Address> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Address1"));
            return base.Find(criteria, false);
        }

        public Address FindByAddress1(string address1)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Address1", address1, MatchMode.Anywhere));
            return FindUnique(criteria, false);
        }

        public IList<Address> FindAllByAddress1(string address1)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Address1", address1, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Address> FindAllByPostcode(string postcode)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Postcode", postcode));
            return Find(criteria, false);
        }

        public IList<Address> FindAllBySuburb(string suburb)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Suburb", suburb));
            return Find(criteria, false);
        }

        public IList<Address> FindAllByState(string state)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("State", state));
            return Find(criteria, false);
        }

        public IList<Address> FindAllByProperties(string address1, string address2, string suburb, string postcode,
                                                  string state)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Address1", address1));
            if (!string.IsNullOrEmpty(address2))
                criteria.Add(Expression.Eq("Address2", address2));
            if (!string.IsNullOrEmpty(suburb))
                criteria.Add(Expression.Eq("Suburb", suburb));
            if (!string.IsNullOrEmpty(postcode))
                criteria.Add(Expression.Eq("Postcode", postcode));
            if (!string.IsNullOrEmpty(state))
                criteria.Add(Expression.Eq("State", state));
            return Find(criteria, false);
        }
    }
}