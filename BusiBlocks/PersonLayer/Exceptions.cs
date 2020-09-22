using System;
using System.Runtime.Serialization;

namespace BusiBlocks.PersonLayer
{
    [Serializable]
    public class PersonNotFoundException : BusiBlocksException
    {
        public PersonNotFoundException(string user)
            : base("No person exists for user " + user)
        {
        }

        public PersonNotFoundException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public PersonNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        protected PersonNotFoundException(SerializationInfo info,
                                          StreamingContext context)
            : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }
}