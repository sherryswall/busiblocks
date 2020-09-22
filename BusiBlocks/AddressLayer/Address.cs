namespace BusiBlocks.AddressLayer
{
    public class Address
    {
        public virtual string Id { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string State { get; set; }
        public virtual bool Deleted { get; set; }

        public override bool Equals(object obj)
        {
            var otherAddress = obj as Address;
            if (otherAddress == null || Address1 == null)
                return false;

            if (!Address1.Equals(otherAddress.Address1))
                return false;
            if (Address2 != null)
            {
                if (!Address2.Equals(otherAddress.Address2))
                    return false;
            }
            if (Suburb != null)
            {
                if (!Suburb.Equals(otherAddress.Suburb))
                    return false;
            }
            if (Postcode != null)
            {
                if (!Postcode.Equals(otherAddress.Postcode))
                    return false;
            }
            if (State != null)
            {
                if (!State.Equals(otherAddress.State))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}