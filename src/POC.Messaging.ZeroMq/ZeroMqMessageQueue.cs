using System;
using NetMQ;

namespace POC.Messaging.ZeroMq
{
    public class ZeroMqMessageQueue : MessageQueueBase<NetMQSocket>
    {
        public ZeroMqMessageQueue(NetMQContext context, IMessageQueueConnection connection, IMessageQueueFactory queueFactory)
            : base(connection, queueFactory)
        {
            Queue = Connection.Direction == Direction.Inbound ? GetInboundSocket(context) : GetOutboundSocket(context);
        }

        private NetMQSocket GetInboundSocket(NetMQContext context)
        {
            NetMQSocket socket;
            switch (Connection.Pattern)
            {
                case MessagePattern.FireAndForget:
                    socket = context.CreatePullSocket();
                    socket.Bind(Connection.Address);
                    break;
                case MessagePattern.RequestResponse:
                    socket = context.CreateResponseSocket();
                    socket.Bind(Connection.Address);
                    break;
                case MessagePattern.PublishSubscribe:
                    var subSocket = context.CreateSubscriberSocket();
                    subSocket.SubscribeToAnyTopic();
                    subSocket.Connect(Connection.Address);
                    socket = subSocket;
                    break;
                default:
                    throw new Exception($"Cannot create an inbound socket for pattern {Connection.Pattern}");
            }

            return socket;
        }

        private NetMQSocket GetOutboundSocket(NetMQContext context)
        {
            NetMQSocket socket;
            switch (Connection.Pattern)
            {
                case MessagePattern.FireAndForget:
                    socket = context.CreatePushSocket();
                    socket.Connect(Connection.Address);
                    break;
                case MessagePattern.RequestResponse:
                    socket = context.CreateRequestSocket();
                    socket.Connect(Connection.Address);
                    break;
                case MessagePattern.PublishSubscribe:
                    socket = context.CreatePublisherSocket();
                    socket.Bind(Connection.Address);
                    break;
                default:
                    throw new Exception($"Cannot create an outbound socket for pattern {Connection.Pattern}");
            }

            return socket;
        }


        public override void Dispose()
        {
            Queue.Dispose();            
        }

        public override IMessageQueue GetReplyQueue(Message message) => this;

        public override IMessageQueue GetResponseQueue() => this;
        
        public override void Receive(Action<Message> onMessageReceived)
        {
            var inbound = Queue.ReceiveFrameString();
            var message = Message.FromJson(inbound);
            onMessageReceived(message);
        }

        public override void Send(Message message)
        {
            var json = message.ToJson();
            Queue.SendFrame(json);
        }
    }
}
