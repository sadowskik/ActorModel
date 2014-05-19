using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ActorModel.Infrastructure.Actors;

namespace Client2
{        
    public class TcpServiceClient
    {
        private readonly Actor _sendingActor;
                
        public TcpServiceClient(IPEndPoint serverEndPoint)
        {
            _sendingActor = new Sender(serverEndPoint);
        }

        public Task Send()
        {
            var source = new TaskCompletionSource<bool>();
            
            var send = new SendMessage(_sendingActor.Id, "testMessage", source);
            _sendingActor.Handle(send);

            return source.Task;
        }

        public class Sender : Actor
        {
            private readonly TcpClient _client;

            public Sender(IPEndPoint serverEndPoint)
                : base(ActorId.GenerateNew())
            {
                _client = new TcpClient();
                _client.Connect(serverEndPoint);
            }

            public override void Dispose()
            {
                base.Dispose();
                _client.Close();
            }

            public void On(SendMessage msg)
            {
                try
                {
                    var clientStream = _client.GetStream();
                    var encoder = new ASCIIEncoding();
                    byte[] buffer = encoder.GetBytes(msg.Message);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();

                    msg.Completion.SetResult(true);
                }
                catch (Exception e)
                {
                    msg.Completion.SetException(e);
                    throw;
                }
            }
        }

        public class SendMessage : Message
        {
            private readonly ActorId _destinationActorId;

            public string Message { get; private set; }
            public TaskCompletionSource<bool> Completion { get; private set; }

            public SendMessage(ActorId destinationActorId, string message, TaskCompletionSource<bool> completion)
            {
                _destinationActorId = destinationActorId;
                Message = message;
                Completion = completion;
            }

            public override ActorId DestinationActorId
            {
                get { return _destinationActorId; }
            }
        }
    }
}