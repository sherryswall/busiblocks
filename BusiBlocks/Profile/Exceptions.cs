using System;

namespace BusiBlocks.Profile
{
    [Serializable]
    public class ProfileValueNotSupportedException : BusiBlocksException
    {
        public ProfileValueNotSupportedException(string propertyName)
            : base("Profile property " + propertyName + " cannot be deserialized, value type not supported.")
        {
        }
    }
}