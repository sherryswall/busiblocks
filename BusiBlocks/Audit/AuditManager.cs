using System;
using System.Collections.Generic;

//FxCop Warning:[Avoid Namespaces with few types, to be considered again if we merge Reporting and Audit.]

namespace BusiBlocks.Audit
{
    public static class AuditManager
    {
        public static void Audit(string userName, string data, AuditRecord.AuditAction action)
        {
            var auditRecord = new AuditRecord();
            auditRecord.Action = action.ToString();
            auditRecord.UserName = userName;
            auditRecord.Data = data;
            auditRecord.TimeStamp = DateTime.Now;

            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);
                //if (!store.AuditRecordExist(username, data, action.ToString()))
                //{
                store.Insert(auditRecord);
                transaction.Commit();
                //}
            }
        }

        public static void DeleteAllAudits()
        {
            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);

                IList<AuditRecord> records = store.FindAll();

                foreach (AuditRecord record in records)
                {
                    store.Delete(record.Id);
                }

                transaction.Commit();
            }
        }

        public static void DeleteAllAuditsAfterDate(DateTime date)
        {
            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);

                IList<AuditRecord> records = store.FindAllFromDate(date);

                foreach (AuditRecord record in records)
                {
                    store.Delete(record.Id);
                }

                transaction.Commit();
            }
        }


        public static IList<AuditRecord> GetAuditItems(string userName, string data, AuditRecord.AuditAction action)
        {
            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            IList<AuditRecord> records;

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);
                records = store.Find(userName, data, action.ToString());
            }

            return records;
        }

        public static IList<AuditRecord> GetAuditItems(string data, AuditRecord.AuditAction action)
        {
            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            IList<AuditRecord> records;

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);
                records = store.Find(data, action.ToString());
            }

            return records;
        }



        public static AuditRecord GetAuditItem(string Id)
        {
            ConnectionParameters parameters = ConnectionParameters.Create("DefaultDB");

            AuditRecord record;

            using (var transaction = new TransactionScope(parameters))
            {
                var store = new AuditDataStore(transaction);

                record = store.FindByKey(Id);
            }

            return record;
        }
    }
}