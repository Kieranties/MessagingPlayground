using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Messaging
{
    public abstract class MessageQueueFactoryBase : IMessageQueueFactory
    {
        private readonly IDictionary<string, string> _addressMapping;        
        private readonly ConcurrentDictionary<string, IMessageQueue> _queues = new ConcurrentDictionary<string, IMessageQueue>();

        protected MessageQueueFactoryBase(IDictionary<string, string> addressMapping)
        {
            _addressMapping = addressMapping;
        }

        public virtual IMessageQueue CreateInbound(string name, MessagePattern pattern, IDictionary<string, object> properties = null)
        {
            return _queues.GetOrAdd(name, key => CreateQueue(name, Direction.Inbound, pattern, properties));
        }

        public virtual IMessageQueue CreateOutbound(string name, MessagePattern pattern, IDictionary<string, object> properties = null)
        {
            return _queues.GetOrAdd(name, key => CreateQueue(name, Direction.Outbound, pattern, properties));
        }

        public virtual string GetAddress(string name)
        {
            string address;
            _addressMapping.TryGetValue(name.ToLowerInvariant(), out address);

            return address;
        }

        protected abstract IMessageQueue CreateQueue(string name, Direction direction, MessagePattern pattern, IDictionary<string, object> properties);
    }
}
