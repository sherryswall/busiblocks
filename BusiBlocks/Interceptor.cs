using System;
using System.Collections;
using NHibernate;
using NHibernate.Type;

namespace BusiBlocks
{
    [Serializable]
    public class InterceptorBase : IInterceptor
    {
        #region IInterceptor Members

        public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState,
                                       string[] propertyNames, IType[] types)
        {
            return null;
        }

        public virtual object Instantiate(Type type, object id)
        {
            return null;
        }

        public virtual object IsUnsaved(object entity)
        {
            return null;
        }

        public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return false;
        }

        public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
        }

        public virtual void PostFlush(ICollection entities)
        {
        }

        public virtual void PreFlush(ICollection entities)
        {
        }

        public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
                                         string[] propertyNames, IType[] types)
        {
            return false;
        }

        public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return false;
        }

        public void AfterTransactionBegin(ITransaction tx)
        {
        }

        public void AfterTransactionCompletion(ITransaction tx)
        {
        }

        public void BeforeTransactionCompletion(ITransaction tx)
        {
        }

        public void SetSession(ISession session)
        {
        }

        #endregion
    }


    /// <summary>
    /// NHibernate interceptor used to update fields UpdateDate and InsertDate of the entities that use the IAudit interface
    /// </summary>
    [Serializable]
    public class BusiBlocksInterceptor : InterceptorBase
    {
        /// <summary>
        /// On update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="currentState"></param>
        /// <param name="previousState"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
                                          string[] propertyNames, IType[] types)
        {
            if (entity is IAudit)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == AuditableProperties.FIELD_UPDATEDATE)
                    {
                        currentState[i] = DateTime.Now;
                        return true;
                    }
                }
            }

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        /// <summary>
        /// On Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity is IAudit)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == AuditableProperties.FIELD_INSERTDATE)
                    {
                        state[i] = DateTime.Now;
                    }
                    else if (propertyNames[i] == AuditableProperties.FIELD_UPDATEDATE)
                    {
                        state[i] = DateTime.Now;
                    }
                }

                return true;
            }

            return base.OnSave(entity, id, state, propertyNames, types);
        }
    }
}