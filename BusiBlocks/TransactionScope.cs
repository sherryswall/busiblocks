using System;
using System.Data;
using NHibernate;

namespace BusiBlocks
{
    /// <summary>
    /// The TransactionScope class identify the scope of the transaction used by the BusiBlocks classes.
    /// Contains a nhibernate transaction and session. (NHibernateTransaction, NHibernateSession).
    /// </summary>
    public class TransactionScope : IDisposable
    {
        private readonly bool mDispose;
        private ISession mSession;
        private ITransaction mTransaction;

        /// <summary>
        /// Create a transaction scope using the specified session and transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="transaction"></param>
        /// <param name="dispose">Set to true to dispose the objects and RollBack the transaction if still active when the TransactionScope is disposed.
        /// This can be useful when you share the ITransaction with other clasess</param>
        public TransactionScope(ISession session,
                                ITransaction transaction,
                                bool dispose)
        {
            mSession = session;
            mTransaction = transaction;
            mDispose = dispose;
        }

        /// <summary>
        /// Create a new transaction scope with a new session and a new transaction.
        /// The session and transaction are created based on the ConnectionParameters class.
        /// Both objects are disposed at the end.
        /// </summary>
        /// <param name="configuration"></param>
        public TransactionScope(ConnectionParameters configuration)
        {
            mSession = configuration.OpenSession();
            mTransaction = mSession.BeginTransaction();
            mDispose = true;
        }

        public ITransaction NHibernateTransaction
        {
            get { return mTransaction; }
        }

        public ISession NHibernateSession
        {
            get { return mSession; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (mDispose)
            {
                //Usually the Transaction implementation automatically rollback the transaction when Dispose is called
                // but seems that MySql doesn't follow this rule so I manually call a RollBack to be sure.
                if (mTransaction.IsActive)
                    mTransaction.Rollback();

                mTransaction.Dispose();
                mSession.Dispose();
            }

            mTransaction = null;
            mSession = null;
        }

        #endregion

        public void Rollback()
        {
            mTransaction.Rollback();
        }

        public void Commit()
        {
            mTransaction.Commit();
        }

        public IDbCommand CreateDbCommand()
        {
            IDbCommand command = NHibernateSession.Connection.CreateCommand();
            NHibernateTransaction.Enlist(command);
            return command;
        }

        public IDbDataParameter CreateDbCommandParameter(IDbCommand command, string parameterName, DbType type,
                                                         object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = type;
            param.Value = value;
            return param;
        }
    }
}