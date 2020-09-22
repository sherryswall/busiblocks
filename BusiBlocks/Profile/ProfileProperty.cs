namespace BusiBlocks.Profile
{
    public class ProfileProperty
    {
        private string mName;

        protected ProfileProperty()
        {
        }

        public ProfileProperty(ProfileUser pUser, string pName)
        {
            User = pUser;
            Name = pName;
        }

        public virtual string Id { get; protected set; }

        public virtual ProfileUser User { get; protected set; }

        public virtual string Name
        {
            get { return mName; }
            protected set
            {
                EntityHelper.ValidateCode("Name", value);
                mName = value;
            }
        }

        public virtual string StringValue { get; protected set; }

        public virtual byte[] BinaryValue { get; protected set; }

        public virtual void SetValue(byte[] val)
        {
            StringValue = null;
            BinaryValue = val;
        }

        public virtual void SetValue(string val)
        {
            BinaryValue = null;
            StringValue = val;
        }

        public virtual void SetNull()
        {
            BinaryValue = null;
            StringValue = null;
        }
    }
}