using System.Net.Sockets;
using ActorModel.Infrastructure.Actors;

namespace Server.Messages
{
    public class ChunkReaded : Message
    {
        private readonly ActorId _destinationId;

        public ChunkReaded(ActorId destinationId, NetworkStream stream, TcpClient client)
        {
            _destinationId = destinationId;
            Stream = stream;
            Client = client;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public NetworkStream Stream { get; private set; }
        
        public TcpClient Client { get; private set; }
    }
}