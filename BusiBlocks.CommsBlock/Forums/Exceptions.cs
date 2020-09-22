using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.Forums
{
    [Serializable]
    public class TopicNotFoundException : BusiBlocksException
    {
        public TopicNotFoundException(string id)
            : base("Topic " + id + " not found")
        {

        }
    }

    [Serializable]
    public class MessageNotFoundException : BusiBlocksException
    {
        public MessageNotFoundException(string id)
            : base("Message " + id + " not found")
        {

        }
    }

    [Serializable]
    public class ForumCategoryNotFoundException : BusiBlocksException
    {
        public ForumCategoryNotFoundException(string id)
            : base("Forum " + id + " not found")
        {

        }
    }

    [Serializable]
    public class ForumNotSpecifiedException : BusiBlocksException
    {
        public ForumNotSpecifiedException()
            : base("Forum list not specified")
        {

        }
    }
}
