using ActorModel.Infrastructure.Actors;

namespace StopLoss
{
    public class RefreshPrices : Message
    {
        private readonly ActorId _destinationId;

        public RefreshPrices(ActorId destinationId)
        {
            _destinationId = destinationId;
        }

        public override ActorId DestinationActorId
        {
            get { return _destinationId; }
        }
    }
}