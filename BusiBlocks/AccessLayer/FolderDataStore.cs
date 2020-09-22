using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.AccessLayer
{
    public class FolderDataStore : EntityDataStoreBase<Folder, string>
    {
        public FolderDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public Folder FindByPath(string path)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Path", path));

            return FindUnique(criteria, null);
        }
    }
}