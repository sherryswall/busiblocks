using System;

namespace BusiBlocks
{
    /// <summary>
    /// Interface used to automatically update UpdateDate and InsertDate fields of an entity using nhibernate interceptor.
    /// </summary>
    public interface IAudit
    {
        /// <summary>
        /// Gets or sets the Insert Data (updated when the entity is first insert in the database)
        /// </summary>
        DateTime InsertDate { get; set; }

        /// <summary>
        /// Gets or sets the Update Date (updated each time the entity is updated)
        /// </summary>
        DateTime UpdateDate { get; set; }
    }

    public static class AuditableProperties
    {
        public const string FIELD_INSERTDATE = "InsertDate";
        public const string FIELD_UPDATEDATE = "UpdateDate";
    }
}