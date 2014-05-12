using System.Net.Sockets;
using ActorModel.Infrastructure.Actors;

namespace Server.Messages
{
    public class ClientConnected : Message
    {
        private readonly ActorId _destinationId;

        public ClientConnected(ActorId destinationId, TcpClient client)
        {
            _destinationId = destinationId;
            Client = client;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }

        public TcpClient Client { get; private set; }
    }    
}