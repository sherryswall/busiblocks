namespace BusiBlocks.PersonLayer
{
    public class PersonPersonType
    {
        public virtual string Id { get; set; }
        public virtual Person Person { get; set; }
        public virtual PersonType PersonType { get; set; }

        public virtual bool Deleted { get; set; }
    }
}