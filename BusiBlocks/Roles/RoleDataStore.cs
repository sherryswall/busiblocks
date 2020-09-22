using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.Roles
{
    /// <summary>
    /// Class that use NHibernate to save the Role data
    /// </summary>
    public class RoleDataStore : EntityDataStoreBase<Role, string>
    {
        public RoleDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public Role FindByName(string applicationName, string name)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.Add(Expression.Eq("Name", name));
            return FindUnique(criteria, false);
        }

        public IList<Role> FindAll(string applicationName)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ApplicationName", applicationName));
            criteria.AddOrder(Order.Asc("Name"));
            return Find(criteria, false);
        }
    }
}