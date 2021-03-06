﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace POC.Messaging
{
    public abstract class MessageQueueFactoryBase<T> : IMessageQueueFactory where T : IMessageQueueConnection
    {
        private readonly IDictionary<string, T> _connectionMap;
        private readonly ConcurrentDictionary<string, IMessageQueue> _queues = new ConcurrentDictionary<string, IMessageQueue>();

        protected MessageQueueFactoryBase(IList<T> connectionMap)
        {
            _connectionMap = connectionMap.ToDictionary(entry => entry.Id, entry => entry);
        }

        public virtual IMessageQueue Get(string id)
        {
            if (_connectionMap.ContainsKey(id)) // get or add from cache
            {
                return _queues.GetOrAdd(id, key => Connect(_connectionMap[key]));
            }
            
            return null;
        }

        public abstract IMessageQueue Connect(IMessageQueueConnection connection);

        public abstract IMessageQueue Create(IMessageQueueConnection connection);               
    }
}
