using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonDataStore : EntityDataStoreBase<Person, string>
    {
        public PersonDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<Person> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("LastName"));
            return Find(criteria, false);
        }

        public Person FindByName(string lastName, string firstName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("LastName", lastName, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("FirstName", firstName, MatchMode.Exact));
            return FindUnique(criteria, false);
        }

        public IList<Person> FindAllByLastName(string lastName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("LastName", lastName, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Person> FindAllByFirstName(string firstName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("FirstName", firstName, MatchMode.Anywhere));
            return Find(criteria, false);
        }

        public IList<Person> FindByEmail(string email)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Email", email, MatchMode.Exact));
            criteria.AddOrder(Order.Asc("LastName"));

            return Find(criteria, false);
        }

        public IList<Person> FindByEmailLike(string email, PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Email", email, MatchMode.Anywhere));
            criteria.AddOrder(Order.Asc("LastName"));

            return Find(criteria, paging, false);
        }
    }
}