using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Messaging
{
    public interface IMessageHandlerFactory
    {
        bool HasHandler(Type type);

        IMessageHandler GetHandler(Type type);
    }
}
