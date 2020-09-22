namespace BusiBlocks.PersonLayer
{
    public class PersonType
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual bool Deleted { get; set; }
    }
}