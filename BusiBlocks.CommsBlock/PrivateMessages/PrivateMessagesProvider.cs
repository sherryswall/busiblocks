using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using BusiBlocks.Membership;

namespace BusiBlocks.CommsBlock.PrivateMessages
{
    /// <summary>
    /// PrivateMessageProvider abstract class. 
    /// A PrivateMessageProvider can be used to store PrivateMessages.
    /// 
    /// </summary>
    public abstract class PrivateMessagesProvider : ProviderBase
    {

        public abstract IList<PrivateMessage> FindByRecipient(User user);

        public abstract IList<PrivateMessage> FindBySender(User user);

        public abstract PrivateMessage FindById(string Id);

        public abstract void Update(PrivateMessage PrivateMessage);

        public abstract void Insert(PrivateMessage PrivateMessage);

        //public abstract void DeletePrivateMessage(string MessageId);

    }
}
