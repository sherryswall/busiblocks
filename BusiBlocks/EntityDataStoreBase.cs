using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks
{
    /// <summary>
    /// Abstract generic class that use NHibernate to save a specified entity class.
    /// This is a basic Data Access Object (DAO) pattern implementation.
    /// Use a TransactionScope instance to share the same transaction with more than one db operations.
    /// </summary>
    public abstract class EntityDataStoreBase<T, TId>
    {
        private const string Deleted = "Deleted";
        private readonly TransactionScope _transactionScope;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transactionScope">Transaction used inside this class for all the operations</param>
        protected EntityDataStoreBase(TransactionScope transactionScope)
        {
            _transactionScope = transactionScope;
        }

        public TransactionScope TransactionScope
        {
            get { return _transactionScope; }
        }

        protected ICriteria CreateCriteria()
        {
            return TransactionScope.NHibernateSession.CreateCriteria(typeof (T));
        }

        protected ICriteria CreateCriteria(Type obj)
        {
            return TransactionScope.NHibernateSession.CreateCriteria(obj);
        }

        protected IQuery CreateQuery(string hql)
        {
            return TransactionScope.NHibernateSession.CreateQuery(hql);
        }

        #region Find

        public T FindByKey(TId id)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", id));

            return FindUnique(criteria, false);
            //return _transactionScope.NHibernateSession.Get<T>(id);
        }

        protected IList<T> FindAll(bool? deleted)
        {
            ICriteria criteria = CreateCriteria();
            return Find(criteria, deleted);
        }

        protected T FindUnique(ICriteria criteria, bool? deleted)
        {
            if (deleted != null)
            {
                if (deleted == true)
                {
                    criteria.Add(Expression.Eq(Deleted, true));
                }
                else if (deleted == false)
                {
                    criteria.Add(Expression.Or(
                        Expression.Eq(Deleted, false),
                        Expression.IsNull(Deleted)
                                     ));
                }
            }
            return (T) criteria.UniqueResult();
        }

        protected IList<T> Find(ICriteria criteria, bool? deleted)
        {
            if (deleted != null)
            {
                if (deleted == true)
                {
                    criteria.Add(Expression.Eq(Deleted, true));
                }
                else if (deleted == false)
                {
                    criteria.Add(Expression.Or(
                        Expression.Eq(Deleted, false),
                        Expression.IsNull(Deleted)
                                     ));
                }
            }
            return criteria.List<T>();
        }

        protected IList<T> Find(ICriteria criteria, PagingInfo paging, bool? deleted)
        {
            paging.RowCount = Count(criteria);

            if (paging.PageSize > 0)
            {
                criteria.SetMaxResults((int) paging.PageSize);
                criteria.SetFirstResult((int) (paging.PageSize*paging.CurrentPage));
            }

            return Find(criteria, deleted);
        }

        protected int Count(ICriteria criteria)
        {
            //TODO check performance of this method (probably is better to use HQL with the COUNT(*) command)
            return criteria.List().Count;
        }

        #endregion

        #region Delete

        public virtual void Delete(TId objId)
        {
            //NOTE: I cannot directly delete an instance if the instance is not attached to the database or is not syncronized

            //TODO Think if there is a fast way to delete the entities (and also think if using an id in the providers delete methods instead of entity instance)

            object entity = FindByKey(objId);
            if (entity == null)
                throw new EntityNotFoundException();

            _transactionScope.NHibernateSession.Delete(entity);
        }

        //        /// <summary>
        //        /// Delete all the rows that match the criteria
        //        /// </summary>
        //        /// <param name="criteria"></param>
        //        /// <returns></returns>
        //        protected virtual int Delete(ICriteria criteria)
        //        {
        //#warning check if there is a way to delete all the matched rows without loading it (but consider that someone can have overriden the Delete method to custom delete actions, like delete attachments) considering using nhibernate cascade options but there is still a problems with attachments.
        //            IList<T> list = Find(criteria);

        //            foreach (T entity in list)
        //            {
        //                Delete(entity);
        //            }

        //            return list.Count;
        //        }

        #endregion

        #region Insert

        public virtual T Insert(T obj)
        {
            _transactionScope.NHibernateSession.Save(obj);
            
            return obj;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update the persistent instance with the identifier of the given transient instance.
        /// The update method automatically attach the entity to current session for updating the entity.
        /// </summary>
        /// <param name="obj">A transient instance containing updated state</param>
        public virtual void Update(T obj)
        {
            _transactionScope.NHibernateSession.Update(obj);
        }

        /// <summary>
        /// Either Save() or Update() the given instance, depending upon the value of its identifier property.
        /// 
        /// Internally calls NHibernate SaveOrUpdate method.
        /// </summary>
        /// <param name="obj">A transient instance containing updated state</param>
        public virtual void InsertOrUpdate(T obj)
        {
            _transactionScope.NHibernateSession.SaveOrUpdate(obj);
        }

        /// <summary>
        /// Copy the state of the given object onto the persistent object with the same
        /// identifier. If there is no persistent instance currently associated with
        /// the session, it will be loaded. Return the persistent instance. If the given
        /// instance is unsaved or does not exist in the database, save it and return
        /// it as a newly persistent instance. Otherwise, the given instance does not
        /// become associated with the session.
        /// 
        /// This method copies the state of the given object onto the persistent object with the same identifier. 
        /// If there is no persistent instance currently associated with the session, it will be loaded. 
        /// The method returns the persistent instance. 
        /// If the given instance is unsaved or does not exist in the database, NHibernate will save it 
        /// and return it as a newly persistent instance. 
        /// Otherwise, the given instance does not become associated with the session.
        /// 
        /// Internally calls NHibernate SaveOrUpdateCopy method.
        /// </summary>
        /// <param name="obj">A transient instance containing updated state</param>
        public virtual T InsertOrUpdateCopy(T obj)
        {
            return (T) _transactionScope.NHibernateSession.SaveOrUpdateCopy(obj);
        }

        #endregion

        #region Other entity related methods

        /// <summary>
        /// The Attach() method allows the application to reassociate an unmodified object with a new session.
        /// Tipically you must call this method when an entity is retrived from the db, the session is closed and then a new session is created and you want to use the previous entity.
        /// Internally calls the NHibernate Lock method.
        /// </summary>
        public void Attach(T entity)
        {
            TransactionScope.NHibernateSession.Lock(entity, LockMode.None);
        }

        #endregion
    }
}