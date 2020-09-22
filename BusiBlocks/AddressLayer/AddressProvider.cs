using System.Collections.Generic;
using System.Configuration.Provider;

namespace BusiBlocks.AddressLayer
{
    public abstract class AddressProvider : ProviderBase
    {
        public abstract void CreateAddress(Address Address);

        public abstract IList<Address> GetAllAddresss();

        public abstract IList<Address> GetAllAddresssBySuburb(string suburb);

        public abstract IList<Address> GetAllAddresssByPostcode(string postcode);

        public abstract Address GetAddressById(string addressId);

        public abstract Address GetAddressByDetails(string number, string street, string suburb, string postcode,
                                                    string state);

        public abstract void UpdateAddress(Address Address);

        public abstract void DeleteAddress(Address Address);

        public abstract bool AddressExists(Address address);

        public abstract bool AddressExists(string address1, string address2, string suburb, string postcode,
                                           string state);
    }
}