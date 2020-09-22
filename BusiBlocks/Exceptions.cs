using System;
using System.Runtime.Serialization;

namespace BusiBlocks
{
    [Serializable]
    public class BusiBlocksException : ApplicationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BusiBlocksException()
        {
        }

        public BusiBlocksException(string message) : base(message)
        {
        }

        public BusiBlocksException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BusiBlocksException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class ConfigurationNotFoundException : BusiBlocksException
    {
        public ConfigurationNotFoundException(string name)
            : base("Configuration or connection string '" + name + "' not found")
        {
        }
    }

    [Serializable]
    public class ConfigurationAlreadyExistsException : BusiBlocksException
    {
        public ConfigurationAlreadyExistsException(string name)
            : base("Configuration '" + name + "' already exists")
        {
        }
    }

    [Serializable]
    public class ConnectionElementNotFoundException : BusiBlocksException
    {
        public ConnectionElementNotFoundException(string element)
            : base("Connection string element " + element + " not found in the specified connection")
        {
        }
    }

    [Serializable]
    public class EntityNotFoundException : BusiBlocksException
    {
        public EntityNotFoundException()
            : base("Entity not found")
        {
        }
    }

    [Serializable]
    public class TextNotValidException : BusiBlocksException
    {
        public TextNotValidException(Exception innerException)
            : base("Text not valid: " + innerException.Message, innerException)
        {
        }
    }

    [Serializable]
    public class SearchStringTooLongException : BusiBlocksException
    {
        public SearchStringTooLongException()
            : base("Search string too long")
        {
        }
    }

    [Serializable]
    public class InvalidPermissionException : BusiBlocksException
    {
        public InvalidPermissionException(string action)
            : base("You don't have the permission to " + action)
        {
        }
    }

    [Serializable]
    public class InvalidGroupMembershipException : BusiBlocksException
    {
        public InvalidGroupMembershipException()
            : base("You don't belong to the correct group")
        {
        }
    }

    [Serializable]
    public class FileAttachNotFoundException : BusiBlocksException
    {
        public FileAttachNotFoundException(string id)
            : base("FileAttach " + id + " not found")
        {
        }
    }

    [Serializable]
    public class FileExceedMaxSizeException : BusiBlocksException
    {
        public FileExceedMaxSizeException(string file)
            : base("File " + file + " exceed the maximum file size")
        {
        }
    }

    [Serializable]
    public class FileExtensionNotValidException : BusiBlocksException
    {
        public FileExtensionNotValidException(string file)
            : base("File extension for " + file + " cannot be used for upload")
        {
        }
    }

    [Serializable]
    public class CodeInvalidCharsException : BusiBlocksException
    {
        public CodeInvalidCharsException(string fieldName, string invalidChars)
            : base("Field " + fieldName + " is not valid, cannot contains any of these characters: " + invalidChars)
        {
        }
    }

    [Serializable]
    public class TagInvalidException : BusiBlocksException
    {
        public TagInvalidException(string tag)
            : base("Tag " + tag + " not supported")
        {
        }
    }

    [Serializable]
    public class TagAttributeInvalidException : BusiBlocksException
    {
        public TagAttributeInvalidException(string attribute)
            : base("Element attribute " + attribute + " not supported")
        {
        }
    }

    [Serializable]
    public class SchemaIntegrityException : BusiBlocksException
    {
        public SchemaIntegrityException(string error)
            : base("Operation not valid, " + error)
        {
        }
    }
}