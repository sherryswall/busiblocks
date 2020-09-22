namespace BusiBlocks.Roles
{
    public class Role
    {
        protected Role()
        {
        }

        public Role(string applicationName, string name)
        {
            ApplicationName = applicationName;
            Name = name;
        }


        public virtual string Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string ApplicationName { get; protected set; }

        public virtual string Comment { get; set; }

        /// <summary>
        /// Field that can be used for user defined extensions.
        /// </summary>
        public virtual string Tag { get; set; }

        public virtual bool Deleted { get; set; }
    }
}