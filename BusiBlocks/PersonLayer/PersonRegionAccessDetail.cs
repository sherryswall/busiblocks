namespace BusiBlocks.PersonLayer
{
    public class PersonRegionAccessDetail
    {
        public virtual string PersonId { get; set; }
        public virtual string LocationId { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsView { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsPrimary { get; set; }
    }
}
