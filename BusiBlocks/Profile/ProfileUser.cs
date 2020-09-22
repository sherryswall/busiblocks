using System;
using System.Collections.Generic;

namespace BusiBlocks.Profile
{
    public class ProfileUser : IAudit
    {
        private DateTime _lastActivityDate = DateTime.Now;
        private DateTime _lastPropertyChangedDate = DateTime.Now;
        private string mName;

        protected ProfileUser()
        {
        }

        public ProfileUser(string applicationName, string userName, ProfileType profileType)
        {
            ApplicationName = applicationName;
            Name = userName;
            ProfileType = profileType;
        }


        public virtual string Id { get; protected set; }

        public virtual string Name
        {
            get { return mName; }
            protected set
            {
                EntityHelper.ValidateCode("Name", value);
                mName = value;
            }
        }

        public virtual string ApplicationName { get; protected set; }

        public virtual ProfileType ProfileType { get; set; }

        /// <summary>
        /// Changes when calling SetPropertyValues and GetPropertyValues method
        /// </summary>
        public virtual DateTime LastActivityDate
        {
            get { return _lastActivityDate; }
            set { _lastActivityDate = value; }
        }

        /// <summary>
        /// This property differs from the UpdateDate because change only when calling SetPropertyValues method
        /// </summary>
        public virtual DateTime LastPropertyChangedDate
        {
            get { return _lastPropertyChangedDate; }
            set { _lastPropertyChangedDate = value; }
        }

        /// <summary>
        /// List used for cascading rules
        /// </summary>
        protected IList<ProfileProperty> Properties { get; set; }

        #region IAudit Members

        public virtual DateTime InsertDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }

        #endregion
    }

    public enum ProfileType
    {
        Anonymous = 1,
        Authenticated = 2
    }
}