using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.Membership
{
    /// <summary>
    /// Class that use NHibernate to save the User data
    /// </summary>
    public class UserDataStore : EntityDataStoreBase<User, string>
    {
        public UserDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public User FindByPerson(string applicationName, string personId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));

            return FindUnique(criteria, false);
        }

        public IList<User> FindAllByPerson(string applicationName, string personId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Person.Id", personId, MatchMode.Exact));

            return Find(criteria, false);
        }

        /// <summary>
        /// Find the specified user by name. Note that the name is searched using a case insensitive match.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public User FindByName(string applicationName, string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Exact));

            return FindUnique(criteria, false);
        }

        public IList<User> FindAll(string applicationName, PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.AddOrder(Order.Asc("Name"));

            return Find(criteria, paging, false);
        }

        public IList<User> FindAll(string applicationName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.AddOrder(Order.Asc("Name"));

            return Find(criteria, false);
        }

        public IList<User> FindAllEnabled()
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Enabled", true));
            criteria.AddOrder(Order.Asc("Name"));
            return Find(criteria, false);
        }

        /// <summary>
        /// Find the specified user by name. Note that the name is searched using a case insensitive match.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="name"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IList<User> FindByNameLike(string applicationName, string name, PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Name", name, MatchMode.Anywhere));
            criteria.AddOrder(Order.Asc("Name"));

            return Find(criteria, paging, false);
        }

        /// <summary>
        /// Find the specified user by e-mail. Note that the e-mail is searched using a case insensitive match.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public IList<User> FindByEmail(string applicationName, string email)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("EMail", email, MatchMode.Exact));
            criteria.AddOrder(Order.Asc("Name"));

            return Find(criteria, false);
        }

        /// <summary>
        /// Find the specified user by e-mail. Note that the e-mail is searched using a case insensitive match.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="email"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IList<User> FindByEmailLike(string applicationName, string email, PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("EMail", email, MatchMode.Anywhere));
            criteria.AddOrder(Order.Asc("Name"));

            return Find(criteria, paging, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userIsOnlineTimeWindow">Specifies the time span after the last-activity date/time stamp for a user during which the user is considered online.</param>
        /// <returns></returns>
        public int NumbersOfLoggedInUsers(string applicationName, TimeSpan onlineTimeWindow)
        {
            DateTime compareTime = DateTime.Now.Subtract(onlineTimeWindow);

            ICriteria criteria = CreateCriteria();
            criteria.Add(new EqExpression("ApplicationName", applicationName));
            criteria.Add(new GtExpression("LastActivityDate", compareTime));

            return Count(criteria);
        }
    }
}