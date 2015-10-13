using System.Collections.Generic;

namespace POC.Messaging
{
    public interface IMessageQueueFactory
    {
        IMessageQueue CreateInbound(string name, MessagePattern pattern, IDictionary<string, object> properties = null);

        IMessageQueue CreateOutbound(string name, MessagePattern pattern, IDictionary<string, object> properties = null);

        string GetAddress(string name);
    }
}
