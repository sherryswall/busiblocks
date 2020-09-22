using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.PersonLayer
{
    public class PersonTypeRoleDataStore : EntityDataStoreBase<PersonTypeRole, string>
    {
        public PersonTypeRoleDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<PersonTypeRole> FindAll()
        {
            ICriteria criteria = CreateCriteria();
            return base.Find(criteria, false);
        }

        public IList<PersonTypeRole> FindPersonTypeRolesByPersonType(string personTypeId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("PersonType.Id", personTypeId, MatchMode.Exact));

            return Find(criteria, false);
        }

        public IList<PersonTypeRole> FindPersonTypesByRole(string roleId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Role.Id", roleId, MatchMode.Exact));

            return Find(criteria, false);
        }
    }
}