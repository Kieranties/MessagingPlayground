using Microsoft.ServiceBus.Messaging;
using System;

namespace POC.Messaging.Azure
{
    public interface IAzureClient : IDisposable
    {
        void Send(BrokeredMessage message);

        BrokeredMessage Receive();
    }
}
