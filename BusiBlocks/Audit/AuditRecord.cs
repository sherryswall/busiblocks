using System;

//FxCop Warning:[Avoid Namespaces with few types, to be considered again if we merge Reporting and Audit.]

namespace BusiBlocks.Audit
{
    public class AuditRecord
    {
        #region AuditAction enum

        public enum AuditAction
        {
            Viewed,
            Acknowledged,
            LogOn
        }

        #endregion

        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Action { get; set; }
        public virtual string Data { get; set; }
        public virtual DateTime TimeStamp { get; set; }
    }
}