using System;

namespace BusiBlocks.Membership
{
    [Serializable]
    public class UserNotFoundException : BusiBlocksException
    {
        public UserNotFoundException(string user)
            : base("User " + user + " not found or invalid password")
        {
        }
    }

    [Serializable]
    public class UserNotDeletedException : BusiBlocksException
    {
        public UserNotDeletedException(string user)
            : base("User " + user + " not deleted")
        {
        }
    }

    [Serializable]
    public class EmailRequiredException : BusiBlocksException
    {
        public EmailRequiredException(string user)
            : base("User " + user + " don't have a valid email specified.")
        {
        }
    }


    [Serializable]
    public class EmailNotValidException : BusiBlocksException
    {
        public EmailNotValidException(string email)
            : base("EMail " + email + " not valid")
        {
        }
    }

    [Serializable]
    public class EmailDuplicatedException : BusiBlocksException
    {
        public EmailDuplicatedException(string email)
            : base("EMail " + email + " duplicated")
        {
        }
    }
}