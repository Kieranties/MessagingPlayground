using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace POC.Messaging
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {   
        public MessageHandlerFactory(IEnumerable<IMessageHandler> handlers)
        {
            Handlers = BuildCache(handlers);
        }

        protected ConcurrentDictionary<Type, IMessageHandler> Handlers { get; set; }

        public IMessageHandler GetHandler(Type type)
        {
            if (HasHandler(type)) return Handlers[type];

            return null;
        }

        public bool HasHandler(Type type) => Handlers.ContainsKey(type);

        protected virtual ConcurrentDictionary<Type, IMessageHandler> BuildCache(IEnumerable<IMessageHandler> handlers)
        {
            var dict = new ConcurrentDictionary<Type, IMessageHandler>();
            foreach(var handler in handlers)
            {
                dict.TryAdd(handler.HandleType, handler);
            }

            return dict;
        }
    }
}
