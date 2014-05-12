using ActorModel.Infrastructure.Actors;

namespace Server.Messages
{
    public class StartListening : Message
    {
        public override ActorId DestinationActorId
        {
            get { return Addresses.Listener; }
        }
    }
}