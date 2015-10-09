using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Integration
{
    public interface IMessageHandler<T>
    {
        void Handle(T message);                
    }    
}
