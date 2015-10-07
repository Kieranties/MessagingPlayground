using Microsoft.Framework.Logging;
using NetMQ;
using System;

namespace ZMQ.Sockets
{
    public abstract class AbstractSocket : IDisposable
    {
        public AbstractSocket(ILogger logger)
        {
            Logger = logger;

            Context = NetMQContext.Create();    
        }
        
        protected NetMQContext Context { get; set; }        

        protected ILogger Logger { get; set; }

        public virtual void Dispose()
        {
            Context.Dispose();
        }
        
        public abstract void Start(Options options);        
    }
}
