using System;
using System.Collections.Generic;
using NetMQ;

namespace POC.Messaging.ZeroMq
{
    public class ZeroMqMessageQueue : MessageQueueBase
    {
        private readonly NetMQContext _context;
        private readonly NetMQSocket _socket;

        public ZeroMqMessageQueue(NetMQContext context, string address, Direction direction, MessagePattern pattern, IDictionary<string, object> properties)
            : base(address, direction, pattern, properties)
        {
            _context = context;
            _socket = direction == Direction.Inbound ? GetInboundSocket() : GetOutboundSocket();
        }

        private NetMQSocket GetInboundSocket()
        {
            NetMQSocket socket;
            switch (Pattern)
            {
                case MessagePattern.FireAndForget:
                    socket = _context.CreatePullSocket();
                    socket.Bind(Address);
                    break;
                case MessagePattern.RequestResponse:
                    socket = _context.CreateResponseSocket();
                    socket.Bind(Address);
                    break;
                case MessagePattern.PublishSubscribe:
                    var subSocket = _context.CreateSubscriberSocket();
                    subSocket.SubscribeToAnyTopic();
                    subSocket.Connect(Address);
                    socket = subSocket;
                    break;
                default:
                    throw new Exception($"Cannot create an inbound socket for pattern {Pattern}");
            }

            return socket;
        }

        private NetMQSocket GetOutboundSocket()
        {
            NetMQSocket socket;
            switch (Pattern)
            {
                case MessagePattern.FireAndForget:
                    socket = _context.CreatePushSocket();
                    socket.Connect(Address);
                    break;
                case MessagePattern.RequestResponse:
                    socket = _context.CreateRequestSocket();
                    socket.Connect(Address);
                    break;
                case MessagePattern.PublishSubscribe:
                    socket = _context.CreatePublisherSocket();
                    socket.Bind(Address);
                    break;
                default:
                    throw new Exception($"Cannot create an outbound socket for pattern {Pattern}");
            }

            return socket;
        }


        public override void Dispose()
        {
            _socket.Dispose();
        }

        public override IMessageQueue GetReplyQueue(Message message) => this;

        public override IMessageQueue GetResponseQueue() => this;

        public override void Listen(Action<Message> onMessageReceived)
        {
            while (true)
            {
                Receive(onMessageReceived);
            }
        }

        public override void Receive(Action<Message> onMessageReceved)
        {
            var inbound = _socket.ReceiveFrameString();
            var message = Message.FromJson(inbound);
            onMessageReceved(message);
        }

        public override void Send(Message message)
        {
            var json = message.ToJson();
            _socket.SendFrame(json);
        }
    }
}
