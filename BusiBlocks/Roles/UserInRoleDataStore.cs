using System.Collections.Generic;
using BusiBlocks.Membership;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.Roles
{
    /// <summary>
    /// Class that use NHibernate to save the UserInRole data.
    /// 
    /// Note that the username is considered always case insensitive.
    /// </summary>
    public class UserInRoleDataStore : EntityDataStoreBase<UserInRole, string>
    {
        public UserInRoleDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public UserInRole Find(string applicationName, User user, Role role)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("User.Id", user.Id, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("Role.Id", role.Id, MatchMode.Exact));

            return FindUnique(criteria, false);
        }

        public IList<UserInRole> FindForUser(string applicationName, User user)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("User.Id", user.Id, MatchMode.Exact));

            return Find(criteria, false);
        }

        public IList<UserInRole> FindForRole(string applicationName, Role role)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Role.Id", role.Id, MatchMode.Exact));

            return Find(criteria, false);
        }

        public IList<UserInRole> FindForUserAndRole(string applicationName, User user, Role role)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.InsensitiveLike("Role.Id", role.Id, MatchMode.Exact));
            criteria.Add(Expression.InsensitiveLike("User.Id", user.Id, MatchMode.Exact));

            return Find(criteria, false);
        }
    }
}